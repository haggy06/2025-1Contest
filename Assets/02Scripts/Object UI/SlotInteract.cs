using System;
using UnityEngine;
using static UnityEditor.Progress;

public class SlotInteract : MonoBehaviour, IItemInteractable
{
    [field: SerializeField]
    public bool CanInteract { get; set; } = true;
    [SerializeField]
    private bool fixItemTransform = true;

    [SerializeField]
    private DragItem _curItem;
    public DragItem curItem => _curItem;

    public void UseItem()
    {
        InteractEnd(_curItem, true);
    }

    public void InteractStart(DragItem item)
    {
        _curItem = item;
        InteractChanged?.Invoke();

        if (fixItemTransform)
        {
            item.RemoveVelocity(true);

            item.transform.position = new Vector3(transform.position.x, transform.position.y, item.transform.position.z);
            item.transform.eulerAngles = Vector3.zero;
        }

        CanInteract = false;
    }

    public void InteractEnd(DragItem item)
    {
        if (curItem == item)
        {
            _curItem = null;
            InteractChanged?.Invoke();

            CanInteract = true;
        }
    }
    public void InteractEnd(DragItem item, bool destroyItem)
    {
        if (curItem == item)
        {
            if (destroyItem)
                _curItem.ReturnToPool();

            _curItem = null;
            InteractChanged?.Invoke();

            CanInteract = true;
        }
    }

    public event Action InteractChanged;
}
