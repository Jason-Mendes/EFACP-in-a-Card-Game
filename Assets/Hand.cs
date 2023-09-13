using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public CardSelectionLogger cardSelectionLogger;
    // Defined events for card selection start and stop
    public event EventHandler OnCardSelectionStart;
    public event EventHandler OnCardSelectionStop;

    public Transform handTransform;
    public Vector3 initialPos;
    public Quaternion initialRotation;
    float timer = 0;
    public float addToHandTime = 5f;

    public List<GameObject> physicalHand;
    public List<Card> hand;

    int numOfCards = 0;
    public float width = 5f;
    public float handCardScale = .8f;

    int numOfCardsNextToDraw = 0;
    List<GameObject> cardsNextToDraw = new List<GameObject>();

    public float hoverPower = .5f;
    public float scalePower = 1.2f;
    public float movePower = 0.2f;

    Vector3 originScale;

    GameObject draggedObject;
    public float dragSpeed = 5f;
    bool dragging;

    public GameObject cardPrefab;

    Deck deck;
    DiscardPile discardPile;
    TurnHandler turnHandler;
    //public TurnSystem turnSystem;

    private void Start()
    {
        cardSelectionLogger = FindObjectOfType<CardSelectionLogger>();
        deck = GameObject.FindObjectOfType<Deck>();
        discardPile = GameObject.FindObjectOfType<DiscardPile>();
        turnHandler = GameObject.FindObjectOfType<TurnHandler>();
        originScale = cardPrefab.transform.localScale;
    }

    public void AddToHand(GameObject physicalCard)
    {
        numOfCardsNextToDraw++;
        numOfCards++;

        cardsNextToDraw.Add(physicalCard);
        if (numOfCardsNextToDraw == 1)
            StartDrawingNextCard();
    }

    bool cardVisible = false;

    private void Update()
    {
        if (numOfCardsNextToDraw > 0)
        {
            int cardPlacement = numOfCards - (numOfCardsNextToDraw - 1);
            cardsNextToDraw[0].transform.position = Vector3.Lerp(initialPos, CardPos(cardPlacement), timer / addToHandTime);
            cardsNextToDraw[0].transform.rotation = Quaternion.Lerp(initialRotation, handTransform.rotation, timer / addToHandTime);

            timer += Time.deltaTime;
            if (timer >= addToHandTime)
            {
                cardsNextToDraw[0].transform.position = CardPos(cardPlacement);
                cardsNextToDraw[0].transform.rotation = handTransform.rotation;
                cardsNextToDraw[0].transform.localScale = originScale;

                /*if (turnSystem.IsPlayerTurn())
                    cardsNextToDraw[0].GetComponent<PhysicalCard>().highlightObject.SetActive(true);*/
                    
                // Invoke OnCardSelectionStart event
                OnCardSelectionStart?.Invoke(this, EventArgs.Empty);
                numOfCardsNextToDraw--;

                cardsNextToDraw[0].GetComponentInChildren<CardMouseInteraction>().PutInHand();

                PhysicalCard physCard = cardsNextToDraw[0].GetComponentInChildren<PhysicalCard>();
                cardSelectionLogger.CardAddedToHand(physCard.GetCard(), cardPlacement, physCard.IsRecall, physCard.IsDeduction);

                cardsNextToDraw.RemoveAt(0);


                if (numOfCardsNextToDraw > 0)
                {
                    StartDrawingNextCard();
                }
                else
                {
                    //cardSelectionLogger.SelectionStart();
                    SetCardsPlayability(true);
                    turnHandler.SetHighlightActive(true);
                }

                timer = 0;
            }
            else if (timer/addToHandTime > 0.5 && !cardVisible)
            {
                cardsNextToDraw[0].GetComponentInChildren<PhysicalCard>().SetCard();
                cardVisible = true;
            }
        }
    }

    void StartDrawingNextCard()
    {
        cardVisible = false;
        initialPos = cardsNextToDraw[0].transform.position;
        initialRotation = cardsNextToDraw[0].transform.rotation;

        cardsNextToDraw[0].transform.parent = handTransform;
        cardsNextToDraw[0].transform.localScale = new Vector3(cardsNextToDraw[0].transform.localScale.x * handCardScale, cardsNextToDraw[0].transform.localScale.y * handCardScale, cardsNextToDraw[0].transform.localScale.z * handCardScale);

        physicalHand.Add(cardsNextToDraw[0]);
        hand.Add(cardsNextToDraw[0].GetComponentInChildren<PhysicalCard>().card);
        cardsNextToDraw[0].GetComponentInChildren<CardMouseInteraction>().UpdateHandPlacement(numOfCards - numOfCardsNextToDraw + 1);

        for (int i = 0; i < numOfCards - (numOfCardsNextToDraw - 1) - 1; i++)
        {
            physicalHand[i].transform.position = CardPos(i + 1);
        }
    }

    public void Dragging(int placement)
    {
        print("DRAG");
        draggedObject = physicalHand[placement - 1];
        dragging = true;

        /*for (int i = 0; i < physicalHand.Count; i++)
            physicalHand[i].GetComponent<PhysicalCard>().highlightObject.SetActive(false);*/
    }

    public void DoneDragging()
    {
        dragging = false;
        draggedObject.SetActive(true);

        /*for (int i = 0; i < physicalHand.Count; i++)
            physicalHand[i].GetComponent<PhysicalCard>().highlightObject.SetActive(true);*/
            
        draggedObject = null;
    }

    public void PlacedOnBattlefield(int placement)
    {
        SetCardsPlayability(false);
        draggedObject = physicalHand[placement - 1];
        dragging = false;
        
        //draggedObject.GetComponent<PhysicalCard>().highlightObject.SetActive(false);
        physicalHand.Remove(draggedObject);
        numOfCards--;
        UpdateCardsInHand();
        
        // Invoke OnCardSelectionStop event
        OnCardSelectionStop?.Invoke(this, EventArgs.Empty);

        /*for (int i = 0; i < physicalHand.Count; i++)
            physicalHand[i].GetComponent<PhysicalCard>().highlightObject.SetActive(true);*/
            
        draggedObject = null;

        //DiscardHand();
    }

    /*
    public void CardEffectIsDone()
    {
        cardEffectIsDone = true;
    }

    bool cardEffectIsDone = false;*/

    public void DiscardHand()
    {
        //yield return new WaitUntil(() => cardEffectIsDone);
        int cardAmount = physicalHand.Count;
        for (int i = 0; i < cardAmount; i++)
        {
            discardPile.AddToDiscardPile(physicalHand[i]);
            UpdateCardsInHand();
        }

        numOfCards = 0;
        physicalHand.Clear();
        hand.Clear();
        //cardEffectIsDone = false;
    }

    public void UpdateCardsInHand()
    {
        for (int i = 0; i < physicalHand.Count; i++)
        {
            physicalHand[i].GetComponentInChildren<CardMouseInteraction>().UpdateHandPlacement(i + 1);
        }
    }

    Vector3 CardPos(int cardPlacement)
    {
        Vector3 a = new Vector3(-((numOfCards - 1) * width) / 2 + (cardPlacement - 1) * width, 0f, 0f);
        return a + handTransform.position;
    }

    public void DrawNewHand(Card[] newHand, bool[] deduction, bool[] recall)
    {
        for (int i = 0; i < newHand.Length; i++)
        {
            deck.CreateCardOnTop(newHand[i], deduction[i],recall[i]);
            deck.DrawCard();
        }
    }

    void SetCardsPlayability(bool playability)
    {
        for (int i = 0; i < physicalHand.Count; i++)
        {
            physicalHand[i].GetComponentInChildren<CardMouseInteraction>().SetPlayability(playability);
            if (!playability)
                physicalHand[i].GetComponentInChildren<PhysicalCard>().HighlightCard(false);
        }

    }

    public PhysicalCard GetPhysicalCard(int cardPos)
    {
        return physicalHand[cardPos].GetComponentInChildren<PhysicalCard>();
    }
}
