using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class Effect
{
    [HideInInspector]
    public WhenEffectHappens when;
    [HideInInspector] public Tribe whenTribe;

    [HideInInspector] public Target target;
    [HideInInspector] public Tribe targetTribe;
    [HideInInspector] public int targetVal1;

    [HideInInspector] public Action action;
    [HideInInspector] public int val1;
    [HideInInspector] public int val2;
    [HideInInspector] public Tribe actionTribe;

}
