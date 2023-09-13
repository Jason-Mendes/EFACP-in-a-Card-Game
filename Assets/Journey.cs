using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newJourney", menuName = "Journey")]
public class Journey : ScriptableObject
{
    public int[] hiddenRecallPointsAmount;
    public Turn[] turns;
}
