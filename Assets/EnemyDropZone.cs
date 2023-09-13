using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDropZone : MonoBehaviour
{
    public int maxCardAmount = 3;

    int cardAmount = 0;
    public float width = 10;

    EffectManager effectManager;
    public Transform cardLayingTrans;
    Hand hand;

    private void Start()
    {
        effectManager = GameObject.FindObjectOfType<EffectManager>();
        hand = GameObject.FindObjectOfType<Hand>();
    }

    public void DestroyAllCards()
    {
        foreach (Transform child in cardLayingTrans)
        {
            Destroy(child.gameObject);
        }
        cardAmount = 0;
    }

    public void PlaceCard(GameObject droppedCard)
    {
        CardMouseInteraction d = droppedCard.GetComponentInChildren<CardMouseInteraction>();

        PhysicalCard physCard = d.GetComponent<PhysicalCard>();

        //d.parentToReturnTo = this.transform;
        d.PlaceCard();
        d.transform.parent.SetParent(cardLayingTrans);
        d.transform.parent.position = CardPos(cardAmount);
        d.GetComponentInChildren<Animator>().SetInteger("Entrance", (int)physCard.GetCard().entranceAnimation + 1);
        cardAmount++;
        StartCoroutine(effectManager.EffectHappens(WhenEffectHappens.OnPlay, d.gameObject));
    }

    Vector3 CardPos(int cardPlacement)
    {
        Vector3 a = new Vector3(-((cardAmount - 1) * width) / 2 + (cardPlacement - 1) * width, 0f, 0f);
        return a + cardLayingTrans.position;
    }
}
