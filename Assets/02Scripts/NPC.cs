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
    public void RandomFace(bool ignoreSpyFace = false)
    {
        if (ignoreSpyFace) // 스파이와 얼굴이 같아도 될 경우(or 스파이 얼굴 정하는 경우)
        {
            faceIndex = UnityEngine.Random.Range((int)NPCType.Normal, NPC_Number);
        }
        else
        {
            do
            {
                faceIndex = UnityEngine.Random.Range((int)NPCType.Normal, NPC_Number);
            } 
            while (faceIndex == DataManager.GameData.spyNPC.faceIndex);
        }
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