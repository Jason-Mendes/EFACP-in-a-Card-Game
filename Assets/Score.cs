using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    public TurnHandler turnHandler;

    private void Start()
    {
        turnHandler = GameObject.FindObjectOfType<TurnHandler>();
    }

    public void ScoresMovingEnded()
    {
        turnHandler.MissionEndScoresDone();
    }

}
