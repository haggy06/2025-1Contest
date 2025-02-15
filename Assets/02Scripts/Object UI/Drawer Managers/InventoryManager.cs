using UnityEngine;

public class InventoryManager : MonoBehaviour, IItemInteractable
{
    [field:SerializeField]
    public bool CanInteract { get; set; }

    [SerializeField]
    private Vector2 inventoryGridSize = new Vector2(4, 4);
    [SerializeField]
    private Vector2 inventorySlotSize = new Vector2(1, 1);

    [Space(5)]
    [SerializeField]
    private ObjectPool pool;
    [SerializeField]
    private Transform inventory;
    [SerializeField]
    private InventorySlot slot;

    private InventorySlot[] slotArr = null;


    private void Awake()
    {
        slotArr = new InventorySlot[DataManager.GameData.itemStatus.Length];
        for (int i = 0; i < DataManager.GameData.itemStatus.Length; i++) // �����ۺ� ���� ����
        {
            InventorySlot newSlot = Instantiate(slot, inventory);

            newSlot.name = DataManager.GameData.itemStatus[i].item.itemData.itemType + " Slot";
            newSlot.SetItem(DataManager.GameData.itemStatus[i].item, i);
            slotArr[i] = newSlot;
        }
    }

    private void OnEnable()
    {
        Init();
    }

    public void Init(bool refreshNumber = true)
    {
        ItemCount[] itemC = DataManager.GameData.itemStatus;

        int activeSlotCount = 0;
        for (int i = 0; i < slotArr.Length; i++)
        {
            int invCount = itemC[i].GetInventoryCount();

            if (invCount > 0)
            {
                slotArr[i].gameObject.SetActive(true);
                if (refreshNumber)
                    slotArr[i].RefreshCount();

                print(activeSlotCount + ", " + new Vector2((activeSlotCount % inventoryGridSize.x) * inventorySlotSize.x, -(int)(activeSlotCount / inventoryGridSize.y) * inventorySlotSize.y));
                slotArr[i].transform.localPosition = new Vector2((activeSlotCount % inventoryGridSize.x) * inventorySlotSize.x, -(int)(activeSlotCount / inventoryGridSize.y) * inventorySlotSize.y);
                activeSlotCount++;
            }
            else
            {
                slotArr[i].gameObject.SetActive(false);
            }
        }
    }
    public void RemoveSlot(DragItem item)
    {
        int activeSlotCount = 0;
        for (int i = 0; i < slotArr.Length; i++)
        {
            if (slotArr[i].gameObject.activeSelf)
                activeSlotCount++;

            if (slotArr[i].item == item)
            {
                slotArr[i].gameObject.SetActive(false);
                slotArr[i].RefreshCount();

                activeSlotCount--;

                for (int j = i +1; j < slotArr.Length; j++)
                {
                    slotArr[j].transform.localPosition = new Vector2((activeSlotCount % inventoryGridSize.x) * inventorySlotSize.x, -(int)(activeSlotCount / inventoryGridSize.y) * inventorySlotSize.y); // ��ġ ������
                    activeSlotCount++;
                }

                break;
            }
        }
    }

    public void AddSlot(DragItem item)
    {
        int activeSlotCount = 0;
        for (int i = 0; i < slotArr.Length; i++)
        {
            if (slotArr[i].gameObject.activeSelf)
                activeSlotCount++;

            if (slotArr[i].item == item)
            {
                slotArr[i].gameObject.SetActive(false);
                slotArr[i].RefreshCount();

                activeSlotCount++;

                for (int j = i + 1; j < slotArr.Length; j++)
                {
                    slotArr[j].transform.localPosition = new Vector2((activeSlotCount % inventoryGridSize.x) * inventorySlotSize.x, -(int)(activeSlotCount / inventoryGridSize.y) * inventorySlotSize.y); // ��ġ ������
                    activeSlotCount++;
                }

                break;
            }
        }
    }

    public void InstantiateItem(DragItem item)
    {
        DragItem outItem = pool.GetPoolObject<DragItem>(item);
        outItem.transform.position = MyCalculator.GetMousePosition();

        outItem.SetCurInteractArbitrarily(this);
        outItem.DragStart();
    }

    public bool InteractStart(DragItem item)
    {
        item.ReturnToPool(); // �ǵ� �ȵǵ� �ϴ� ����

        if (CanInteract)
        {
            ItemCount itemC = MyCalculator.GetItemCount(item);
            if (itemC.itemCount > 0 && itemC.outCount > 0)
            {
                itemC.outCount--;
                if (itemC.GetInventoryCount() == 1) // ���� 1�� �Ǿ��� ���(=������ ���ܾ� �� ���)
                {
                    Init(false);
                }

                slotArr[item.itemData.itemIndex].RefreshCount();

                return true;
            }
            else
            {
                Debug.LogError("�������� �ʴ� �������� ����");
            }
        }
        return false;
    }

    public bool InteractEnd(DragItem item)
    {
        return true;
    }
}
