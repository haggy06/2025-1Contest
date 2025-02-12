using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopManager : Singleton<ShopManager>
{
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

    protected new void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        Init();
    }
    private void Init()
    {
        slotScroll.transform.localPosition = Vector3.zero;

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

    public void Buy(int itemID)
    {
        // Todo : 아이템 구매 구현
    }

    protected override void SceneChanged(Scene replacedScene, Scene newScene)
    {
        
    }
}
