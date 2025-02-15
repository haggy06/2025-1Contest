using UnityEngine;

public interface IItemInteractable
{
    public bool CanInteract {  get; set; }
    public bool InteractStart(DragItem item);
    public bool InteractEnd(DragItem item);
}
