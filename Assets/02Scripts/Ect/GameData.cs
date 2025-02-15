using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameData
{
    public int day = 1;

    public int money = 10000;
    public float danger = 0f;

    public ItemCount[] itemStatus = new ItemCount[19];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ResetOutCount()
    {
        foreach (var item in itemStatus)
        {
            item.outCount = 0;
        }
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

    public ItemCount(DragItem item)
    {
        this.item = item;
    }
}