using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum WhenTextAppears
{
    AfterEnemyPlayedCard,
    AfterThePrevious,
    AfterPlayingACard,
    WhenInformantShowsUp,
    AtTheEndOfOperation,
    AtTheEndOfDay,
    WhenDragging,
    JustBeforeChoicePromptGoesAway
}

[System.Serializable]
public class TutorialText
{
    public string tutorialText;
    public WhenTextAppears whenTextAppears;
}

[System.Serializable]
public class TutorialTextPersistent
{
    public int turn;
    public string tutorialText;
    public WhenTextAppears whenTextAppear;
    public WhenTextAppears whenTextDisappear;
}

public class Tutorial : MonoBehaviour
{
    //string[] tutorialText = { "Your enemy played a card with 6 power", "Get most points to win", "Drag the card with the highest power", "After playing a card, the day ends.", "You will draw new cards each day." };
    public TMP_Text tutorialTxt;
    public TMP_Text persistentTxt;
    public GameObject tutorialPopUp;
    public GameObject tutorialPersistentPopUp;

    public TutorialText[] tutorialTexts;
    public TutorialTextPersistent[] tutorialTextsPersistent;

    int curIndex = 0;
    int curIndexPersistent = 0;

    WhenTextAppears lastWTA;

    TurnHandler turnHandler;
    DropZone dropzone;

    private void Start()
    {
        turnHandler = GameObject.FindObjectOfType<TurnHandler>();
        dropzone = GameObject.FindObjectOfType<DropZone>();
    }

    private void Update()
    {
        if (tutorialPopUp.gameObject.activeSelf)
            if (Input.GetMouseButtonDown(0))
                if (!CanTextAppear(WhenTextAppears.AfterThePrevious))
                {
                    tutorialPopUp.gameObject.SetActive(false);
                    GoBackToGame();
                }
    }

    public bool CanTextAppear(WhenTextAppears wTA)
    {
        if (tutorialTextsPersistent.Length > curIndexPersistent)
        {

            if (turnHandler.GetCurTurn() == tutorialTextsPersistent[curIndexPersistent].turn)
            {
                if (tutorialTextsPersistent[curIndexPersistent].whenTextDisappear == wTA)
                {
                    RemovePersistentText();

                    curIndexPersistent++;
                }
            }
        }
        if (tutorialTextsPersistent.Length > curIndexPersistent)
        {
            if (turnHandler.GetCurTurn() == tutorialTextsPersistent[curIndexPersistent].turn)
            {
                if (tutorialTextsPersistent[curIndexPersistent].whenTextAppear == wTA)
                {
                    MakePersistentTextAppear();
                }
            }
        }

        if (tutorialTexts.Length > curIndex)
            if (tutorialTexts[curIndex].whenTextAppears == wTA)
            {
                MakeTextAppear();
                curIndex++;

                if (wTA != WhenTextAppears.AfterThePrevious)
                    lastWTA = wTA;
                return true;
            }

        return false;
    }

    void GoBackToGame()
    {
        switch (lastWTA)
        {
            case WhenTextAppears.AfterPlayingACard:
                turnHandler.DoneDiscarding();   
                break;
            case WhenTextAppears.AfterEnemyPlayedCard:
                turnHandler.TurnEnd();
                break;
            case WhenTextAppears.AtTheEndOfOperation:
                turnHandler.RoundFinished();
                break;
            case WhenTextAppears.WhenInformantShowsUp:
                turnHandler.ShowAgentUI();
                break;
            case WhenTextAppears.JustBeforeChoicePromptGoesAway:
                dropzone.StopDragCard();
                dropzone.PlaceCard();
                break;
        }
    }

    public bool TextShown()
    {
        return tutorialPopUp.gameObject.activeSelf;
    }

    void MakePersistentTextAppear()
    {
        tutorialPersistentPopUp.gameObject.SetActive(true);
        persistentTxt.text = tutorialTextsPersistent[curIndexPersistent].tutorialText;
    }
    void RemovePersistentText()
    {

        tutorialPersistentPopUp.gameObject.SetActive(false);
    }

    void MakeTextAppear()
    {
        tutorialPopUp.gameObject.SetActive(true);
        tutorialTxt.text = tutorialTexts[curIndex].tutorialText;
    }
}
