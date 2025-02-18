using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour, IItemInteractable
{
    [field:SerializeField]
    public bool CanInteract { get; set; }

    [SerializeField]
    private Vector2Int inventoryGridSize = new Vector2Int(4, 4);
    [SerializeField]
    private Vector2 inventorySlotSize = new Vector2(1, 1);

    [Space(5)]
    [SerializeField]
    private ObjectPool pool;
    [SerializeField]
    private GameObject inventory;
    [SerializeField]
    private Transform inventoryPivot;
    [SerializeField]
    private InventorySlot slot;

    private InventorySlot[] slotArr = null;

    [Space(5)]
    [SerializeField]
    private float scrollTime = 0.1f;
    [SerializeField]
    private LeanTweenType scrollType = LeanTweenType.linear;

    private void Awake()
    {
        slotArr = new InventorySlot[DataManager.GameData.itemStatus.Length];
        for (int i = 0; i < DataManager.GameData.itemStatus.Length; i++) // �����ۺ� ���� ����
        {
            InventorySlot newSlot = Instantiate(slot, inventoryPivot);

            newSlot.name = DataManager.GameData.itemStatus[i].item.itemData.itemType + " Slot";
            newSlot.SetItem(DataManager.GameData.itemStatus[i].item, i);
            slotArr[i] = newSlot;
        }
    }

    private void OnEnable()
    {
        Init();
    }

    private Queue<InventorySlot> activeSlotQueue = new Queue<InventorySlot>(10); // ���� �ִ� ������ 19ĭ������... �װ� �� �� ���� �ְھ�?
    public void Init(bool refreshNumber = true)
    {
        ItemCount[] itemC = DataManager.GameData.itemStatus;

        activeSlotQueue.Clear();
        for (int i = 0; i < slotArr.Length; i++)
        {
            int invCount = itemC[i].GetInventoryCount();

            if (invCount > 0)
            {
                slotArr[i].gameObject.SetActive(true);
                if (refreshNumber)
                    slotArr[i].RefreshCount();

                slotArr[i].transform.localPosition = new Vector2((activeSlotQueue.Count % inventoryGridSize.x) * inventorySlotSize.x, -(activeSlotQueue.Count / inventoryGridSize.y) * inventorySlotSize.y);
                activeSlotQueue.Enqueue(slotArr[i]);
            }
            else
            {
                slotArr[i].gameObject.SetActive(false);
            }
        }
        ScrollActiveChange();
    }
    public void ScrollActiveChange()
    {
        Queue<InventorySlot> activeQueue = new Queue<InventorySlot>(activeSlotQueue);
        for (int i = 0; i < activeSlotQueue.Count; i++)
        {
            InventorySlot invSlot = activeQueue.Dequeue();
            
            int lineIndex = i / inventoryGridSize.x;
            invSlot.ColActive((scrollDepth <= lineIndex && lineIndex < inventoryGridSize.y + scrollDepth)); // ������ ���� �׸��忡 ���� ���� ��쿡�� true
        }
    }

    [SerializeField]
    private int scrollDepth = 0;
    private int inventoryTweenID = 0;
    public void SlotScroll(bool isUp)
    {
        scrollDepth = Mathf.Clamp(scrollDepth + (isUp ? -1 : 1), 0, Mathf.CeilToInt(Mathf.Clamp(activeSlotQueue.Count - inventoryGridSize.x * inventoryGridSize.y, 0, int.MaxValue) / (float)inventoryGridSize.x));

        LeanTween.cancel(inventoryTweenID);
        inventoryTweenID = LeanTween.moveLocalY(inventory, inventorySlotSize.y * scrollDepth, scrollTime).setEase(scrollType).id;

        ScrollActiveChange();
    }

    /*    
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
    */

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
