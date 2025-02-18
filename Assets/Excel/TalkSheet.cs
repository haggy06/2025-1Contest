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