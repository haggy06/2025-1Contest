using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField]
    private ObjectPool pool;
    [SerializeField]
    private ShopSlot[] slotArr;

    [Space(5)]
    [SerializeField]
    private GameObject slotScroll;

    [SerializeField]
    private float slotRange = 3f;
    [SerializeField]
    private float scrollTime = 0.2f;
    [SerializeField]
    private LeanTweenType scrollType = LeanTweenType.linear;
    private void Awake()
    {
        Init();

        GameManager.Inst.Subscribe(EventType.TutorialStart, TutorialSet);
        GameManager.Inst.Subscribe(EventType.TutorialFinish, Init);
        GameManager.Inst.Subscribe(EventType.DayStart, Init);
    }
    private void TutorialSet()
    {
        DataManager.GameData.itemStatus[9].itemCount++;
        for (int i = 0; i < slotArr.Length; i++)
        {
            if (GameManager.Inst.tutorialItem != slotArr[i].item)
                slotArr[i].SoldOut();
        }
    }
    /*
            if (slotArr[curScrollIndex].isSoldOut)
                slotArr[curScrollIndex].SoldOut();
    */
    public void Init()
    {
        DataManager.GameData.todayBuyCount = 0;

        slotScroll.transform.localPosition = Vector3.zero;
        curScrollIndex = 0;

        for (int i = 0; i < slotArr.Length; i++)
        {
            slotArr[i].ResetStock();
            if (i > 0)
                slotArr[i].ColActive(false);
        }
    }

    private int curScrollIndex = 0;
    public void ScrollUP()
    {
        if (curScrollIndex > 0)
        {
            slotArr[curScrollIndex].ColActive(false);
            curScrollIndex--;
            slotArr[curScrollIndex].ColActive(true);

            SlotLeanTween(curScrollIndex * slotRange);
        }
    }
    public void ScrollDOWN()
    {
        if (curScrollIndex < slotArr.Length-1)
        {
            slotArr[curScrollIndex].ColActive(false);
            curScrollIndex++;
            slotArr[curScrollIndex].ColActive(true);

            SlotLeanTween(curScrollIndex * slotRange);
        }
    }

    private int slotTweenID = 0;
    private void SlotLeanTween(float targetY)
    {
        LeanTween.cancel(slotTweenID);

        slotTweenID = LeanTween.moveLocalY(slotScroll, targetY, scrollTime).setEase(scrollType).id;
    }

    public void Buy(int itemIndex)
    {
        DataManager.GameData.itemStatus[itemIndex].itemCount++;
        DataManager.GameData.todayBuyCount++;

        GameManager.Inst.TutorialCheck(TutorialState.Shopping);

        /*
        DragItem poolObj = pool.GetPoolObject<DragItem>(item);
        poolObj.Init(new Vector2(Random.Range(-4f, -3f), Random.Range(-3.5f, 2f)), 0f);
        */
    }
}
