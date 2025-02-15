using System.Collections;
using System.Drawing;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;
using static UnityEngine.RuleTile.TilingRuleOutput;

[RequireComponent (typeof(PolygonCollider2D), typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class DragItem : PoolObject, IDragable
{
    [field: SerializeField]
    public bool Active { get; set; }
    [SerializeField]
    private ItemData _itemData;
    public ItemData itemData => _itemData;

    private static Vector2 minMoveVec = new Vector2(-5.25f, -4.55f);
    private static Vector2 maxMoveVec = new Vector2(5.25f, 2.85f);

    private Rigidbody2D _rigid2D;
    public Rigidbody2D rigid2D => _rigid2D;

    private SpriteRenderer sRenderer;

    private void Awake()
    {
        _rigid2D = GetComponent<Rigidbody2D>();
        sRenderer = GetComponent<SpriteRenderer>();
    }
    public override void Init(Vector2 position, float angle = 0)
    {
        base.Init(position, angle);
        SetLayer();
    }

    private bool isHolding = false;
    private bool isOnDrawer = false;
    private Vector3 dragStartPos;
    private float dragStartRot;
    private void OnMouseDown()
    {
        if (Active)
            if (isOnDrawer || !IsCollapseToTable()) // 테이블에 깔려있지 않을 때만 DragStart 호출
                DragStart();
    }
    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (Active)
                if (isHolding) // 들고 있었을 때만 DragEnd 호출
                    DragEnd();
        }
    }
    /*
    private void OnMouseUp() // 인벤토리로 소환했을 때 안 놔지는 문제
    {
        if (Active)
            if (isHolding) // 들고 있었을 때만 DragEnd 호출
                DragEnd();
    }
    */
    /*
    private void OnMouseDrag() // 인벤토리로 소환했을 때 바로 안 집히는 문제
    {
        if (Active)
            if (isHolding) // 들고 있을 때만 Dragging 호출
                Dragging();
    }
    */

    private Vector2 dragStartVec = Vector2.zero;
    private Vector2 myStartPos = Vector2.zero;
    public void Dragging()
    {
        Vector2 goalPosition = MyCalculator.GetMousePosition() - dragStartVec + myStartPos;
        goalPosition.x = Mathf.Clamp(goalPosition.x, minMoveVec.x, maxMoveVec.x);
        goalPosition.y = Mathf.Clamp(goalPosition.y, minMoveVec.y, maxMoveVec.y);

        _rigid2D.MovePosition(goalPosition);
    }

    public void DragStart()
    {
        _rigid2D.bodyType = RigidbodyType2D.Dynamic;
        InteractExit();

        dragStartPos = transform.position;
        dragStartRot = transform.eulerAngles.z;

        isOnDrawer = false;
        transform.parent = null;

        sRenderer.sortingOrder = 51;
        gameObject.layer = 7;

        myStartPos = transform.position;
        dragStartVec = MyCalculator.GetMousePosition();

        isHolding = true;
        StartCoroutine("DragCor");
    }
    private IEnumerator DragCor()
    {
        while (isHolding)
        {
            if (Active)
                if (isHolding) // 들고 있을 때만 Dragging 호출
                    Dragging();

            yield return null;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void InteractExit()
    {
        if (curInteract != null)
        {
            curInteract.InteractEnd(this);
            // curInteract = null; // 혹시 돌아올 수도 있으므로 curInteract 초기화는 좀 있다가 하기
        }
    }

    private Collider2D[] cols = new Collider2D[3];
    private IItemInteractable curInteract = null;
    public void SetCurInteractArbitrarily(IItemInteractable interactable)
    {
        curInteract = interactable;
    }

    public void DragEnd()
    {
        isHolding = false;
        StopCoroutine("DragCor");

        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = 1 << 10;
        filter.useTriggers = true;

        for (int i = 0; i < cols.Length; i++)
        {
            cols[i] = null;
        }

        IItemInteractable lastInteract = curInteract;
        curInteract = null;

        bool isShouldUndo = false;
        if (_rigid2D.GetContacts(filter, cols) > 0)
        {
            foreach(Collider2D col in cols)
            {
                if (col == null)
                    break;

                if (col.TryGetComponent<IItemInteractable>(out var interact))
                {
                    print(col.name + "와 상호작용 시도");
                    if (interact.InteractStart(this))
                    {
                        isShouldUndo = false;

                        curInteract = interact;

                        break;
                    }
                    else
                    {
                        isShouldUndo = true;
                    }
                }
            }
        }

        if (isShouldUndo) // 되돌아가야 할 경우
        {
            if (lastInteract != null)
            {
                curInteract = lastInteract;
                curInteract.InteractStart(this);
            }

            UndoDrag();
        }

        SetLayer();
    }

    public void RemoveVelocity(bool isKinematic = false)
    {
        rigid2D.linearVelocity = Vector2.zero;
        rigid2D.angularVelocity = 0f;

        if (isKinematic)
            rigid2D.bodyType = RigidbodyType2D.Kinematic;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetLayer()
    {
        if (IsCollapseToTable()) // 테이블 위에 올려놨을 경우
        {
            sRenderer.sortingOrder = 50;
            gameObject.layer = 8;
            transform.position = new Vector3(transform.position.x, transform.position.y, -0.5f);

            isOnDrawer = true;
            transform.parent = DrawerManager.Inst.GetCurDrawer().transform;
        }
        else if (IsCollapseToTray()) // 트레이 위에 올려놨을 경우
        {
            sRenderer.sortingOrder = 50;
            gameObject.layer = 8;
            transform.position = new Vector3(transform.position.x, transform.position.y, -0.5f);

            isOnDrawer = true;
            transform.parent = SubmitManager.Inst.tray.transform;
        }
        else
        {
            sRenderer.sortingOrder = 0;
            gameObject.layer = 6;
            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        }
    }

    private bool IsCollapseToTable()
    {
        Vector2 drawerPosition = DrawerManager.Inst.Drawer.position;

        /*
        print(drawerPosition.x - (DrawerManager.drawerSize.x / 2f) + " < " + transform.position.x + " < " + drawerPosition.x + (DrawerManager.drawerSize.x / 2f));
        print(drawerPosition.y + " < " + transform.position.y + " < " + drawerPosition.y + DrawerManager.drawerSize.y);
        print((drawerPosition.x - (DrawerManager.drawerSize.x / 2f) < transform.position.x && transform.position.x < drawerPosition.x + (DrawerManager.drawerSize.x / 2f)) &&
            (drawerPosition.y < transform.position.y && transform.position.y < drawerPosition.y + DrawerManager.drawerSize.y));
        */

        return (drawerPosition.x - (DrawerManager.drawerSize.x / 2f) < transform.position.x && transform.position.x < drawerPosition.x + (DrawerManager.drawerSize.x / 2f)) &&
            (drawerPosition.y < transform.position.y && transform.position.y < drawerPosition.y + DrawerManager.drawerSize.y);
    }
    private bool IsCollapseToTray()
    {
        Vector2 trayPosition = SubmitManager.Inst.tray.transform.position;

        return (trayPosition.x - (SubmitManager.traySize.x / 2f) < transform.position.x && transform.position.x < trayPosition.x + (SubmitManager.traySize.x / 2f)) &&
            (trayPosition.y - (SubmitManager.traySize.y / 2f) < transform.position.y && transform.position.y < trayPosition.y + (SubmitManager.traySize.y / 2f));
    }

    public void UndoDrag()
    {
        transform.position = dragStartPos;
        transform.eulerAngles = Vector3.forward * dragStartRot;

        RemoveVelocity();
    }

    [ContextMenu("Test")]
    public void ItemDicSaveTest()
    {
        DataManager.SaveGameData();
    }
}
