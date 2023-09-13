using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecallPoints : MonoBehaviour
{
    TurnHandler turnHandler;

    public bool isEnemy = false;

    // Start is called before the first frame update
    void Start()
    {
        turnHandler = GameObject.FindObjectOfType<TurnHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoneAddingToPoints()
    {
        turnHandler.AddingRecallPointsDone(isEnemy);
    }
}
