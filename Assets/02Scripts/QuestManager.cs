using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField]
    private Transform npcs;
    [SerializeField]
    private Animator order;
    [SerializeField]
    private Animator items;
    [SerializeField]
    private GameObject getOrderButton;

    [Space(5)]
    [SerializeField]
    private ItemData[] itemArr;
    private void Awake()
    {
        getOrderButton.SetActive(false);
        GameManager.Inst.Subscribe(EventType.Submit, SubmitStart);
    }
    
    private void SubmitStart()
    {
        for (int i = 0; i < itemArr.Length; i++)
        {
            items.transform.GetChild(i).gameObject.SetActive(itemArr[i] == GameManager.Inst.submitItem);

            items.SetTrigger(fromPlayer);
        }
    }
    public void SubmitComplete()
    {
        items.SetTrigger(fromNPC);
    }

    int orderIndex = 0;
    public void OrderStart()
    {
        orderIndex = Random.Range(0, itemArr.Length) - 1;
        order.SetTrigger(fromNPC);
    }
    public void OrderStart(int itemIndex)
    {
        orderIndex = itemIndex;

        order.SetTrigger(fromNPC);
    }
    public void OrderComplete()
    {
        getOrderButton.SetActive(true);
    }

    public void AcceptStart()
    {
        getOrderButton.SetActive(false);

        order.SetTrigger(fromPlayer);
    }
    public void AcceptComplete()
    {
        GameManager.Inst.SetOrderItem(itemArr[orderIndex]);
    }

    private static readonly int fromPlayer = Animator.StringToHash("FromPlayer");
    private static readonly int fromNPC = Animator.StringToHash("FromNPC");
}
