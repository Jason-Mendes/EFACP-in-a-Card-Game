using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum RecallMode
{
    RecallSameAsDeduction,
    RecallDifferentFromDeduction,
    RecallDeduction5050Split,
    RecallDeduction2080Split
}

public enum TestScenario
{
    S1HighlightS2,
    S1S2Highlight,
    S2HighlightS1,
    S2S1Highlight
}

public class TurnHandler : MonoBehaviour
{
    public string playerName;
    public TestScenario testScenario;
    public RecallMode recallMode;
    public CardSelectionLogger cardSelectionLogger;
    private float playerCardSelectionStartTime;
    public float timeTakenToSelectCard { get; private set; }
    int curTurn = 0;
    public int journeyIndexChosen = 0;
    public Journey[] journeys;
    private Turn[] turns;
    public Transform optionSpawnTrans;
    public GameObject optionPrefab;
    public TMP_Text recallText;
    public TMP_Text turnText;
    public TMP_Text recallPointsText;
    public TMP_Text enemyRecallPointsText;
    int recallPoints;
    List<int>[] turnToRecallEachTurn;
    int[] recallChoice;
    int[] deductionChoice;

    Hand hand;

    bool enemyTurn = false;

    EnemyDropZone enemyDropZone;
    DropZone playerDropZone;
    EffectManager effectManager;
    public GameObject agentUI;
    bool agentPopup = false;
    public float timeToReadMessage = 10f;
    float messageTimer = 0f;
    public Animator recallPointAnimator;
    public Animator recallPointAnimatorEnemy;
    int enemyRecallPoints;
    int round = 1;
    public int pointsPerRecall = 5;

    public GameObject missionEndUI;
    public TMP_Text missionEndText;
    bool missionEndScoresDone = false;

    public Animator playerPointsAnimator;
    public Animator enemyPointsAnimator;
    public Animator playerLightsAnimator;

    public float timeToReadMissionEndScreen = 5f;
    float missionEndScreenTimer = 0;
    bool missionEnded = false;

    public GameObject playerProgress;
    Image[] playerLights;
    int lightPoints = 0;
    bool gameEndScoresDone = false;
    public float gameEndScreenTime = 3f;
    float gameEndScreenTimer = 0;

    bool tutorialDone = false;

    public float timeBeforeHighliht = 5f;
    float highlightTimer = 0f;
    bool highlightTimerActive = false;

    public int roundsInTotal = 2;

    public bool isTutorial = true;
    Tutorial tutorial;

    public bool highlightOn = true;

    public GameObject recallOptionFrame;

    public Sprite normalLight;
    public Sprite wonLight;
    public Sprite LostLight;

    public GameObject intermissionPopUp;

    int highScore;
    public TMP_Text highscoreText;
    
    public int Getround()
    {
        return round;
    }
    
    public int GetCurTurn ()
    {
        return curTurn;
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    void SetupTestScenario()
    {
        Journey j1;
        Journey j2;
        switch (testScenario)
        {
            case TestScenario.S1HighlightS2:
                highlightOn = true;
                break;
            case TestScenario.S1S2Highlight:
                highlightOn = false;
                break;
            case TestScenario.S2HighlightS1:
                highlightOn = true;
                j1 = journeys[1];
                j2 = journeys[2];
                journeys[2] = j1;
                journeys[1] = j2;

                break;
            case TestScenario.S2S1Highlight:
                highlightOn = false;
                j1 = journeys[1];
                j2 = journeys[2];
                journeys[2] = j1;
                journeys[1] = j2;
                break;
        }
    }

    private void Start()
    {
        tutorial = GameObject.FindObjectOfType<Tutorial>();
        cardSelectionLogger = FindObjectOfType<CardSelectionLogger>();
        enemyDropZone = GameObject.FindObjectOfType<EnemyDropZone>();
        playerDropZone = GameObject.FindObjectOfType<DropZone>();
        effectManager = GameObject.FindObjectOfType<EffectManager>();
        hand = GameObject.FindObjectOfType<Hand>();
        turns = journeys[journeyIndexChosen].turns;
        SetupTestScenario();
        FindWhatToRecallEachTurn();
        enemyRecallPoints = journeys[journeyIndexChosen].hiddenRecallPointsAmount[round - 1];

        cardSelectionLogger.NewObjective();
        //turnText.text = "Turn " + curTurn.ToString();
        TurnEnd();
        //NewTurn();

        playerLights = playerProgress.GetComponentsInChildren<Image>();
    }

    void HighlightCards()
    {
        print(curTurn-1);
        PhysicalCard physCard = hand.GetPhysicalCard(recallChoice[curTurn - 1] - 1);
        physCard.HighlightCard(true);
        cardSelectionLogger.HighlightCard(physCard.GetCard(), physCard.GetComponent<CardMouseInteraction>().GetHandPlacement(), physCard.IsRecall, physCard.IsDeduction);
        List<int> recallIndexes = new List<int>();

        for (int i = 0; i < 5; i++)
            recallIndexes.Add(i);

        recallIndexes.RemoveAt(recallChoice[curTurn - 1]-1);

        for(int i = 0; i < 2; i++)
            recallIndexes.RemoveAt(Random.Range(0, recallIndexes.Count));

        for (int i = 0; i < recallIndexes.Count; i++)
        {
            physCard = hand.GetPhysicalCard(recallIndexes[i]);
            physCard.HighlightCard(true);
            cardSelectionLogger.HighlightCard(physCard.GetCard(), physCard.GetComponent<CardMouseInteraction>().GetHandPlacement(),physCard.IsRecall, physCard.IsDeduction);
        }
    }

    public int GetLightPoints()
    {
        return lightPoints;
    }

    private void Update()
    {
        if (highlightTimerActive)
        {
            highlightTimer += Time.deltaTime;

            if (highlightTimer >= timeBeforeHighliht)
            {
                highlightTimerActive = false;
                print("Highlight");
                if (highlightOn && !isTutorial)
                    HighlightCards();
                highlightTimer = 0;
            }
        }

        if (Input.GetMouseButtonDown(0))
            if (intermissionPopUp.activeSelf)
            {
                //IntermissionOver();
            }

        /*if (agentPopup)
        {
            messageTimer += Time.deltaTime;

            if (messageTimer >= timeToReadMessage * turnToRecallEachTurn[curTurn - 1].Count)
            {
                messageTimer = 0;
            }
        }*/
        if (gameEndScoresDone)
        {
            gameEndScreenTimer += Time.deltaTime;

            if (gameEndScreenTimer >= gameEndScreenTime)
            {
                gameEndScreenTimer = 0;
                gameEndScoresDone = false;
                intermissionPopUp.SetActive(true);
                highscoreText.text = "Final Score: " + highScore.ToString();
            }
        }

        if (missionEnded)
        {
            missionEndScreenTimer += Time.deltaTime;

            if (missionEndScreenTimer >= timeToReadMissionEndScreen)
            {
                if (!isTutorial)
                {
                    int playerPoints = effectManager.GetPoints(false);
                    int enemyPoints = effectManager.GetPoints(true);

                    if (playerPoints > enemyPoints)
                    {
                        playerLights[round - 1].sprite = wonLight;
                        lightPoints += 1;
                    }
                    else if (playerPoints < enemyPoints)
                    {
                        playerLights[round - 1].sprite = LostLight;
                        lightPoints -= 1;
                    }
                    else
                        playerLights[round - 1].color = Color.yellow;

                    missionEndUI.SetActive(false);
                    missionEndScreenTimer = 0;
                    DestroyAllCards();

                    effectManager.ResetExtraPoints();
                    playerPointsAnimator.SetBool("MissionEnd", false);
                    enemyPointsAnimator.SetBool("MissionEnd", false);

                    round++;
                    cardSelectionLogger.NewObjective();

                    if (round <= roundsInTotal)
                        TurnEnd();
                    else
                        GameEnd();
                }
                else
                {
                    cardSelectionLogger.TutorialOver();
                    MissionEnd();
                }
                missionEnded = false;
            }
        }
    }
    void IntermissionOver()
    {
        intermissionPopUp.SetActive(false);
        highlightOn = !highlightOn;

        MissionEnd();
    }

    void MissionEnd()
    {
        print("Mission Ended in tutorial");
        journeyIndexChosen++;
        turns = journeys[journeyIndexChosen].turns;
        round = 1;
        curTurn = 0;
        lightPoints = 0;
        isTutorial = false;
        turnText.text = "Day " + curTurn.ToString();
        playerLights[0].sprite = normalLight;
        playerLights[1].sprite = normalLight;
        playerLights[0].color = Color.white;
        playerLights[1].color = Color.white;
        enemyTurn = false;

        missionEndUI.SetActive(false);
        missionEndScreenTimer = 0;
        DestroyAllCards();

        effectManager.ResetExtraPoints();
        playerPointsAnimator.SetBool("MissionEnd", false);
        enemyPointsAnimator.SetBool("MissionEnd", false);
        playerLightsAnimator.SetBool("MissionEnd", false);


        FindWhatToRecallEachTurn();
        TurnEnd();
    }

    void FindWhatToRecallEachTurn()
    {
        cardSelectionLogger.NewMission();

        turnToRecallEachTurn = new List<int>[turns.Length];
        deductionChoice = new int[turns.Length];

        for (int i  =0; i < turnToRecallEachTurn.Length; i++)
            turnToRecallEachTurn[i] = new List<int>();

        for (int i= 0; i< turns.Length; i++)
        {


            int turnToRecallThis = (i+1) - turns[i].recallTurnsBefore;
            turnToRecallEachTurn[turnToRecallThis - 1].Add(i + 1);

            for (int j = 0; j < turns[i].options.Length; j++)
                if (turns[i].options[j].isCorrect)
                    deductionChoice[i] = j+1;
        }

            FindRecallChoices();

        foreach (List<int> turn in turnToRecallEachTurn)
            foreach (int whatToRecall in turn) {
                //print(whatToRecall);
            }

    }

    void FindRecallChoices()
    {
        recallChoice = new int[turns.Length];

        if (recallMode != RecallMode.RecallDeduction2080Split)
            for (int i = 0; i < recallChoice.Length; i++)
                FindRecallChoice(recallMode, i);
        else
        {
            List<int> orderedTurnList = new List<int>();
            for (int j = 0; j < turns.Length; j++)
            {
                if (j < 3 && isTutorial)
                    continue;
                orderedTurnList.Add(j + 1);
            }

            List<int> randomizedList = new List<int>();
            for (int j = 0; j < turns.Length; j++)
            {
                if (j < 3 && isTutorial)
                    continue;
                int randIndex = Random.Range(0, orderedTurnList.Count);
                randomizedList.Add(orderedTurnList[randIndex]);
                orderedTurnList.RemoveAt(randIndex);
            }

            bool[] different = new bool[randomizedList.Count];
            if (isTutorial)
                recallChoice[3] = 4;
            else
                for (int j = 0; j < different.Length; j++)
                    if (j % 7 == 0)
                        FindRecallChoice(RecallMode.RecallSameAsDeduction, randomizedList[j] - 1);
                    else
                        FindRecallChoice(RecallMode.RecallDifferentFromDeduction, randomizedList[j] - 1);                   
        }
    }

    void FindRecallChoice(RecallMode mode, int i)
    {
        switch (mode)
        {
            case RecallMode.RecallSameAsDeduction:
                recallChoice[i] = deductionChoice[i];
                print("Same");
                break;
            case RecallMode.RecallDifferentFromDeduction:
                List<int> nonDeductionOptions = new List<int>();
                for (int j = 0; j < turns[i].options.Length; j++)
                    if (j + 1 != deductionChoice[i])
                        nonDeductionOptions.Add(j + 1);

                int rndIndex = Random.Range(0, nonDeductionOptions.Count);
                recallChoice[i] = nonDeductionOptions[rndIndex];
                //print(recallChoice[i]);
                print("Different");

                break;
        }
    }
    
    void RecallToLaterTurn()
    {
        string recallString = "";
        for(int i = 0; i< turnToRecallEachTurn[curTurn - 1].Count; i++)
        {
            int turnToRecallTo = turnToRecallEachTurn[curTurn - 1][i];
            print(recallChoice[turnToRecallTo - 1]);
            recallString += "On Day " + turnToRecallTo + ", play the " + recallChoice[turnToRecallTo-1] +". choice.";

            if (i != turnToRecallEachTurn[curTurn - 1].Count - 1)
                recallString += "\n";
        }
        recallText.text = recallString;
    }

    void EndScreen()
    {
        recallText.text = "The End";
    }

    public void OptionChosen(Card card, int handPlacement, bool isDeductive, bool isRecall, bool choseDeduction, bool choseRecall)
    {
        cardSelectionLogger.OptionChosen(card, handPlacement, isRecall, isDeductive, choseRecall, choseDeduction);

        //cardSelectionLogger.SelectionStop();
        timeTakenToSelectCard = Time.time - playerCardSelectionStartTime;
        Debug.Log("Time taken to select a card: " + timeTakenToSelectCard);
        float timeTaken = Time.time - playerCardSelectionStartTime;
        Debug.Log("Time taken to select a card: " + timeTaken);
        print("Deductive: " + isDeductive);
        print("Recall: " + isRecall);

        if (isRecall)
        {
            recallPoints++;
            recallPointsText.text = recallPoints.ToString();
        }
        highlightTimer = 0;
        SetHighlightActive(false);
    }

    public bool IsEnemyTurn()
    {
        return enemyTurn;
    }

    public void TurnEnd()
    {
        if (isTutorial && enemyTurn)
        {
            if (tutorial.CanTextAppear(WhenTextAppears.AfterEnemyPlayedCard))
                return;
        }

        PhysicalCard[] playerBattlefieldCards = playerDropZone.GetComponentsInChildren<PhysicalCard>();

        if (enemyTurn)
            StartCoroutine(cardSelectionLogger.EnemyCardPlayed(turns[curTurn - 1].prompt));
        else if (playerBattlefieldCards.Length > 0)
            cardSelectionLogger.PlayerCardPlayed();


        if (isTutorial && playerBattlefieldCards.Length == 4 )
            RoundFinished();

        if (playerDropZone.maxCardAmount == playerBattlefieldCards.Length && !missionEnded)
            RoundFinished();
        /*else if (curTurn > turns.Length)
            EndScreen();*/
        else
        {
            enemyTurn = !enemyTurn;
            if (enemyTurn)
            {
                curTurn++;
                turnText.text = "Day " + curTurn.ToString();
                cardSelectionLogger.NewDay();
                NewOppTurn();
            }
            else
            {
                NewTurn();
            }
        }
    }

    void GameEnd()
    {
        missionEndUI.SetActive(true);
        missionEndText.gameObject.SetActive(false);
        playerLightsAnimator.SetBool("MissionEnd", true);
        StartCoroutine(GameEndTextPopup());
    }

    IEnumerator GameEndTextPopup()
    {
        yield return new WaitUntil(() => gameEndScoresDone);

        missionEndUI.SetActive(true);
        missionEndText.gameObject.SetActive(true);

        if (lightPoints > 0)
            missionEndText.text = "Mission Accomplished";
        else if (lightPoints < 0)
            missionEndText.text = "Mission Failed";
        else
            missionEndText.text = "Mission Inconclusive";
    }

    public int GetRecallPoints()
    {
        return recallPoints;
    }
    public int GetEnemyRecallPoints()
    {
        return enemyRecallPoints;
    }

    public void AddingRecallPointsDone(bool isEnemy)
    {
        if (!isEnemy)
        {
            recallPoints--;
            recallPointsText.text = recallPoints.ToString();
        }
        else
        {
            enemyRecallPoints--;
            enemyRecallPointsText.text = enemyRecallPoints.ToString();
        }
        effectManager.AddToPoints(pointsPerRecall, isEnemy);

        if (recallPoints <= 0)
            recallPointAnimator.SetBool("Count", false);

        if (enemyRecallPoints <= 0)
            recallPointAnimatorEnemy.SetBool("Count", false);

        if (recallPoints <= 0 && enemyRecallPoints <= 0)
        {
            CountingOver();
        }
    }

    public void CountRecallPoints()
    {
        enemyRecallPoints = journeys[journeyIndexChosen].hiddenRecallPointsAmount[round - 1];
        enemyRecallPointsText.text = enemyRecallPoints.ToString();
        if (recallPoints > 0)
            recallPointAnimator.SetBool("Count", true);
        if (enemyRecallPoints > 0)
            recallPointAnimatorEnemy.SetBool("Count", true);

        if (recallPoints <= 0 && enemyRecallPoints <= 0)
        {
            CountingOver();
        }
    }

    void CountingOver()
    {
        cardSelectionLogger.RecallPointsCounted();
        missionEnded = true;
        missionEndUI.SetActive(true);
        missionEndText.gameObject.SetActive(false);
        StartCoroutine(MissionEndTextPopup());
        playerPointsAnimator.SetBool("MissionEnd", true);
        enemyPointsAnimator.SetBool("MissionEnd", true);
    }

    IEnumerator MissionEndTextPopup()
    {
        yield return new WaitUntil(() => missionEndScoresDone);
        missionEndText.gameObject.SetActive(true);

        int playerPoints = effectManager.GetPoints(false);
        int enemyPoints = effectManager.GetPoints(true);
        
        if(!isTutorial)
        highScore += playerPoints;

        if (playerPoints > enemyPoints)
            missionEndText.text = "Objective Accomplished";
        else if (playerPoints < enemyPoints)
            missionEndText.text = "Objective Failed";
        else
            missionEndText.text = "Objective Inconclusive";
        
        missionEndScoresDone = false;
    }

    public void MissionEndScoresDone()
    {

        if (round > roundsInTotal)
            gameEndScoresDone = true;
        else
            missionEndScoresDone = true;
    }

    public void RoundFinished()
    {
        cardSelectionLogger.ObjectiveEnd();
        if (isTutorial)
            if (tutorial.CanTextAppear(WhenTextAppears.AtTheEndOfOperation))
                return;
        CountRecallPoints();
        /*recallPoints = 0;
        recallPointsText.text = recallPoints.ToString();
        DestroyAllCards();*/
    }

    void DestroyAllCards()
    {
        playerDropZone.DestroyAllCards();
        enemyDropZone.DestroyAllCards();
    }

    void OppTurn()
    {
        Card oppCard = turns[curTurn - 1].prompt;       
        var cardObj = Instantiate(optionPrefab);
        cardObj.GetComponentInChildren<PhysicalCard>().SetCard(oppCard);
        enemyDropZone.PlaceCard(cardObj);
    }

    void NewOppTurn()
    {
        if (isTutorial && curTurn > 3)
            ShowAgentUI();
        else if (turnToRecallEachTurn[curTurn - 1].Count > 0 && !isTutorial)
        {
            ShowAgentUI();
        }
        else
            OppTurn();
    }

    public void ShowAgentUI()
    {
        RecallToLaterTurn();
        agentUI.SetActive(true);

        if (isTutorial)
            if (tutorial.CanTextAppear(WhenTextAppears.WhenInformantShowsUp))
                return;

        agentPopup = true;
    }

    public void AgentAffirmativeButtonPressed()
    {
        if (isTutorial)
        {
            if (!tutorial.TextShown())
            {
                agentPopup = false;
                agentUI.SetActive(false);
                OppTurn();
            }
        }
        else
        {
            agentPopup = false;
            agentUI.SetActive(false);
            OppTurn();
        }
    }

    void StartPlayerTurn()
    {
        playerCardSelectionStartTime = Time.time;
        //DestroyOptions();
        recallText.text = "";
        RecallToLaterTurn();

        //updates prompt and the option buttons
        //promptText.text = turns[curTurn-1].prompt;
        Option[] options = turns[curTurn - 1].options;

        Card[] newHand = new Card[options.Length];
        bool[] deduction = new bool[options.Length];
        bool[] recall = new bool[options.Length];

        for (int i = 0; i < newHand.Length; i++)
        {
            newHand[i] = options[i].answer;
            deduction[i] = options[i].isCorrect;

            bool isRecall = i == recallChoice[curTurn - 1] - 1;
            recall[i] = isRecall;
        }
        hand.DrawNewHand(newHand, deduction, recall);
    }

    public void SetHighlightActive(bool isActive)
    {
        highlightTimerActive = isActive;
    }

    void NewTurn()
    {
        /*if (turnToRecallEachTurn[curTurn - 1].Count > 0)
        {
            ShowAgentUI();
        }
        else*/
            StartPlayerTurn();

        /*for (int i = 0; i < options.Length; i++)
        {
            //GameObject optionObj = Instantiate(optionPrefab, optionSpawnTrans);

            //Button optionBtn = optionObj.GetComponent<Button>();
            //bool isRecall = i == recallChoice[curTurn - 1]-1;

            //bool isCorrect = options[i].isCorrect;
            //optionBtn.onClick.AddListener(delegate { OptionChosen(isCorrect, isRecall); });

            //TMP_Text btnText = optionObj.GetComponentInChildren<TMP_Text>();
            //btnText.text = options[i].answer;
        }*/
    }

    public void StartDrag()
    {
        if (isTutorial)
        {
            if (curTurn < 4)
                recallOptionFrame.SetActive(false);
            else
                recallOptionFrame.SetActive(true);

            if (tutorial.CanTextAppear(WhenTextAppears.WhenDragging))
                return;
        }
    }

    public void DoneDiscarding()
    {
        if (isTutorial)
            if (tutorial.CanTextAppear(WhenTextAppears.AfterPlayingACard))
                return;

        TurnEnd();
    }
}
