using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public abstract class ObjectButtonBase : MonoBehaviour, IDragable
{
    bool isPress = false;

    [field: SerializeField]
    public bool Active { get; set; } = true;

    private void OnMouseEnter()
    {
        isPress = true;
    }
    private void OnMouseExit()
    {
        isPress = false;
    }

    private void OnMouseDown()
    {
        isPress = true;
        DragStart();
    }
    private void OnMouseUp()
    {
        DragEnd();
    }
    protected abstract void ButtonPress();
    protected abstract void ButtonClick();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DragStart()
    {
        ButtonPress();
    }

    public void Dragging()
    {

    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DragEnd()
    {
        ButtonClick();
    }
}
