using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnLogger : MonoBehaviour
{
    private LoggingManager loggingManager;
    private float turnStartTime;

    void Start()
    {
        loggingManager = GameObject.Find("Logging").GetComponent<LoggingManager>();
    }

    public void StartTurn()
    {
        turnStartTime = Time.time;
    }

    public void EndTurn()
    {
        float turnEndTime = Time.time;
        float turnTime = turnEndTime - turnStartTime;

        loggingManager.Log("TurnLog", "TurnTime", turnTime);
        loggingManager.SaveAllLogs();
    }
}

/*{
    private LoggingManager loggingManager;
    private TurnHandler turnHandler;
    private float startTime;
    private int currentTurn;
    private List<Card> chosenCards;

    // Start is called before the first frame update
    void Start()
    {
        // Find the logging Manager in the scene.
        loggingManager = GameObject.Find("Logging").GetComponent<LoggingManager>();

        // Find the turn handler in the scene.
        turnHandler = FindObjectOfType<TurnHandler>();

        // Initialize the chosen cards list.
        chosenCards = new List<Card>();

        // Register to listen for the end of each turn.
        turnHandler.OnTurnEnd += LogTurn;
    }

    // Update is called once per frame
    void Update()
    {
        // If it's the player's turn, update the time spent.
        if (!turnHandler.IsEnemyTurn())
        {
            float elapsedTime = Time.time - startTime;
            loggingManager.UpdateData("Turn Time", currentTurn.ToString(), elapsedTime);
        }
    }

    void LogTurn()
    {
        // Log the time spent on the previous turn.
        float elapsedTime = Time.time - startTime;
        loggingManager.Log("Turn Time", currentTurn.ToString(), elapsedTime);

        // Log the cards chosen on the previous turn.
        loggingManager.Log("Chosen Cards", currentTurn.ToString(), chosenCards);

        // Reset the start time and chosen cards list for the next turn.
        startTime = Time.time;
        currentTurn++;

        chosenCards.Clear();
    }

    public void CardChosen(Card chosenCard)
    {
        // If it's the player's turn, add the chosen card to the list.
        if (!turnHandler.IsEnemyTurn())
        {
            chosenCards.Add(chosenCard);
        }
    }
}
*/