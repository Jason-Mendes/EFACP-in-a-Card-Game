using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class PhysicalCard : MonoBehaviour
{
    public TMP_Text powerText;
    public TMP_Text descriptionText;
    public TMP_Text tribeText;

    public Image artworkImage;
    public Image template;
    public Sprite[] templates;

    public Card card;
    private int power;

    string description;
    string tribe;

    EffectManager effectManager;

    bool readyForAction = false;

    public TMP_Text changeInPowerPopup;
    string changeInPowerText;

    bool isDeduction = false;
    bool isRecall = false;

    Card cardToTurnInto;
    public GameObject cylinder;

    public GameObject highlight;

    public void SetCardForTheFuture(Card newCard)
    {
        cardToTurnInto = newCard;
    }
    
    private void Start()
    {
        effectManager = GameObject.FindObjectOfType<EffectManager>();
        //SetCard(card);
    }

    public IEnumerator DecreasePower(int amount)
    {
        if (amount == 0)
        {
            EffectAnimationDone();
            readyForAction = false;
        }
        else
        {
            if (amount > 0)
                changeInPowerText = "-" + amount.ToString();
            else
                changeInPowerText = "+" + (-amount).ToString();
            changeInPowerPopup.text = changeInPowerText;

            yield return new WaitUntil(() => readyForAction);
            power -= amount;
            UpdateCard();

            readyForAction = false;
        }
    }

    public IEnumerator DestroyThis()
    {
        yield return new WaitUntil(() => readyForAction);
        Destroy(transform.parent.gameObject);
    }

    public int GetPower()
    {
        return power;
    }

    public void SetCard()
    {
        SetCard(cardToTurnInto, cardToTurnInto.power);
    }

    public void SetCard(Card newCard)
    {
        SetCard(newCard, newCard.power);
    }

    public void SetCard(Card newCard, int power)
    {
        card = newCard;
        ShowNewCard(power);
        cylinder.SetActive(true);
    }

    public void HighlightCard(bool hasHighlight)
    {
        highlight.SetActive(hasHighlight);
    }

    public Card GetCard()
    {
        return card;
    }

    private void ShowNewCard(int newPower)
    {
      /*  if (card.cardType == Card.CardType.Minion)
        {*/
            description = card.description;
            power = newPower;
        tribe = card.tribe.ToString();

            UpdateCard();
       /* }
        else
        {
            attackObject.SetActive(false);
            defenseObject.SetActive(false);
        }*/

        artworkImage.sprite = card.artwork;
        template.sprite = templates[(int)card.GetTribe()];
    

    }

    void UpdateCard()
    {
        print("Updates card");
        /*if (card.cardType == Card.CardType.Minion)
        {*/
        powerText.text = power.ToString();
        descriptionText.text = description.ToString();
        tribeText.text = tribe;

        /*}
        else
        {
            attackObject.SetActive(false);
            defenseObject.SetActive(false);
        }*/
    }

    public void DoAction()
    {
        readyForAction = true;
    }

    public void EntranceAnimationDone()
    {
        GetComponent<Animator>().SetInteger("Entrance", 0);
        effectManager.EntranceAnimationDone();
    }

    public void EffectAnimationDone()
    {
        GetComponent<Animator>().SetInteger("Effect", 0);
        effectManager.EffectAnimationDone();
    }

    public bool IsDeduction
    {
        get { return isDeduction; }
        set { isDeduction = value; }
    }
    public bool IsRecall
    {
        get { return isRecall; }
        set { isRecall = value; }
    }
}
