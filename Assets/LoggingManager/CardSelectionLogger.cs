using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelectionLogger : MonoBehaviour
{
    public event EventHandler OnSelectionStart;
    public event EventHandler OnSelectionStop;

    private DropZone dropZone;
    private TurnHandler turnHandler;
    public EffectManager effectManager;
    public LoggingManager loggingManager;
    private Hand hand;

    private int turnCounter;
    private int roundCounter;
    private string player = "player1";
    private int selectionStart;
    private int selectionStop;
    private float selectionTime;
    private bool isSelectionTime;
    bool playerCardPlayed = false;
   

    private void Start()
    {
        turnHandler = FindObjectOfType<TurnHandler>();
        player = turnHandler.GetPlayerName();
        dropZone = FindObjectOfType<DropZone>();
        hand = FindObjectOfType<Hand>();

        turnCounter = 1;
        roundCounter = 1;

        //dropZone.OnCardDropped += DropZone_OnCardDropped;

        //hand.OnCardSelectionStart += CardSelectionLogger_OnSelectionStart;
        //hand.OnCardSelectionStop += CardSelectionLogger_OnSelectionStop;
        
    }

    private void Update()
    {
        if (isSelectionTime)
        {
            selectionTime += Time.deltaTime;
        }
        turnCounter = turnHandler.GetCurTurn();
        roundCounter = turnHandler.Getround();
    }

    /*public void SelectionStart()
    {
        OnSelectionStart?.Invoke(this, EventArgs.Empty);
    }

    public void SelectionStop()
    {
        OnSelectionStop?.Invoke(this, EventArgs.Empty);
    }

    private void CardSelectionLogger_OnSelectionStart(object sender, EventArgs e)
    {
        selectionStart = 1;
        isSelectionTime = true;
        selectionTime = 0;
        LogPlayerEvent("SelectionStart", null);
    }

    private void CardSelectionLogger_OnSelectionStop(object sender, EventArgs e)
    {
        selectionStop = 1;
        selectionTime = 0;

        isSelectionTime = false;
        LogPlayerEvent("SelectionStop", null);
    }

    private void DropZone_OnCardDropped(object sender, EventArgs e)
    {
        LogPlayerEvent("CardDropped", null);
    }*/

        public void TutorialOver()
    {
        LogPlayerEvent("TUTORIAL_OVER");
    }

    public void ObjectiveEnd()
    {
        LogPlayerEvent("OBJECTIVE_END");
    }

    public void RecallPointsCounted()
    {
        LogPlayerEvent("RECALL_POINTS_COUNTED");
    }

    public void NewMission()
    {
        playerPoints = 0;
        enemyPoints = 0;
        LogPlayerEvent("NEW_MISSION");
    }
    public void NewDay()
    {
        LogPlayerEvent("NEW_DAY");
    }
    public void NewObjective()
    {
        LogPlayerEvent("NEW_OBJECTIVE");
    }
    string cardName = "";
    string cardPlacement = "";
    string cardHandRecall = "";
    string cardHandDeduction = "";
    public void CardAddedToHand(Card card, int placement, bool isRecall, bool isDeduction)
    {
        cardName = card.name;
        cardPlacement = placement.ToString();
        cardHandDeduction = isDeduction.ToString();
        cardHandRecall = isRecall.ToString();
        LogPlayerEvent("DRAW_CARD");
    }
    public IEnumerator EnemyCardPlayed(Card card)
    {
        yield return new WaitForEndOfFrame();
        cardName = card.name;
        LogPlayerEvent("ENEMY_PLAYED_CARD");
    }

    string choseRecall = "";
    string choseDeduction = "";
    public void OptionChosen(Card card,int placement, bool isRecall, bool isDeduction, bool choseRecall, bool choseDeduction)
    {
        //yield return new WaitUntil(()=> playerCardPlayed);
        cardName = card.name;
        cardPlacement = placement.ToString();
        cardHandDeduction = isDeduction.ToString();
        cardHandRecall = isRecall.ToString();
        this.choseRecall = choseRecall.ToString();
        this.choseDeduction = choseDeduction.ToString();
    }

    public void HighlightCard(Card card, int placement,  bool isRecall, bool isDeduction)
    {
        cardName = card.name;
        cardPlacement = placement.ToString();
        cardHandDeduction = isDeduction.ToString();
        cardHandRecall = isRecall.ToString();
        LogPlayerEvent("HIGHLIGHT_CARD");
    }

    public void PlayerCardPlayed()
    {
        LogPlayerEvent("PLAYER_PLAYED_CARD");
    }

    int playerPoints = 0;
    int enemyPoints = 0;
    private void LogPlayerEvent(string eventName, Card cardData = null)
    {
        int diffPlayerPoints = Mathf.Abs(effectManager.GetPoints(false) - playerPoints);
        int diffEnemyPoints = Mathf.Abs(effectManager.GetPoints(true) - enemyPoints);

        int totalDiff = diffPlayerPoints + diffEnemyPoints;

        playerPoints = effectManager.GetPoints(false);
        enemyPoints = effectManager.GetPoints(true);

        Dictionary<string, object> eventData = new Dictionary<string, object>() {
            {"EventName", eventName},
            {"PlayerName", player},
            {"TestScenario", turnHandler.testScenario },
            { "Day", turnHandler.GetCurTurn()},
            {"Mission", turnHandler.journeyIndexChosen},
            {"Objective", turnHandler.Getround()},
            {"HighlightTest", turnHandler.highlightOn },
            {"Journey", GetJourney() },
            {"PointTotalPlayer", playerPoints },
            {"PointTotalEnemy", enemyPoints},
            {"DiffPlayerPoints", diffPlayerPoints },
            {"DiffEnemyPoints", diffEnemyPoints},
            {"TotalDiff", totalDiff},
            {"CardName", cardName},
            {"CardPlacement", cardPlacement },
            {"CardRecall", cardHandRecall},
            {"CardDeduction", cardHandDeduction},
            {"ChoseDeduction", choseDeduction},
            {"ChoseRecall", choseRecall},
            {"RecallPoints", turnHandler.GetRecallPoints() },
            {"EnemyRecallPoints", turnHandler.GetEnemyRecallPoints() },
            {"LightPoints", turnHandler.GetLightPoints() },
        };

        /*if (cardData != null)
        {
            eventData["CardType"] = cardData.cardType;
        }*/

        loggingManager.Log("PlayerEvents", eventData);
        loggingManager.SaveLog("PlayerEvents");
        loggingManager.ClearLog("PlayerEvents");

        //CLEANUP
        cardName = "";
        cardPlacement = "";
        cardHandDeduction = "";
        cardHandRecall = "";
        choseRecall = "";
        choseDeduction = "";

        //selectionStart = 0;
        //selectionStop = 0;
    }

    int GetJourney()
    {
        int journeyIndex = turnHandler.journeyIndexChosen;

        if (journeyIndex == 0)
            return 0;

        TestScenario testScenario = turnHandler.testScenario;

        if (journeyIndex == 1)
        {
            if (testScenario == TestScenario.S2HighlightS1 || testScenario == TestScenario.S2S1Highlight)
                return 2;
            else return 1;
        }

        if (journeyIndex == 2)
        {
            if (testScenario == TestScenario.S2HighlightS1 || testScenario == TestScenario.S2S1Highlight)
                return 1;
            else return 2;
        }

        return -1;
    }
}




        
//     //     loggingManager.Log("CardSelectionDatastorage", new Dictionary<string, object> { { "CardSelectionTime", timeTaken } });

//     //     Dictionary<string, object> otherData = new Dictionary<string, object>() {
//     //         {"SoundVolume", soundvol},
//     //         {"PlayerName", player}
//     //     };

      
//     //     loggingManager.SaveLog("CardSelectionDatastorage");

       
//     //     loggingManager.ClearLog("CardSelectionDatastorage");
//     //     loggingManager.NewFilestamp();
//     // 
   


//     private void LogPlayerTurnData()
//     {
//         // loggingManager.Log("CardSelectionDatastorage", new Dictionary<string, object> { { "LogMessage", $"Round {roundCounter}, Time taken to select a card: {turnHandler.timeTakenToSelectCard}, Turn {turnCounter}:" } });
//         // // loggingManager.Log("CardSelectionDatastorage", new Dictionary<string, object> { { "LogMessage", $"Time taken to select a card: {turnHandler.timeTakenToSelectCard}" } });

//         Dictionary<string, object> otherData = new Dictionary<string, object>() {
//             // {"SoundVolume", soundvol},
//             {"PlayerName", player},
//             {"Round", roundCounter},
//             {"Turn", turnCounter},
//             // {"TimeTaken", turnHandler.timeTakenToSelectCard},
//             {"SelectionStart", selectionStart},
//             {"SelectionTime", selectionTime},
//             {"SelectionStop", selectionStop}
//         };

//         loggingManager.Log("CardSelectionDatastorage", otherData);
//         loggingManager.SaveLog("CardSelectionDatastorage");
//         loggingManager.ClearLog("CardSelectionDatastorage");
        
//     }
// }