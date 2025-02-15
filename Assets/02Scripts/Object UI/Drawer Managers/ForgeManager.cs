using System;
using UnityEngine;

public class ForgeManager : MonoBehaviour
{
    [SerializeField]
    private ObjectPool pool;
    [SerializeField]
    private DragItem failItem;

    [Space(5)]
    [SerializeField]
    private SlotInteract slot1;
    [SerializeField]
    private SlotInteract slot2;
    [SerializeField]
    private GameObject combineBtn;

    [Space(5)]
    [SerializeField]
    private ParticleSystem failParticle;
    [SerializeField]
    private ParticleSystem successParticle;
    [SerializeField]
    private Transform cushion;

    private void Awake()
    {
        slot1.InteractChanged += InteractChanged;
        slot2.InteractChanged += InteractChanged;

        combineBtn.SetActive(false);
        /*
        for (int i = 0; i < 150; i++)
            print(i + " => " + (ItemType)i + ", " + Enum.IsDefined(typeof(ItemType), i));
        */
    }
    private void InteractChanged()
    {
        if (slot1.curItem != null && slot2.curItem != null)
        {
            combineBtn.SetActive(true);
        }
        else
            combineBtn.SetActive(false);
    }

    public void CombineItem()
    {
        DragItem resultItem = failItem;
        foreach (Recipe recipe in slot1.curItem.itemData.recipes)
        {
            if (recipe.element == slot2.curItem.itemData.itemType)
            {

                resultItem = recipe.output;
            }
        }

        slot1.UseItem();
        slot2.UseItem();

        DragItem outputItem = pool.GetPoolObject<DragItem>(resultItem);
        ItemCount itemC = MyCalculator.GetItemCount(resultItem);
        itemC.itemCount++;
        itemC.outCount++;

        outputItem.Init(cushion.position);
    }
}
/*
 * { { ItemType.BlueGem, ItemType.BlueRace }, { ItemType.RedGem, ItemType.RedRace } },
        { { ItemType.Blood, ItemType.BloodBottle }, { ItemType.Slime, ItemType.SlimeBottle}, { ItemType.Flower, ItemType.FlowerBottle} },
        { {ItemType.Ink, ItemType.DarkScroll } }
 */