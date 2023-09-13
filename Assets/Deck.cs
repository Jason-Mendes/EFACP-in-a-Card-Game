using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public Transform deckPlacement;
    public GameObject emptyCardPrefab;
    public GameObject physicalCardPrefab;
    public Hand hand;
    public List<Card> cardsInDeck;
    public List<GameObject> physicalDeck;

    int cardsInTotal = 0;

    private void Start()
    {
        //ShuffleCards();
        CreatePhysicalDeck();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            DrawCard();
        }
    }

    void ShuffleCards()
    {
        List<Card> initialCards = new List<Card>(cardsInDeck);
        int numOfCardsInDeck = cardsInDeck.Count;
        cardsInDeck = new List<Card>();

        for (int i = 0; i < numOfCardsInDeck; i++)
        {
            int rndNum = Random.Range(0, initialCards.Count);
            cardsInDeck.Add(initialCards[rndNum]);
            initialCards.RemoveAt(rndNum);
        }
    }

    void CreatePhysicalDeck()
    {
        for (int i = 0; i < 30; i++)
        {
            GameObject instantiatedCard = Instantiate(emptyCardPrefab, deckPlacement);
            //physicalDeck.Add(instantiatedCard);
            instantiatedCard.transform.localRotation = Quaternion.Euler(0, 180f, 0);
            instantiatedCard.transform.localPosition = new Vector3(0, 0, i *0.1f);
            cardsInTotal++;
            //instantiatedCard.GetComponentInChildren<PhysicalCard>().SetCard(cardsInDeck[i]);
        }
    }

    public GameObject CreateCardOnTop(Card newCard, bool deduction= false, bool recall=false)
    {
        GameObject instantiatedCard = Instantiate(physicalCardPrefab, deckPlacement);
        physicalDeck.Add(instantiatedCard);
        instantiatedCard.transform.localRotation = Quaternion.Euler(0, 180f, 0);
        instantiatedCard.transform.localPosition = new Vector3(0, 0, cardsInTotal * 0.1f);
        PhysicalCard physCard = instantiatedCard.GetComponentInChildren<PhysicalCard>();
        physCard.SetCardForTheFuture(newCard);
        physCard.IsDeduction = deduction;
        physCard.IsRecall = recall;
        cardsInTotal++;
        return instantiatedCard;
    }

    public void DrawCard()
    {
        GameObject topPhysicalCard = physicalDeck[0];
        hand.AddToHand(topPhysicalCard);

        //Card topCard = cardsInDeck[0];
        //cardsInDeck.Remove(topCard);
        physicalDeck.Remove(topPhysicalCard);

    }

}
