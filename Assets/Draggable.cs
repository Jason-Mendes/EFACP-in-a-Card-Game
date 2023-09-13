using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour//, IBeginDragHandler, IDragHandler, IEndDragHandler
{
  /*  private bool curDragging = false;

    public Transform parentToReturnTo = null;

    public bool isPlaced = false;
    
    public bool inHand = true;

    //DraggingHandler draggingHandler;

    private void Start()
    {
        //draggingHandler = GameObject.FindObjectOfType<DraggingHandler>();
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (!isPlaced)
        {
            curDragging = true;

            parentToReturnTo = this.transform.parent;

            GetComponent<CanvasGroup>().blocksRaycasts = false;

            //Dumt og midlertidigt
            if (this.transform.parent.parent.GetComponent<Canvas>() != null)
                this.transform.SetParent(this.transform.parent.parent);
            else
                this.transform.SetParent(this.transform.parent.parent.parent);
            //--
            
           /* if (inHand)
                draggingHandler.MoveFromHand();
            if (isPlaced)
                draggingHandler.MoveFromBattlefield();

        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isPlaced)
            this.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
            curDragging = false;
            this.transform.SetParent(parentToReturnTo);
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        /*draggingHandler.StopMoving();
        draggingHandler.CheckForTriples();
    }

    //-----

    public bool CurBeingDragged()
    {
        return curDragging;
    }

    public bool IsPlaced()
    {
        return isPlaced;
    }
    public bool InHand()
    {
        return inHand;
    }

    public void PlaceCard()
    {
        isPlaced = true;
        inHand = false;
    }*/
}
