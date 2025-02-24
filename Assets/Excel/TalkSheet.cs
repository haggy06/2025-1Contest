using UnityEngine;
using System;

[Serializable]
public class TalkSheet
{
    public bool next;

    public int speaker;

    public int face;

    public string text;

    public string methodName;
    public int parameter;
    public bool startInvoke;
}

public enum Speaker
{
    Nobody,
    Me,
    God,
    Info,

    Tutorial1,
    Tutorial2,
    Tutorial3,

    Ingame_NPC,
    Ingame_Player,
}
public enum Face
{
    Sneer,
    Cry,
    Normal,
    Angry,
    Smile,
    Laugh,
    Sorry
}

public enum StoryTalkIndex
{
    Prolog = 0,
    Tutorial = 20,

    TrapEnd = 46,
    BankruptcyEnd = 55,
    NormalEnd = 59,
    HappyEnd = 66,
}

public enum NPCTalkIndex
{
    Regular_First = 9,
    Regular_Second = 12,

    Assasine = 19,

    Spy_LowMoney = 24,
    Spy_UnknownSpy = 26,
    Spy_Accept = 28,
}