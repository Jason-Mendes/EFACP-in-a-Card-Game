using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMouseInteraction : MonoBehaviour
{
    DropZone dropZone;
    public PhysicalCard physicalCard;
    bool dragging = false;

    Vector3 handPos;
    public float dragZPos;

    public bool isPlaced = false;
    public bool inHand = false;
    int handPlacement = 0;
    bool playable = false;

    private void Start()
    {
        dropZone = GameObject.FindObjectOfType<DropZone>();
    }

    private void Update()
    {
        if (dragging)
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 normalizedMousePos = new Vector2(mousePos.x / Screen.width, mousePos.y / Screen.height);
            float x = Mathf.Lerp(-100f, 100f, normalizedMousePos.x);
            float y = Mathf.Lerp(-50f, 50f, normalizedMousePos.y);

            transform.parent.position = new Vector3(x, y , dragZPos);

            /*if (Input.GetMouseButtonUp(0) && normalizedMousePos.y < .4f)
            {
                DoneDragging();
            }*/
        }
    }

    private void OnMouseDown()
    {
        if (inHand && playable)
        {
            dragging = true;
            handPos = transform.parent.position;
            dropZone.ShowPlayabilityEffect(true);
            dropZone.StartDragCard();
        }
    }

    private void OnMouseDrag()
    {
        //dragging = true;
        //physicalCard.Drag(true);
    }

    private void OnMouseUp()
    {

        dropZone.ShowPlayabilityEffect(false);
        if (inHand && playable && dragging)
        {
            dragging = false;
            dropZone.StopDragCard();
            bool dropped = dropZone.DroppedCard(gameObject);
            if (!dropped)
                transform.parent.position = handPos;
        }
        //physicalCard.Drag(false);
    }

    public void UpdateHandPlacement(int placement)
    {
        handPlacement = placement;
    }

    public int GetHandPlacement()
    {
        return handPlacement;
    }

    public bool CurBeingDragged()
    {
        return dragging;
    }

    public bool IsPlaced()
    {
        return isPlaced;
    }
    public bool InHand()
    {
        return inHand;
    }
    public void PutInHand()
    {
        inHand = true;
        isPlaced = false;
    }
    public void PutInGraveyard()
    {
        inHand = false;
        isPlaced = false;
    }
    public void PlaceCard()
    {
        isPlaced = true;
        inHand = false;
    }

    public void SetPlayability(bool playability)
    {
        playable = playability;
    }
}
