using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(BoxCollider2D))]
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
    private BoxCollider2D col;
    private void Awake()
    {
        inventoryManager = GetComponentInParent<InventoryManager>();
        col = GetComponent<BoxCollider2D>();
    }

    private void OnMouseDown()
    {
        ItemCount itemCount = DataManager.GameData.itemStatus[_slotIndex];
        itemCount.outCount++;
        print(itemCount.outCount);

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
        icon.sprite = item.itemData.iconSprite;

        _slotIndex = slotIndex;
        RefreshCount();
    }
    public void RefreshCount()
    {
        countText.text = DataManager.GameData.itemStatus[_slotIndex].GetInventoryCount().ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ColActive(bool active)
    {
        col.enabled = active;
    }
}
