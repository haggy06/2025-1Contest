using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private GameObject _activeNPC;
    public GameObject activeNPC
    {
        get => _activeNPC;
        set
        {
            _activeNPC?.SetActive(false);
            _activeNPC = value;

            _activeNPC?.SetActive(true);
        }
    }

    [SerializeField]
    private TalkManager talk;

    [Space(5)]
    [SerializeField]
    private AudioClip npcSound;
    [SerializeField]
    private Animator npcs;
    [SerializeField]
    private Animator order;
    [SerializeField]
    private Animator items;
    [SerializeField]
    private GameObject getOrderButton;

    [Space(5)]
    [SerializeField]
    private ItemData[] itemArr;

    private Dictionary<ItemData, int> itemIndexDic = new Dictionary<ItemData, int>();
    private void Awake()
    {
        getOrderButton.SetActive(false);

        npcs.transform.localPosition = new Vector3(0f, -2.5f, 0f);

        for (int i = 0; i < itemArr.Length; i++)
        {
            itemIndexDic.Add(itemArr[i], i);
        }

        GameManager.Inst.Subscribe(EventType.TutorialFinish, ExitStart);
        GameManager.Inst.Subscribe(EventType.DayStart, Init);

        talk.TalkFinish += TalkFinished;
    }
    private void TalkFinished(Speaker speaker)
    {
        if (speaker == Speaker.Ingame_NPC && submitItem != null)
            GetItemStart();
    }

    private void Init()
    {
        npcs.transform.localPosition = new Vector3(0f, -2.5f, 0f);

        if (DataManager.GameData.day == 1)
        {
            npcs.transform.localPosition = Vector3.zero;
            activeNPC = npcs.transform.GetChild((int)NPCType.God).gameObject;
        }
        else
            Invoke("EnterStart", 1f);
    }

    private ItemData submitItem = null;
    public void SubmitStart(ItemData item)
    {
        if (submitItem != null)
            if (itemIndexDic.TryGetValue(submitItem, out int lastIndex))
                items.transform.GetChild(lastIndex).gameObject.SetActive(false);

        submitItem = item;
        if (itemIndexDic.TryGetValue(submitItem, out int index))
        {
            items.transform.GetChild(index).gameObject.SetActive(true);

            items.SetTrigger(fromPlayer);
        }
    }
    public void GetItemStart()
    {
        items.SetTrigger(fromNPC);
    }
    public void GetItemComplete()
    {
        itemIndexDic.TryGetValue(submitItem, out int lastIndex);
        items.transform.GetChild(lastIndex).gameObject.SetActive(false);

        submitItem = null;

        if (GameManager.Inst.tutorialState == TutorialState.None)
            ExitStart();
    }

    public void Review()
    {
        GameManager.Inst.Submit(submitItem);
        
        if (GameManager.Inst.tutorialState != TutorialState.None) // 튜토리얼 진행중일 경우
        {
            GameManager.Inst.Calculate();
            GetItemStart();
        }
        else // 튜토리얼 진행중이 아닐 경우
        {
            if (GameManager.Inst.Calculate())
            {
                talk.TalkStart(talkIndex + 1, SheetType.NPC);
            }
            else
            {
                talk.TalkStart(talkIndex + 2, SheetType.NPC);
            }
        }
    }
    public void ReviewNext()
    {
        if (talk.curTalk == null)
            return;

        if ((Speaker)talk.curTalk.speaker >= Speaker.Ingame_NPC)
        {
            talk.TalkInteract(true);
        }
    }

    int orderIndex = 0;
    int talkIndex = 0;
    public void OrderStart()
    {
        NPC npc = GameManager.Inst.curNPC;
        itemIndexDic.TryGetValue(npc.orderItem, out orderIndex);

        switch (npc.npcType)
        {
            case NPCType.God:
                break;

            case NPCType.Assassin:
                talkIndex = (int)NPCTalkIndex.Assasine;
                talk.TalkStart(talkIndex, SheetType.NPC);
                break;

            case NPCType.Regular:
                talkIndex = DataManager.GameData.day < 2 ? (int)NPCTalkIndex.Regular_First : (int)NPCTalkIndex.Regular_Second;
                talk.TalkStart(talkIndex, SheetType.NPC);
                break;
            default:
                talkIndex = Random.Range(0, 3) * 3;
                talk.TalkStart(talkIndex, SheetType.NPC);
                break;
        }

        order.SetTrigger(fromNPC);
    }
    public void OrderStart(ItemData itemIndex)
    {
        itemIndexDic.TryGetValue(itemIndex, out orderIndex);

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

    public void ExitStart()
    {
        AudioManager.Inst.PlaySFX(npcSound);
        npcs.SetTrigger(fromNPC);
    }
    public void ExitComplete()
    {
        if (GameManager.Inst.NextNPC())
        {
            EnterStart();
        }
    }
    public void EnterStart()
    {
        activeNPC = npcs.transform.GetChild(GameManager.Inst.curNPC.faceIndex).gameObject;

        AudioManager.Inst.PlaySFX(npcSound);
        npcs.SetTrigger(fromPlayer);
    }
    public void EnterComplete()
    {
        OrderStart();
    }

    public void AcceptComplete()
    {
        GameManager.Inst.SetOrderItem(itemArr[orderIndex]);
    }

    private static readonly int fromPlayer = Animator.StringToHash("FromPlayer");
    private static readonly int fromNPC = Animator.StringToHash("FromNPC");
}
