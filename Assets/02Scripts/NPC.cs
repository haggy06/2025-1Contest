using System;
using System.Runtime.CompilerServices;

[Serializable]
public class NPC
{
    public const int NPC_Number = 23;

    public int faceIndex = 0;
    public NPCType npcType = NPCType.Normal;
    public int _orderItemIndex;
    public ItemData orderItem => DataManager.itemDataList.dataList[_orderItemIndex].itemData;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RandomFace()
    {
        faceIndex = UnityEngine.Random.Range((int)NPCType.Normal, NPC_Number);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RandomOrder()
    {
        _orderItemIndex = UnityEngine.Random.Range(10, DataManager.itemDataList.dataList.Length - 1);
    }
}
public enum NPCType
{   
    God,
    Regular,
    Assassin,

    Spy,

    Normal
}