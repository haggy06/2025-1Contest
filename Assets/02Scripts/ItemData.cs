using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    [SerializeField]
    private ItemType _itemType;
    public ItemType itemType => _itemType;

    [SerializeField]
    private Sprite _sprite;
    public Sprite sprite => _sprite;

    [SerializeField]
    private int _price;
    public int price => _price;

    [SerializeField]
    private int _stock;
    public int stock => _stock;

    [SerializeField]
    private Recipe[] _recipes;
    public Recipe[] recipes => _recipes;
}

[Serializable]
public struct Recipe
{
    public ItemType element;
    public DragItem output;

    public Recipe(ItemType element, DragItem output)
    {
        this.element = element;
        this.output = output;
    }
}

public enum ItemType
{
    RedGem = 0,
    BlueGem,
    Blood,
    Slime,
    Flower,
    Ink,

    Ring = 10,
    Necklace,
    bottle,
    Scroll,

    RedRing = 100,
    BlueRing,
    RedLace,
    BlueLace,
    BloodBottle,
    SlimeBottle,
    FlowerBottle,
    DarkScroll,

    Fail = 1000
}