using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(BoxCollider2D))]
public class ShopSlot : ObjectTriggerButton
{
    [SerializeField, Space(5)]
    private BoxCollider2D col;

    [SerializeField]
    private AudioClip failSound;
    [SerializeField]
    private GameObject soldOut;
    [SerializeField]
    private SpriteRenderer nameSprite;
    [SerializeField]
    private SpriteRenderer icon;
    [SerializeField]
    private TextMeshPro price;

    [Space(5)]
    [SerializeField]
    private ItemData _item;
    public ItemData item => _item;

    protected new void Awake()
    {
        base.Awake();

        nameSprite.sprite = _item.nameSprite;
        icon.sprite = _item.iconSprite;

        price.text = MyCalculator.AddComma(_item.price);
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

        if (_item.price <= DataManager.GameData.money)
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
                    SoldOut();
                }
            }
        }
        else
            AudioManager.Inst.PlaySFX(failSound);
    }
    public void SoldOut()
    {
        ColActive(false);
        soldOut.SetActive(true);
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
