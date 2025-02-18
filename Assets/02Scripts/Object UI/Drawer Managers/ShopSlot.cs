using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(BoxCollider2D))]
public class ShopSlot : ObjectTriggerButton
{
    [SerializeField]
    private AudioClip failSound;
    [SerializeField]
    private GameObject soldOut;
    [SerializeField]
    private SpriteRenderer icon;
    [SerializeField]
    private TextMeshPro price;

    [Space(5)]
    [SerializeField]
    private ItemData _item;
    public ItemData item => _item;

    private BoxCollider2D col;
    protected new void Awake()
    {
        base.Awake();
        col = GetComponent<BoxCollider2D>();

        icon.sprite = _item.sprite;
        price.text = MyCalculator.AddComma(_item.price);

        ResetStock();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ColActive(bool active)
    {
        if (!soldOut.activeSelf) // 품절되지 않은 상태에서만 변경 가능.
            col.enabled = active;
    }

    protected override void ButtonClick()
    {
        if (normalSprite)
            sRenderer.sprite = normalSprite;
        sRenderer.color = normalColor;

        if (_item.price < DataManager.GameData.money)
        {
            if (IsThereStock())
            {
                PlaySound();
                clickEvent?.Invoke();

                DataManager.GameData.money = DataManager.GameData.money - _item.price;
                IngameCanvasManager.RefreshMoney();

                GetStoke();
                if (!IsThereStock())
                {
                    ColActive(false);
                    soldOut.SetActive(true);
                }
            }
        }
    }
    protected override void PlaySound()
    {
        if (_item.price < DataManager.GameData.money)
        {
            // Todo 소리 재생
        }
        else
        {

        }
    }

    private int curStock;
    public void ResetStock()
    {
        soldOut.SetActive(false);
        ColActive(true);

        curStock = _item.stock;
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
