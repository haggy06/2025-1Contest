using UnityEngine;
using UnityEngine.SceneManagement;

public class SubmitManager : Singleton<SubmitManager>   
{
    public static readonly Vector2 traySize = new Vector2(3.2f, 2.2f);

    [SerializeField]
    private SlotInteract _tray;
    public SlotInteract tray => _tray;
    [SerializeField]
    private AudioClip traySound;

    [Space(5)]
    [SerializeField]
    private GameObject _order;
    public GameObject order => _order;

    [SerializeField]
    private QuestManager _questManager;
    protected new void Awake()
    {
        base.Awake();

        GameManager.Inst.Subscribe(EventType.Order, NewOrder);
        GameManager.Inst.Subscribe(EventType.DayStart, Init);
    }
    private void Init()
    {
        _tray.transform.localPosition = new Vector2(-3.9f, 0f);
        order.transform.localPosition = new Vector2(-1.75f, -depth);
    }

    private void NewOrder()
    {
        OrderMove(true);
    }

    bool isTrayMoving = false;
    public void Submit()
    {
        if (!GameManager.Inst.CanClickBell() || isTrayMoving)
            return;

        if (_tray.curItem != null)
        {
            Debug.Log(_tray.curItem.gameObject.name + " 제출");
            OrderMove(false);
            TrayMove(false);
        }
        else
        {
            Debug.Log("제출할 아이템을 올려두지 않음");
        }
    }

    [SerializeField]
    private float depth = 4f;
    [SerializeField]
    private float trayMoveTime = 0.5f;
    [SerializeField]
    private LeanTweenType trayTweenType = LeanTweenType.easeInOutBack;

    public void TrayMove(bool isUp)
    {
        AudioManager.Inst.PlaySFX(traySound);
        if (isUp)
        {
            LeanTween.moveLocalY(_tray.gameObject, 0f, trayMoveTime).setEase(trayTweenType);
        }
        else
        {
            isTrayMoving = true;
            LeanTween.moveLocalY(_tray.gameObject, -depth, trayMoveTime).setEase(trayTweenType).setOnComplete(ClearTray); // 내렸다 바로 다시 올리기
        }
    }
    private void ClearTray()
    {
        GameManager.Inst.TutorialCheck(TutorialState.Bell);
        isTrayMoving = false;

        _questManager.SubmitStart(_tray.curItem.itemData);
        _tray.InteractEnd(_tray.curItem, true);

        TrayMove(true);
    }

    public void OrderMove(bool isUp)
    {
        if (isUp)
        {
            LeanTween.moveLocalY(order.gameObject, 0f, trayMoveTime).setEase(trayTweenType);
        }
        else
        {
            LeanTween.moveLocalY(order, -depth, trayMoveTime).setEase(trayTweenType); // 내렸다 바로 다시 올리기
        }
    }


    /*
    [ContextMenu("Move Down")]
    public void MoveDOWN(bool isRemoveItem = false)
    {
        LeanTween.cancel(trayTweenID);
        float tweenTime = Mathf.Abs(depth - transform.localPosition.y) / depth * trayMoveTime;
        trayTweenID = LeanTween.moveLocalY(gameObject, -depth, tweenTime).setEase(trayTweenType).id;
        
        if (isRemoveItem)
            Invoke("RemoveItem", tweenTime);
    }
    [ContextMenu("Move Up")]
    public void MoveUP()
    {
        LeanTween.cancel(trayTweenID);
        trayTweenID = LeanTween.moveLocalY(gameObject, 0f, Mathf.Abs(transform.localPosition.y) / depth * trayMoveTime).setEase(trayTweenType).id;
    }
    */
    public void TutoCheck()
    {
        GameManager.Inst.TutorialCheck(TutorialState.OrderCheck);
    }

    private void RemoveItem()
    {
        _tray.InteractEnd(_tray.curItem, true);
    }

    protected override void SceneChanged(Scene replacedScene, Scene newScene)
    {

    }
}
