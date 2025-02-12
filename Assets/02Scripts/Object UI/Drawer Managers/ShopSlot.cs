using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(BoxCollider2D))]
public class ShopSlot : ObjectTriggerButton
{
    [SerializeField]
    private AudioClip failSound;
    [SerializeField]
    private GameObject soldOut;

    [Space(5)]
    [SerializeField]
    private int _itemID;
    public int itemID => _itemID;

    [SerializeField]
    private int _price;
    public int price => _price;


    [SerializeField]
    private int stock;
    private int curStock;

    private BoxCollider2D col;
    protected new void Awake()
    {
        base.Awake();
        col = GetComponent<BoxCollider2D>();
    }
    public void ColActive(bool active)
    {
        col.enabled = active;
    }

    protected override void ButtonClick()
    {
        base.ButtonClick();

        if (price < DataManager.GameData.money)
        {
            if (IsThereStock())
            {
                ShopManager.Inst.Buy(itemID);
                GetStoke();

                if (!IsThereStock())
                {
                    soldOut.SetActive(true);
                    Active = false;
                }
            }
        }
    }
    protected override void PlaySound()
    {
        if (price < DataManager.GameData.money)
        {
            //Todo 소리 재생
        }
        else
        {

        }
    }

    public void ResetStock()
    {
        soldOut.SetActive(false);
        Active = true;

        curStock = stock;
    }
    public bool IsThereStock()
    {
        return curStock > 0;
    }
    public bool GetStoke()
    {
        if (curStock > 0)
        {
            curStock--;

            return true;
        }
        else
            return false;
    }
}
