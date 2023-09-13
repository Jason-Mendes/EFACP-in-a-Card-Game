using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
// https://answers.unity.com/questions/1567607/how-to-change-inspector-with-non-monobehaviour-obj.html
[CustomPropertyDrawer(typeof(Effect), false)]
public class MyActionPropertyDrawer : PropertyDrawer
{
    //static Card card;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var rectWhen = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        var rectWhen2 = new Rect(position.x, position.y + 20f, position.width, EditorGUIUtility.singleLineHeight);

        var rectWhat = new Rect(position.x, position.y + 45f, position.width, EditorGUIUtility.singleLineHeight);
        var rectWhat2 = new Rect(position.x, position.y + 65f, position.width, EditorGUIUtility.singleLineHeight);
        var rectWhat3 = new Rect(position.x, position.y + 85f, position.width, EditorGUIUtility.singleLineHeight);

        var rectAction = new Rect(position.x, position.y + 110f, position.width, EditorGUIUtility.singleLineHeight);
        var rectAction2 = new Rect(position.x, position.y + 130, position.width, EditorGUIUtility.singleLineHeight);
        var rectAction3 = new Rect(position.x, position.y + 150, position.width, EditorGUIUtility.singleLineHeight);

        var when = property.FindPropertyRelative("when");
        var whenTribe = property.FindPropertyRelative("whenTribe");

        var what = property.FindPropertyRelative("target");
        var whatTribe = property.FindPropertyRelative("targetTribe");
        var targetVal1 = property.FindPropertyRelative("targetVal1");

        var action = property.FindPropertyRelative("action");
        var val1 = property.FindPropertyRelative("val1");
        var val2 = property.FindPropertyRelative("val2");
        var actionTribe = property.FindPropertyRelative("actionTribe");
        var card = property.FindPropertyRelative("spawnedCard");
        var abilityGiven = property.FindPropertyRelative("abilityGiven");


        when.intValue = EditorGUI.Popup(rectWhen, "When", when.intValue, when.enumNames);

        switch ((WhenEffectHappens)when.intValue)
        {


        }

        what.intValue = EditorGUI.Popup(rectWhat, "Target", what.intValue, what.enumNames);

        switch ((Target)what.intValue)
        {
            case Target.AllEnemies:
            case Target.AllOtherPals:
                whatTribe.intValue = EditorGUI.Popup(rectWhat2, "Specific Type", whatTribe.intValue, whatTribe.enumNames);
                break;
            case Target.RandomPal:
            case Target.RandomOpponent:
                targetVal1.intValue = EditorGUI.IntField(rectWhat2, "Amount", targetVal1.intValue);
                whatTribe.intValue = EditorGUI.Popup(rectWhat3, "Specific Type", whatTribe.intValue, whatTribe.enumNames);
                break;
        }


        action.intValue = EditorGUI.Popup(rectAction, "Effect", action.intValue, action.enumNames);

        switch ((Action)action.intValue)
        {
            case Action.DecreasePowerForEachCard:
            case Action.DecreasePowerOnOppNextCard:
            case Action.GainPowerForEachFriendlyCard:
            case Action.DecreasePowerForEachFriendlyCard:
            case Action.DecreasePower:
            case Action.GainPower:
                val1.intValue = EditorGUI.IntField(rectAction2, "Amount", val1.intValue);
                break;

            case Action.IfOppLastCardHasXPowerOrLessGainPower:
                val1.intValue = EditorGUI.IntField(rectAction2, "X power or less", val1.intValue);
                val2.intValue = EditorGUI.IntField(rectAction3, "Gain Amount", val2.intValue);
                break;

            case Action.IfOppControlXGainPower:
            case Action.IfOppLastCardIsSpecificTribeGainPower:
            case Action.GainPowerForEachFriendlySpecificTribe:
            case Action.DecreasePowerForEachFriendlySpecificTribe:
            case Action.IfYouControlXGainPower:
            case Action.GainPowerForEachOppCardOfSpecificTribe:
                actionTribe.intValue = EditorGUI.Popup(rectAction2, "Specific Type", actionTribe.intValue, actionTribe.enumNames);
                val1.intValue = EditorGUI.IntField(rectAction3, "Amount", val1.intValue);
                break;

        }

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();

    }

    //This will need to be adjusted based on what you are displaying
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (150 - EditorGUIUtility.singleLineHeight) + (EditorGUIUtility.singleLineHeight * 2);
    }
}
#endif