using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EffectManager : MonoBehaviour
{
    public Transform playerCardsTrans;
    public Transform enemyCardsTrans;
    PhysicalCard[] playerPhysicalCards;
    PhysicalCard[] enemyPhysicalCards;

    public TMP_Text playerPointsText;
    public TMP_Text enemyPointsText;

    int playerExtraPower = 0;
    int enemyExtraPower = 0;

    int animationsDone = 0;
    int animationsStarted = 0;

    bool cardJustEntered = false;

    TurnHandler turnHandler;
    Hand hand;
    bool doubleNextCardsPower = false;
    bool justPlayedEffectForNextTurn = false;
    int givePowerToNextCard = 0;

    int playerPointTotal = 0;
    int enemyPointTotal = 0;

    private void Start()
    {
        turnHandler = GameObject.FindObjectOfType<TurnHandler>();
        hand = GameObject.FindObjectOfType<Hand>();
        GetPhysicalCards();
        int enemyPointTotal = 0;
        foreach (PhysicalCard card in enemyPhysicalCards)
        {
            enemyPointTotal += card.GetPower();
        }
        enemyPointsText.text = enemyPointTotal.ToString();
    }

    public void AddToPoints(int amount, bool enemy)
    {
        if (enemy)
        {
            enemyExtraPower += amount;
        }
        else
            playerExtraPower += amount;

        StartCoroutine(CalculatePoints());
    }

    public void ResetExtraPoints()
    {
            enemyExtraPower = 0;
            playerExtraPower =0;

        StartCoroutine(CalculatePoints());
    }

    public int GetPoints(bool enemy)
    {
        if (enemy)
        {
            return enemyPointTotal;
        }
        else
            return playerPointTotal;
    }

    IEnumerator CalculatePoints()
    {
        yield return new WaitForEndOfFrame();
        GetPhysicalCards();
        playerPointTotal = 0;
        foreach (PhysicalCard card in playerPhysicalCards)
        {
            playerPointTotal += card.GetPower();
        }
        playerPointTotal += playerExtraPower;
        playerPointsText.text = playerPointTotal.ToString();

        enemyPointTotal = 0;
        foreach (PhysicalCard card in enemyPhysicalCards)
        {
            enemyPointTotal += card.GetPower();
        }
        enemyPointTotal += enemyExtraPower;
        enemyPointsText.text = enemyPointTotal.ToString();
    }

    void GetPhysicalCards()
    {
        playerPhysicalCards = playerCardsTrans.GetComponentsInChildren<PhysicalCard>();
        enemyPhysicalCards = enemyCardsTrans.GetComponentsInChildren<PhysicalCard>();
    }
    List<PhysicalCard> targets;
    public void DoEffect(Effect[] effects, GameObject cardGameObject, GameObject cardThatTriggeredEffect = null)
    {
        GetPhysicalCards(); 

        bool isEnemyTurn = turnHandler.IsEnemyTurn();

        PhysicalCard[] playerCards;
        PhysicalCard[] enemyCards;

        if (isEnemyTurn)
        {
            playerCards = enemyPhysicalCards;
            enemyCards = playerPhysicalCards;
        }
        else
        {
            enemyCards = enemyPhysicalCards;
            playerCards = playerPhysicalCards;
        }

        //Targets

        List<DropZone> spaces = new List<DropZone>();
        targets = new List<PhysicalCard>();

        foreach (Effect effect in effects)
        {
            switch (effect.target)
            {
                case Target.AllOtherPals:
                    foreach (PhysicalCard physCard in playerCards)
                    {
                        if (physCard.gameObject != cardGameObject)
                            if (effect.targetTribe == Tribe.None || effect.targetTribe == physCard.GetCard().GetTribe())
                                targets.Add(physCard);
                    }
                    break;

                case Target.Itself:
                    targets.Add(cardGameObject.GetComponent<PhysicalCard>());
                    break;

                case Target.AllEnemies:
                    foreach (PhysicalCard physCard in enemyCards)
                    {
                        if (physCard.gameObject != cardGameObject)
                            if (effect.targetTribe == Tribe.None || effect.targetTribe == physCard.GetCard().GetTribe())
                                targets.Add(physCard);
                    }
                    break;
                case Target.RandomOpponent:
                    List<PhysicalCard> tempEnemyCards = new List<PhysicalCard>();

                    foreach (PhysicalCard enemyCard in enemyCards)
                    {
                        if (enemyCard.gameObject != cardGameObject)
                            tempEnemyCards.Add(enemyCard);
                    }
                    
                    for (int i = 0; i < effect.targetVal1; i++)
                    {
                        if (tempEnemyCards.Count > 0)
                        {
                            int randIndex = Random.Range(0, tempEnemyCards.Count);
                            targets.Add(tempEnemyCards[randIndex]);
                            tempEnemyCards.RemoveAt(randIndex);
                        }
                    }
                    break;
                case Target.RandomPal:
                    List<PhysicalCard> tempPlayerCards = new List<PhysicalCard>();

                    foreach (PhysicalCard playerCard in playerCards)
                    {
                        if (playerCard.gameObject != cardGameObject)
                            tempPlayerCards.Add(playerCard);
                    }

                    for (int i = 0; i < effect.targetVal1; i++)
                    {
                        if (tempPlayerCards.Count > 0)
                        {
                            int randIndex = Random.Range(0, tempPlayerCards.Count);
                            targets.Add(tempPlayerCards[randIndex]);
                            tempPlayerCards.RemoveAt(randIndex);
                        }
                    }
                    break;
                case Target.StrongestOpponent:
                    PhysicalCard strongestCard = null;
                    foreach (PhysicalCard physCard in enemyCards)
                    {
                        if (physCard.gameObject != cardGameObject)
                        {
                            if (!strongestCard)
                                strongestCard = physCard;
                            else if (physCard.GetPower() > strongestCard.GetPower())
                                strongestCard = physCard;
                        }
                    }
                    targets.Add(strongestCard);
                    break;
            }

            if (effect.action == Action.IfOppLastCardHasXPowerOrLessGainPower)
            {
                if (enemyCards[enemyCards.Length - 1].GetPower() > effect.val1)
                {
                    targets = new List<PhysicalCard>();
                }
            }
            if (effect.action == Action.IfOppLastCardIsSpecificTribeGainPower)
            {
                if (enemyCards[enemyCards.Length - 1].GetCard().tribe != effect.actionTribe)
                {
                    targets = new List<PhysicalCard>();
                }
            }
            if (effect.action == Action.IfOppControlXGainPower)
            {
                bool controlTribe = false;
                foreach (PhysicalCard target in enemyCards)
                {
                    if (target.GetCard().GetTribe() == effect.actionTribe)
                        controlTribe = true;
                }

                if (!controlTribe) targets = new List<PhysicalCard>();
            }
            if (effect.action == Action.IfYouControlXGainPower)
            {
                bool controlTribe = false;
                foreach (PhysicalCard target in playerCards)
                {
                    if (target.GetCard().GetTribe() == effect.actionTribe)
                        controlTribe = true;
                }
                if (!controlTribe) targets = new List<PhysicalCard>();
            }

            foreach (PhysicalCard target in targets)
            {
                animationsStarted++;
                if (effect.action == Action.Destroy)
                    target.GetComponent<Animator>().SetInteger("Effect", 2);
                else
                    target.GetComponent<Animator>().SetInteger("Effect", 1);
            }

            print("Length of card array: " + targets.Count);
            foreach (PhysicalCard target in targets)
                print(target.gameObject);

            //Actions
            int otherCardsAmount = playerCards.Length - 1;
            int oppCardsAmount = enemyCards.Length;

            switch (effect.action)
            {
                case Action.DecreasePower:
                    foreach (PhysicalCard target in targets)
                    {
                        StartCoroutine(target.DecreasePower(effect.val1));
                    }
                    break;
                case Action.GainPower:
                    foreach (PhysicalCard target in targets)
                    {
                        StartCoroutine(target.DecreasePower(-effect.val1));
                    }
                    break;
                case Action.HalvePower:
                    foreach (PhysicalCard target in targets)
                    {
                        StartCoroutine(target.DecreasePower(target.GetPower() / 2));
                    }
                    break;
                case Action.DoublePower:
                    foreach (PhysicalCard target in targets)
                    {
                        StartCoroutine(target.DecreasePower(-target.GetPower()));
                    }
                    break;
                case Action.DecreasePowerForEachCard:

                    foreach (PhysicalCard target in targets)
                    {
                        StartCoroutine(target.DecreasePower(effect.val1 * (otherCardsAmount+oppCardsAmount)));
                    }
                    break;
                case Action.DecreasePowerForEachFriendlyCard:

                    foreach (PhysicalCard target in targets)
                    {
                        StartCoroutine(target.DecreasePower(effect.val1 * otherCardsAmount));
                    }
                    break;
                case Action.GainPowerForEachFriendlyCard:
                    otherCardsAmount = -otherCardsAmount;

                    foreach (PhysicalCard target in targets)
                    {
                        StartCoroutine(target.DecreasePower(effect.val1 * otherCardsAmount));
                    }
                    break;
                case Action.Destroy:
                    foreach (PhysicalCard target in targets)
                    {
                        StartCoroutine(target.DestroyThis());
                    }
                    break;
                case Action.NextCardPlayedDoublePower:
                    doubleNextCardsPower = true;
                    justPlayedEffectForNextTurn = true;
                    break;
                case Action.DecreasePowerOnOppNextCard:
                    doubleNextCardsPower = true;
                    givePowerToNextCard = -effect.val1;
                    break;
                case Action.IfOppLastCardHasXPowerOrLessGainPower:
                    if (enemyCards[enemyCards.Length - 1].GetPower() <= effect.val1)
                        foreach (PhysicalCard target in targets)
                        {
                            StartCoroutine(target.DecreasePower(-effect.val2));
                        }
                    break;
                case Action.IfOppLastCardIsSpecificTribeGainPower:
                    if (enemyCards[enemyCards.Length - 1].GetCard().tribe == effect.actionTribe)
                        foreach (PhysicalCard target in targets)
                        {
                            StartCoroutine(target.DecreasePower(-effect.val1));
                        }
                    break;
                case Action.GainPowerForEachOppCardOfSpecificTribe:
                    int amountOfOppSpecificTribe = 0;
                    foreach (PhysicalCard target in enemyCards)
                    {
                        if (target.GetCard().GetTribe() == effect.actionTribe)
                            amountOfOppSpecificTribe++;
                    }

                    foreach (PhysicalCard target in targets)
                    {
                        StartCoroutine(target.DecreasePower(-effect.val1 * amountOfOppSpecificTribe));
                    }
                    break;
                case Action.DecreasePowerForEachFriendlySpecificTribe:
                    amountOfOppSpecificTribe = 0;
                    foreach (PhysicalCard target in playerCards)
                    {
                        if (target.GetCard().GetTribe() == effect.actionTribe && target != cardGameObject.GetComponent<PhysicalCard>())
                            amountOfOppSpecificTribe++;
                    }

                    foreach (PhysicalCard target in targets)
                    {
                        StartCoroutine(target.DecreasePower(effect.val1 * amountOfOppSpecificTribe));
                    }
                    break;
                case Action.GainPowerForEachFriendlySpecificTribe:
                    amountOfOppSpecificTribe = 0;
                    foreach (PhysicalCard target in playerCards)
                    {
                        if (target.GetCard().GetTribe() == effect.actionTribe && target != cardGameObject.GetComponent<PhysicalCard>())
                            amountOfOppSpecificTribe++;
                    }

                    foreach (PhysicalCard target in targets)
                    {
                        StartCoroutine(target.DecreasePower(-effect.val1 * amountOfOppSpecificTribe));
                    }
                    break;
                case Action.IfYouControlXGainPower:
                    bool controlTribe = false;
                    foreach (PhysicalCard target in playerCards)
                    {
                        if (target.GetCard().GetTribe() == effect.actionTribe)
                            controlTribe = true;
                    }
                    if (controlTribe)
                        foreach (PhysicalCard target in targets)
                        {
                            StartCoroutine(target.DecreasePower(-effect.val1));
                        }
                    break;
                case Action.IfOppControlXGainPower:
                    controlTribe = false;
                    foreach (PhysicalCard target in enemyCards)
                    {
                        if (target.GetCard().GetTribe() == effect.actionTribe)
                            controlTribe = true;
                    }
                    if (controlTribe)
                        foreach (PhysicalCard target in targets)
                        {
                            StartCoroutine(target.DecreasePower(-effect.val1));
                        }
                    break;
            }
        }

        if (targets.Count == 0)
        {
            animationsStarted = 0;
            animationsDone = 0;
            EffectAnimationDone();
        }
    }

    public void EffectAnimationDone()
    {
        animationsDone++;

        if (animationsDone >= animationsStarted)
        {
            StartCoroutine(CalculatePoints());
            if (givePowerToNextCard != 0 && turnHandler.IsEnemyTurn() && !justPlayedEffectForNextTurn)
            {
                givePowerToNextCard = 0;
            }
            else if (doubleNextCardsPower && turnHandler.IsEnemyTurn() && !justPlayedEffectForNextTurn)
            {
                doubleNextCardsPower = false;
            }
            else
            {
                if (!turnHandler.IsEnemyTurn())
                    hand.DiscardHand();
                else
                    turnHandler.TurnEnd();
            }
            justPlayedEffectForNextTurn = false;
        }
    }

    public void EntranceAnimationDone()
    {
        cardJustEntered = true;
    }
    
    public IEnumerator EffectHappens(WhenEffectHappens whenEffectHappens, GameObject cardGameObject, GameObject cardThatTriggeredEffect = null)
    {
        yield return new WaitUntil(() => cardJustEntered);
        cardJustEntered = false;
        PhysicalCard cardTriggerEff = null;
        if (cardThatTriggeredEffect)
            cardTriggerEff = cardThatTriggeredEffect.GetComponent<PhysicalCard>();
        PhysicalCard physCard = cardGameObject.GetComponent<PhysicalCard>();
        Card card = physCard.GetCard();
        animationsStarted = 0;
        animationsDone = 0;

        if (givePowerToNextCard != 0 && turnHandler.IsEnemyTurn())
        {
            Effect[] effect = { new Effect() };
            effect[0].when = WhenEffectHappens.OnPlay;
            effect[0].target = Target.Itself;
            effect[0].action = Action.DecreasePower;
            effect[0].val1 = -givePowerToNextCard;
            DoEffect(effect, cardGameObject, cardGameObject);
            yield return new WaitUntil(() => givePowerToNextCard == 0);
        }

        if (doubleNextCardsPower && turnHandler.IsEnemyTurn())
        {
            Effect[] effect = { new Effect() };
            effect[0].when = WhenEffectHappens.OnPlay;
            effect[0].target = Target.Itself;
            effect[0].action = Action.DoublePower;
            DoEffect(effect, cardGameObject, cardGameObject);
            yield return new WaitUntil(() => !doubleNextCardsPower);
        }

        for (int i = 0; i < card.effects.Length; i++)
            if (card.effects[i].when == whenEffectHappens)
            {
                if (cardTriggerEff)
                {
                    if (card.effects[i].whenTribe == Tribe.None || cardTriggerEff.GetCard().tribe == card.effects[i].whenTribe)
                        DoEffect(card.GetEffects(), cardGameObject, cardThatTriggeredEffect);
                }
                else
                    DoEffect(card.GetEffects(), cardGameObject, cardThatTriggeredEffect);
            }

        if (card.GetEffects().Length == 0)
        {
            animationsStarted = 0;
            animationsDone = 0;
            EffectAnimationDone();
        }
    }

    /*public void SearchForEffectInDropZones(bool isPlayer, WhenEffectHappens whenEffect, GameObject cardThatTriggeredEffect = null)
    {
        DropZone[] dropZones;
        if (isPlayer)
            dropZones = playerDropZones;
        else
            dropZones = enemyDropZones;

        foreach (DropZone dropZone in dropZones)
        {
            PhysicalCard card = dropZone.GetComponentInChildren<PhysicalCard>();
            if (card)
            {
                EffectHappens(whenEffect, card.gameObject, cardThatTriggeredEffect);
            }
        }
    }*/
}
