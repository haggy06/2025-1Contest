using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

[RequireComponent (typeof(PolygonCollider2D), typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class DragItem : MonoBehaviour, IDragable
{
    [field: SerializeField]
    public bool Active { get; set; }


    private static Vector2 minMoveVec = new Vector2(-5.25f, -4.55f);
    private static Vector2 maxMoveVec = new Vector2(5.25f, 2.85f);

    private Rigidbody2D rigid2D;
    private SpriteRenderer sRenderer;
    private void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        sRenderer = GetComponent<SpriteRenderer>();
    }

    private bool isHolding = false;
    private bool isOnDrawer = false;
    private void OnMouseDown()
    {
        if (Active)
            if (isOnDrawer || !IsCollapseToTable()) // ���̺� ������� ���� ���� DragStart ȣ��
            DragStart();
    }
    private void OnMouseUp()
    {
        if (Active)
            if (isHolding) // ��� �־��� ���� DragEnd ȣ��
            DragEnd();
    }
    private void OnMouseDrag()
    {
        if (Active)
            if (isHolding) // ��� ���� ���� Dragging ȣ��
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
        isHolding = true;
        isOnDrawer = false;
        transform.parent = null;

        sRenderer.sortingOrder = 51;
        gameObject.layer = 7;

        myStartPos = transform.position;
        dragStartVec = MyCalculator.GetMousePosition();
    }

    public void DragEnd()
    {
        isHolding = false;

        if (IsCollapseToTable()) // ���̺� ���� �÷����� ���
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
}
