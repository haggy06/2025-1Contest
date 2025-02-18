using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    protected override void SceneChanged(Scene replacedScene, Scene newScene)
    {
        CameraResolutionLock.SetResolution(16f, 9f);
    }
    private void FixedUpdate()
    {
        if (!CameraResolutionLock.CheckResolution())
        {
            CameraResolutionLock.SetResolution(16f, 9f);
        }
    }

    [SerializeField]
    private TutorialState _tutorialState = TutorialState.None;
    public TutorialState tutorialState => _tutorialState;

    [SerializeField]
    private DragItem[] tutorialItemArr;
    public void ChangeTutorial(TutorialState newState)
    {
        _tutorialState = newState;
    }
    public void TutorialClear()
    {
        _tutorialState = TutorialState.None;
    }
    
    public bool CanBuyItem(DragItem item)
    {
        if (_tutorialState != TutorialState.Shopping)
            return true;

        foreach (var buyableItem in tutorialItemArr)
        {
            if (item == buyableItem)
            {
                return true;
            }
        }
        return false;
    }
    public bool CanClickDrawer(int drawerIndex)
    {
        if (_tutorialState == TutorialState.None)
            return true;

        switch (drawerIndex)
        {
            case 0: // 조합 버튼
                EventInvoke(EventType.NextTutorial);
                return _tutorialState == TutorialState.GoForge;

            case 1: // 상점 버튼
                EventInvoke(EventType.NextTutorial);
                return _tutorialState == TutorialState.GoShop;

            case 2: // 정보 버튼
                EventInvoke(EventType.NextTutorial);
                return _tutorialState == TutorialState.GoInfo;

            case 3: // 인벤토리 버튼
                EventInvoke(EventType.NextTutorial);
                return _tutorialState == TutorialState.GoInventory;

            default: // 이외엔 다 false
                return false;
        }
    }

    private ItemData _orderItem = null;
    public ItemData orderItem => _orderItem;

    private ItemData _submitItem = null;
    public ItemData submitItem => _submitItem;
    public void SetOrderItem(ItemData newOrder)
    {
        _orderItem = newOrder;
        _submitItem = null;

        EventInvoke(EventType.Order);
    }

    public bool CanClickBell()
    {
        if (_tutorialState == TutorialState.None)
            return _orderItem != null;
        else
        {
            return _tutorialState == TutorialState.Bell;
        }
    }
    public void Submit(ItemData submitItem)
    {
        _submitItem = submitItem;
        EventInvoke(EventType.Submit);
    }

    private UnityAction[] PlayEventArr = new UnityAction[Enum.GetValues(typeof(EventType)).Length];
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EventInvoke(EventType eventType)
    {
        PlayEventArr[(int)eventType]?.Invoke();
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
    NextTutorial,
    TutorialClear,

    Submit,
    Order,

    DayFinish,
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

    Submit,
    Bell,

    GoInfo,
    GetInfo,
}