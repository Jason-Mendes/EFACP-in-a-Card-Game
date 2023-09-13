using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
//https://forum.unity.com/threads/card-effects-for-a-game-bad-coding-vs-good-coding.1075513/

public enum Tribe
{
    Agent,
    Soldier,
    Scientist,
    Diplomat,
    None
}

public enum EntranceAnimation
{
    SwingPlay,
    FromBelow
}

public enum WhenEffectHappens
{
    OnPlay
}

public enum Target
{
    Itself,
    AllOtherPals,
    AllEnemies,
    RandomOpponent,
    RandomPal,
    StrongestOpponent,
    NoTarget
}
public enum Action
{
    GainPower,
    DecreasePower,
    HalvePower,
    DecreasePowerForEachFriendlyCard,
    GainPowerForEachFriendlyCard,
    Destroy,
    NextCardPlayedDoublePower,
    DoublePower,
    DecreasePowerOnOppNextCard,
    IfOppLastCardHasXPowerOrLessGainPower,
    GainPowerForEachOppCardOfSpecificTribe,
    IfYouControlXGainPower,
    GainPowerForEachFriendlySpecificTribe,
    DecreasePowerForEachFriendlySpecificTribe,
    IfOppLastCardIsSpecificTribeGainPower,
    IfOppControlXGainPower,
    DecreasePowerForEachCard
}

[CreateAssetMenu(fileName = "Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public enum CardType
    {
        Minion,
        Spell
    }

    public CardType cardType = new CardType();
    
    public string description;
    public int power;
    public Sprite artwork;
    public Tribe tribe;

    public EntranceAnimation entranceAnimation;

    public Tribe GetTribe()
    {
        return tribe;
    }

    public Effect[] effects;
    
    public Effect[] GetEffects()
    {
        return effects;
    }
}
