using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Turn 
{
    public Card prompt;
    public Option[] options;
    public int recallTurnsBefore;
}
