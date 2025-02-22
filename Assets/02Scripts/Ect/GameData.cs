using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameData
{
    public int day = 1;

    public int money = 15000;
    public float danger = 0f;

    public NPC spyNPC = new NPC();
    public bool isSpyOwnered = false;
    public bool isSpyDead = false;
    public bool isSpyWkown = false;

    public bool getFunded = false;
    public int todayBuyCount = 0;

    public ItemCount[] itemStatus = new ItemCount[19];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ResetOutCount()
    {
        foreach (var item in itemStatus)
        {
            item.outCount = 0;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool DangerBySpy()
    {
        return isSpyOwnered && !isSpyDead; // 스파이 주문을 들어줬고 스파이가 안 죽었을 경우
    }
}

[Serializable]
public class ItemCount
{
    public DragItem item = null;
    public int itemCount = 0;
    public int outCount = 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetInventoryCount()
    {
        return itemCount - outCount;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DelectItem()
    {
        itemCount = Mathf.Clamp(itemCount - 1, 0, int.MaxValue);
        outCount = Mathf.Clamp(outCount - 1, 0, int.MaxValue);
    }

    public ItemCount(DragItem item)
    {
        this.item = item;
    }
}