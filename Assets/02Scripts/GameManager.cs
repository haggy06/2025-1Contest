using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public static readonly float dangerMaginot = 0.775f;
    public const int happyEndMoney = 25000;

    protected override void SceneChanged(Scene replacedScene, Scene newScene)
    {
        CameraResolutionLock.SetResolution(16f, 9f);

        if (newScene.buildIndex <= 1)
        {
            DataManager.RefreshGameData();
            for (int i = 0; i < PlayEventArr.Length; i++) // �̺�Ʈ �ʱ�ȭ
            {
                PlayEventArr[i] = null;
            }
        }
        else if (newScene.buildIndex == 2)
        {
            spyInformed = false;

            RerollNPC();
            DayStartInvoke();
        }
    }
    private void FixedUpdate()
    {
        if (!CameraResolutionLock.CheckResolution())
        {
            CameraResolutionLock.SetResolution(16f, 9f);
        }
    }

    #region _About Tutorial_
    [SerializeField]
    private TutorialState _tutorialState = TutorialState.None;
    public TutorialState tutorialState => _tutorialState;

    [SerializeField]
    private ItemData _tutorialItem;
    public ItemData tutorialItem => _tutorialItem;
    public void TutorialStart()
    {
        _tutorialState = TutorialState.OrderGet;
        EventInvoke(EventType.TutorialStart);
    }
    public void TutorialChange(TutorialState newState)
    {
        print("TutorialChange");
        _tutorialState = newState;
        EventInvoke(EventType.TutorialNext);
    }
    public void TutorialClear()
    {
        _tutorialState = TutorialState.None;
        EventInvoke(EventType.TutorialFinish);
    }
    public bool CanClickDrawer(int drawerIndex)
    {
        if (TutorialEquals(TutorialState.None))
            return true;

        bool isCorrectDrawer = false;
        switch (drawerIndex)
        {
            case 0: // ���� ��ư
                isCorrectDrawer = TutorialEquals(TutorialState.GoForge);
                break;

            case 1: // ���� ��ư
                isCorrectDrawer = TutorialEquals(TutorialState.GoShop);
                break;

            case 2: // ���� ��ư
                isCorrectDrawer = TutorialEquals(TutorialState.GoInfo);
                break;

            case 3: // �κ��丮 ��ư
                isCorrectDrawer = TutorialEquals(TutorialState.GoInventory);
                break;

            default: // �̿ܿ� �� false
                return false;
        }

        if (isCorrectDrawer)
            EventInvoke(EventType.TutorialClear);

        return isCorrectDrawer;
    }
    public bool CanClickBell()
    {
        return _orderItem != null && (TutorialEquals(TutorialState.None) || TutorialEquals(TutorialState.Bell)); // �ֹ��� ���� ������ Ʃ�丮���� �ƴ�(or �� Ʃ��) ��� true
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TutorialEquals(TutorialState state)
    {
        return _tutorialState.Equals(state);
    }
    public bool TutorialCheck(TutorialState state)
    {
        if (TutorialEquals(state))
        {
            print(state + " ?= " + _tutorialState);
            EventInvoke(EventType.TutorialClear);
            return true;
        }
        else
            return false;
    }
    #endregion

    private ItemData _orderItem = null;
    public ItemData orderItem => _orderItem;

    private ItemData _submitItem = null;
    public ItemData submitItem => _submitItem;
    public void SetOrderItem(ItemData newOrder)
    {
        _orderItem = newOrder;
        _submitItem = null;

        EventInvoke(EventType.Order);

        TutorialCheck(TutorialState.OrderGet);
    }
    public void Submit(ItemData submitItem)
    {
        _submitItem = submitItem;
        print(submitItem);
        EventInvoke(EventType.Submit);
    }

    private float successDanger = 0.05f;
    private StoryTalkIndex _endingType;
    public StoryTalkIndex endingType => _endingType;
    public bool Calculate()
    {
        if (_orderItem == _submitItem)
        {
            DataManager.GameData.money += _orderItem.price;

            float dangerPercent = DataManager.GameData.danger + successDanger;
            if (dangerPercent > 1f || Mathf.Approximately(dangerPercent, 1f))
            {
                DataManager.GameData.danger = 1f;
                IngameCanvasManager.RefreshDanger();

                //_endingType = StoryTalkIndex.TrapEnd;
                //EventInvoke(EventType.End);
            }
            else
            {
                DataManager.GameData.danger = dangerPercent;
                IngameCanvasManager.RefreshDanger();
            }

            IngameCanvasManager.RefreshMoney();

            _orderItem = null;

            if (curNPC.npcType == NPCType.Spy) // �������� ����Ʈ���� ���
                DataManager.GameData.isSpyOwnered = true;

            return true;
        }
        else
        {
            int curMoney = DataManager.GameData.money - (orderItem.price / 2);
            if (curMoney >= 0)
            {
                DataManager.GameData.money = curMoney;
                IngameCanvasManager.RefreshMoney();
            }
            else
            {
                _endingType = StoryTalkIndex.BankruptcyEnd;
                EventInvoke(EventType.End);
            }

            DataManager.GameData.danger = Mathf.Clamp(DataManager.GameData.danger - successDanger, 0f, 1f);
            IngameCanvasManager.RefreshDanger();

            _orderItem = null;
            return false;
        }
    }

    private bool spyInformed = false;
    public void InfoCrystal()
    {
        if (DataManager.GameData.money >= 800) // ���� ����� ���
        {
            DataManager.GameData.money -= 800;
            IngameCanvasManager.RefreshMoney();
        }
        else
            return;

        if (tutorialState != TutorialState.None)
        {
            TutorialCheck(TutorialState.GetInfo);
            return;
        }

        if (DataManager.GameData.day == 3 && !spyInformed) // ������ �� �������� ��� & ���� ������ ��⸦ �� ����� ���
        {
            spyInformed = true;
            TalkManager.Inst.TalkStart(1, SheetType.Info);
        }
        else if (DataManager.GameData.money <= 1000 && !DataManager.GameData.getFunded) // �Ļ� ������ ��
        {
            DataManager.GameData.getFunded = true;
            TalkManager.Inst.TalkStart(8, SheetType.Info);
        }        
        else if (DataManager.GameData.todayBuyCount > 5) // ��� 5�� �̻� �Ҹ� ��(���� �ִ� 13��)
        {
            TalkManager.Inst.TalkStart(6, SheetType.Info);
        }
        else
        {
            TalkManager.Inst.TalkStart(0, SheetType.Info);
        }
    }

    [SerializeField]
    private NPC[] npcArr = new NPC[4];
    private int curIndex = 0;

    private NPC _curNPC;
    public NPC curNPC
    {
        get
        {
            if (_curNPC == null)
            {
                _curNPC = npcArr[0];
            }
            return _curNPC;
        }
    }
    public bool NextNPC()
    {
        if (++curIndex < npcArr.Length) // �մ��� ������ ���
        {
            _curNPC = npcArr[curIndex];

            return true;
        }
        else
        {
            EventInvoke(EventType.DayFinish);

            DataManager.GameData.day++;
            curIndex = 0;
            RerollNPC();

            DataManager.GameData.ResetOutCount();
            DataManager.SaveGameData();

            return false;
        }
    }
    private void RerollNPC()
    {
        switch (DataManager.GameData.day)
        {
            case 1: // �ܰ� ���� ��
                for (int i = 0; i < npcArr.Length; i++)
                {
                    if (i == 3)
                    {
                        npcArr[i].npcType = NPCType.Regular;
                        npcArr[i].faceIndex = (int)NPCType.Regular;
                    }
                    else
                    {
                        npcArr[i].npcType = NPCType.Normal;
                        npcArr[i].RandomFace();
                    }
                    npcArr[i].RandomOrder();
                }
                break;
            case 2: // ������ ���� ��
                int spyIndex = UnityEngine.Random.Range(0, npcArr.Length);
                for (int i = 0; i < npcArr.Length; i++)
                {
                    if (i == spyIndex)
                    {
                        npcArr[i].npcType = NPCType.Spy;
                        npcArr[i].faceIndex = DataManager.GameData.spyNPC.faceIndex;
                        npcArr[i]._orderItemIndex = DataManager.GameData.spyNPC._orderItemIndex;
                    }
                    else
                    {
                        npcArr[i].npcType = NPCType.Normal;
                        npcArr[i].RandomOrder();
                        npcArr[i].RandomFace();
                    }
                }
                break;
            case 3: // �ڰ� ���� ��
                for (int i = 0; i < npcArr.Length; i++)
                {
                    if (i == 3)
                    {
                        npcArr[i].npcType = NPCType.Assassin;
                        npcArr[i].faceIndex = (int)NPCType.Assassin;
                    }
                    else
                    {
                        npcArr[i].npcType = NPCType.Normal;
                        npcArr[i].RandomFace();
                    }
                    npcArr[i].RandomOrder();
                }
                break;
            case 4: // �ܰ� ���� ��
                for (int i = 0; i < npcArr.Length; i++)
                {
                    if (i == 1)
                    {
                        npcArr[i].npcType = NPCType.Regular;
                        npcArr[i].faceIndex = (int)NPCType.Regular;
                    }
                    else
                    {
                        npcArr[i].npcType = NPCType.Normal;
                        npcArr[i].RandomFace();
                    }
                    npcArr[i].RandomOrder();
                }
                break;
            case 5: // �ڰ� �� �� �ִ� ��
                for (int i = 0; i < npcArr.Length; i++)
                {
                    if (i == 3 && DataManager.GameData.DangerBySpy()) // �����̰� ������ ���
                    {
                        npcArr[i].npcType = NPCType.Assassin;
                        npcArr[i].faceIndex = (int)NPCType.Assassin;
                    }
                    else
                    {
                        npcArr[i].npcType = NPCType.Normal;
                        npcArr[i].RandomFace();
                    }
                    npcArr[i].RandomOrder();
                }
                break;
            default:
                for (int i = 0; i < npcArr.Length; i++)
                {
                    npcArr[i].npcType = NPCType.Normal;
                    npcArr[i].RandomFace();
                    npcArr[i].RandomOrder();
                }
                break;
        }

        curIndex = 0;
        _curNPC = npcArr[curIndex];
    }

    private UnityAction[] PlayEventArr = new UnityAction[Enum.GetValues(typeof(EventType)).Length];
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EventInvoke(EventType eventType)
    {
        PlayEventArr[(int)eventType]?.Invoke();
    }

    public void DayStartInvoke()
    {
        if (DataManager.GameData.day >= 6) // ��������
        {
            if (DataManager.GameData.DangerBySpy() || DataManager.GameData.danger >= dangerMaginot) // ������ �̽��� �ذ���� �ʾҰų� ���赵�� ���ġ �̻��� ���
            {
                _endingType = StoryTalkIndex.TrapEnd;
                EventInvoke(EventType.End);
            }
            else if (DataManager.GameData.money < happyEndMoney) // ���� ���ڶ� ���
            {
                _endingType = StoryTalkIndex.NormalEnd;
                EventInvoke(EventType.End);
            }
            else
            {
                _endingType = StoryTalkIndex.HappyEnd;
                EventInvoke(EventType.End);
            }
        }
        else
        {
            EventInvoke(EventType.DayStart);
        }
    }
    public void Subscribe(EventType type, UnityAction listener)
    {
        PlayEventArr[(int)type] += listener;
    }
    public void Unsubscribe(EventType type, UnityAction listener)
    {
        PlayEventArr[(int)type] -= listener;
    }
}

public enum EventType
{
    TutorialStart,
    TutorialNext,
    TutorialClear,
    TutorialFinish,

    Submit,
    Order,

    DayStart,
    DayFinish,

    End
}
public enum TutorialState
{
    None,

    JustTalk,

    OrderGet,
    OrderCheck,

    GoShop,
    Shopping,

    GoInventory,
    PullOutItem,

    GoForge,
    PutItem,
    Combine,

    Bell,

    GoInfo,
    GetInfo,
}