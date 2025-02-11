using UnityEngine;

public class CursorCheck : MonoBehaviour,IDragable
{
    public bool Active { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public void DragEnd()
    {
        throw new System.NotImplementedException();
    }

    public void Dragging()
    {
        throw new System.NotImplementedException();
    }

    public void DragStart()
    {
        print(gameObject.name);
    }

    private void OnMouseEnter()
    {

    }
    private void OnMouseDown()
    {
        DragStart();
    }
}
