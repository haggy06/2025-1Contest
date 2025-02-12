using UnityEngine;

public interface IItemInteractable
{
    public bool CanInteract {  get; set; }
    public void InteractStart(DragItem item);
    public void InteractEnd(DragItem item);
}
