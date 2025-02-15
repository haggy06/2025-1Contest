using TMPro;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    private DragItem _item;
    public DragItem item => _item;
    private int _slotIndex;

    [SerializeField]
    private SpriteRenderer icon;
    [SerializeField]
    private TextMeshPro countText;

    private InventoryManager inventoryManager;
    private void Awake()
    {
        inventoryManager = GetComponentInParent<InventoryManager>();
    }
    private void OnMouseDown()
    {
        ItemCount itemCount = DataManager.GameData.itemStatus[_slotIndex];
        itemCount.outCount++;

        if (itemCount.GetInventoryCount() <= 0)
        {
            inventoryManager.Init(false);
        }
        else
        {
            RefreshCount();
        }
        inventoryManager.InstantiateItem(item);
    }

    public void SetItem(DragItem item, int slotIndex)
    {
        _item = item;
        icon.sprite = item.itemData.sprite;

        _slotIndex = slotIndex;
        RefreshCount();
    }
    public void RefreshCount()
    {
        countText.text = DataManager.GameData.itemStatus[_slotIndex].GetInventoryCount().ToString();
    }
}
