using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class MyCalculator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 GetMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddComma(int num)
    {
        return String.Format("{0:#,0}", num);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ItemCount GetItemCount(DragItem item)
    {
        return DataManager.GameData.itemStatus[item.itemData.itemIndex];
    }
}
