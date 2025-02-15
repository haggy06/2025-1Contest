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
    private InteractLimit interactLimit = InteractLimit.Every;

    [Space(5)]
    [SerializeField]
    private DragItem _curItem;
    public DragItem curItem => _curItem;

    public void UseItem()
    {
        InteractEnd(_curItem, true);
    }

    public bool InteractStart(DragItem item)
    {
        if (CanInteract)
        {
            switch (interactLimit)
            {
                case InteractLimit.Elemental:
                    if ((int)item.itemData.itemType >= 100) // 결과물일 경우 반환
                        return false;
                    break;
                case InteractLimit.Output:
                    if ((int)item.itemData.itemType < 100) // 재료일 경우 반환
                        return false;
                    break;
            }

            _curItem = item;
            InteractChanged?.Invoke();

            if (fixItemTransform)
            {
                item.RemoveVelocity(true);

                item.transform.position = new Vector3(transform.position.x, transform.position.y, item.transform.position.z);
                item.transform.eulerAngles = Vector3.zero;
            }

            CanInteract = false;

            return true;
        }
        else
        {
            return false;
        }
    }

    public bool InteractEnd(DragItem item)
    {
        if (curItem == item)
        {
            _curItem = null;
            InteractChanged?.Invoke();

            CanInteract = true;

            return true;
        }
        else
        {
            return false;
        }
    }
    public bool InteractEnd(DragItem item, bool destroyItem)
    {
        if (curItem == item)
        {
            if (destroyItem)
                _curItem.ReturnToPool();

            _curItem = null;
            InteractChanged?.Invoke();

            CanInteract = true;

            return true;
        }
        else
        {
            return false;
        }
    }

    public event Action InteractChanged;
}

public enum InteractLimit
{
    Every,
    Elemental,
    Output,

}