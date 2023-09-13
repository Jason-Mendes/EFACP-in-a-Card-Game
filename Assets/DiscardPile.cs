using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardPile : MonoBehaviour
{
    public Transform discardPileTrans;
    int numOfCardsNextToDiscard = 0;
    int cardsInDiscardPile =0;
    List<GameObject> cardsNextToDiscard = new List<GameObject>();
    float timer;
    public float addToPileTime;
    Vector3 initialPos;
    Quaternion initialRotation;
    Vector3 targetPos;

    GameObject topCard;

    public GameObject emptyCardPrefab;

    EffectManager effectManager;
    TurnHandler turnHandler;

    // Start is called before the first frame update
    void Start()
    {
        effectManager = GameObject.FindObjectOfType<EffectManager>();
        turnHandler = GameObject.FindObjectOfType<TurnHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (numOfCardsNextToDiscard > 0)
        {
            targetPos = discardPileTrans.position + new Vector3(0, 0, cardsInDiscardPile * -.1f);
            cardsNextToDiscard[0].transform.position = Vector3.Lerp(initialPos, targetPos, timer / addToPileTime);
            cardsNextToDiscard[0].transform.rotation = Quaternion.Lerp(initialRotation, discardPileTrans.rotation, timer / addToPileTime);

            timer += Time.deltaTime;
            if (timer >= addToPileTime)
            {
                cardsNextToDiscard[0].transform.position = targetPos;
                cardsNextToDiscard[0].transform.rotation = discardPileTrans.rotation;
                cardsNextToDiscard[0].GetComponentInChildren<CardMouseInteraction>().PutInGraveyard();

                if (cardsInDiscardPile > 0)
                {
                    var emptyPrefab = Instantiate(emptyCardPrefab, transform);
                    emptyPrefab.transform.position = topCard.transform.position;
                    Destroy(topCard);
                }

                cardsInDiscardPile++;

                topCard = cardsNextToDiscard[0];

                cardsNextToDiscard.RemoveAt(0);
                numOfCardsNextToDiscard--;

                if (numOfCardsNextToDiscard > 0)
                    StartDiscardingNextCard();
                else
                    DoneDiscarding();

                timer = 0;
            }
        }
    }

    void DoneDiscarding()
    {
        turnHandler.DoneDiscarding();
    }

    void StartDiscardingNextCard()
    {
        initialPos = cardsNextToDiscard[0].transform.position;
        initialRotation = cardsNextToDiscard[0].transform.rotation;

        cardsNextToDiscard[0].transform.parent = discardPileTrans;
    }

    public void AddToDiscardPile(GameObject physicalCard)
    {
        numOfCardsNextToDiscard++;

        cardsNextToDiscard.Add(physicalCard);
        if (numOfCardsNextToDiscard == 1)
            StartDiscardingNextCard();
    }

}
