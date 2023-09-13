using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour
{
    public event EventHandler OnCardDropped;
    public int maxCardAmount = 3;

    int cardAmount = 0;
    public float width = 10;

    EffectManager effectManager;
    public Transform cardLayingTrans;
    Hand hand;
    TurnHandler turnHandler;

    public GameObject playabilityEffect;

    public GameObject choiceUI;
    public bool choicesEnabled = false;
    public RectTransform deductionChoiceTrans;
    public RectTransform recallChoiceTrans;

    Tutorial tutorial;

    private void Start()
    {
        tutorial = GameObject.FindObjectOfType<Tutorial>();
        effectManager = GameObject.FindObjectOfType<EffectManager>();
        hand = GameObject.FindObjectOfType<Hand>();
        turnHandler = GameObject.FindObjectOfType<TurnHandler>();
    }

    public void DestroyAllCards()
    {
        foreach (Transform child in cardLayingTrans)
        {
            Destroy(child.gameObject);
        }
        cardAmount = 0;
    }

    public void StartDragCard()
    {
        if (choicesEnabled)
            choiceUI.SetActive(true);

        turnHandler.StartDrag();
    }
    public void StopDragCard()
    {
        if (choicesEnabled)
            choiceUI.SetActive(false);
    }

    public bool DroppedCard(GameObject droppedCard)
    {
        RectTransform dropTransform = transform as RectTransform;

        CardMouseInteraction cmi = droppedCard.GetComponentInChildren<CardMouseInteraction>();
        if (cmi != null)
        {
            if (!choicesEnabled)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(dropTransform, Input.mousePosition, Camera.main))
                {
                    if (cmi.InHand() && this.transform.childCount < maxCardAmount)
                    {
                        PlaceCard(cmi, false, false);

                        return true;
                    }
                }
            }
            else
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(deductionChoiceTrans, Input.mousePosition, Camera.main))
                {
                    if (cmi.InHand() && this.transform.childCount < maxCardAmount)
                    {
                        print("Participant think they chose deduction");
                        PlaceCard(cmi, false, true);

                        return true;
                    }
                }

                if (RectTransformUtility.RectangleContainsScreenPoint(recallChoiceTrans, Input.mousePosition, Camera.main))
                {
                    if (cmi.InHand() && this.transform.childCount < maxCardAmount)
                    {
                        print("Participant think they chose recall");
                        PlaceCard(cmi, true, false);

                        return true;
                    }
                }
            }
        }

        return false;
    }

    public void PlaceCard()
    {
        PlaceCard(cmi, choseRecall, choseDeduction);
    }
    bool choseRecall;
    bool choseDeduction;
    private CardMouseInteraction cmi;
    public void PlaceCard(CardMouseInteraction cmi, bool choseRecall, bool choseDeduction)
    {
        this.cmi = cmi;
        this.choseRecall = choseRecall;
        this.choseDeduction = choseDeduction;
        PhysicalCard physCard = cmi.GetComponent<PhysicalCard>();

        if (turnHandler.isTutorial)
            if (tutorial.CanTextAppear(WhenTextAppears.JustBeforeChoicePromptGoesAway))
        {
            choiceUI.SetActive(true);
            return;
        }
        //d.parentToReturnTo = this.transform;
        StartCoroutine(effectManager.EffectHappens(WhenEffectHappens.OnPlay, cmi.gameObject));
        cmi.PlaceCard();
        cmi.transform.parent.SetParent(cardLayingTrans);
        cmi.transform.parent.position = CardPos(cardAmount);
        cmi.GetComponentInChildren<Animator>().SetInteger("Entrance", (int)physCard.GetCard().entranceAnimation + 1);
        hand.PlacedOnBattlefield(cmi.GetHandPlacement());
        cardAmount++;
        turnHandler.OptionChosen(physCard.GetCard(), cmi.GetHandPlacement(), physCard.IsDeduction, physCard.IsRecall, choseDeduction, choseRecall);
        OnCardDropped?.Invoke(this, EventArgs.Empty);
    }

    public void ShowPlayabilityEffect(bool show)
    {
        playabilityEffect.SetActive(show);
    }

    Vector3 CardPos(int cardPlacement)
    {
        Vector3 a = new Vector3(-((cardAmount - 1) * width) / 2 + (cardPlacement - 1) * width, 0f, 0f);
        return a + cardLayingTrans.position;
    }
}