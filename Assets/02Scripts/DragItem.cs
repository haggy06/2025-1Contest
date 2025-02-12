using System.Drawing;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;
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

    private Rigidbody2D rigid2D;
    private SpriteRenderer sRenderer;

    private void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
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
    private void OnMouseUp()
    {
        if (Active)
            if (isHolding) // 들고 있었을 때만 DragEnd 호출
            DragEnd();
    }
    private void OnMouseDrag()
    {
        if (Active)
            if (isHolding) // 들고 있을 때만 Dragging 호출
            Dragging();
    }

    private Vector2 dragStartVec = Vector2.zero;
    private Vector2 myStartPos = Vector2.zero;
    public void Dragging()
    {
        Vector2 goalPosition = MyCalculator.GetMousePosition() - dragStartVec + myStartPos;
        goalPosition.x = Mathf.Clamp(goalPosition.x, minMoveVec.x, maxMoveVec.x);
        goalPosition.y = Mathf.Clamp(goalPosition.y, minMoveVec.y, maxMoveVec.y);

        rigid2D.MovePosition(goalPosition);
    }

    public void DragStart()
    {
        rigid2D.bodyType = RigidbodyType2D.Dynamic;
        InteractExit();

        dragStartPos = transform.position;
        dragStartRot = transform.eulerAngles.z;

        isHolding = true;
        isOnDrawer = false;
        transform.parent = null;

        sRenderer.sortingOrder = 51;
        gameObject.layer = 7;

        myStartPos = transform.position;
        dragStartVec = MyCalculator.GetMousePosition();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void InteractExit()
    {
        if (curInteract != null)
        {
            curInteract.InteractEnd(this);
            curInteract = null;
        }
    }

    Collider2D[] cols = new Collider2D[3];
    IItemInteractable curInteract = null;
    public void DragEnd()
    {
        isHolding = false;

        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = 1 << 10;
        filter.useTriggers = true;

        for (int i = 0; i < cols.Length; i++)
        {
            cols[i] = null;
        }

        bool isShouldUndo = false;
        if (rigid2D.GetContacts(filter, cols) > 0)
        {
            foreach(Collider2D col in cols)
            {
                if (col == null)
                    break;

                if (col.TryGetComponent<IItemInteractable>(out var interact))
                {
                    print(col.name + "와 상호작용 시도");
                    if (interact.CanInteract)
                    {
                        isShouldUndo = false;

                        rigid2D.angularVelocity = 0f;
                        rigid2D.bodyType = RigidbodyType2D.Kinematic;
                        curInteract = interact;
                        interact.InteractStart(this);

                        break;
                    }
                    else
                    {
                        isShouldUndo = true;
                    }
                }
            }
        }
        if (isShouldUndo)
            UndoDrag();

        SetLayer();
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

    public void UndoDrag()
    {
        transform.position = dragStartPos;
        transform.eulerAngles = Vector3.forward * dragStartRot;
    }
}
