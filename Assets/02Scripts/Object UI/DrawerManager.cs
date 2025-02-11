using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DrawerManager : Singleton<DrawerManager>
{
    #region _Static Elements_
    public static Vector2 drawerSize = new Vector2(6.6f, 6f);
    #endregion

    [SerializeField]
    private Transform drawer;
    public Transform Drawer => drawer;

    [SerializeField]
    private GameObject[] drawerObjects;
    private Dictionary<DrawerButton, int> drawerButtons = new Dictionary<DrawerButton, int>();

    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f,0f,0f,0.5f);
        Gizmos.DrawCube(drawer.position + new Vector3(0f, drawerSize.y / 2f, 0f), drawerSize);
    }
    */

    protected new void Awake()
    {
        base.Awake();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent<DrawerButton>(out DrawerButton btn))
                drawerButtons.Add(btn, i);
            else
                Debug.LogError("DrawerButton이 없는 오브젝트가 자식으로 있음.");
        }

        Init();
    }
    public void Init()
    {
        drawer.localPosition = Vector3.zero;
        foreach (var obj in drawerObjects)
        {
            obj.SetActive(false);
        }
    }

    private bool isOpen = false;
    private int curDrawerIndex = 0;

    private int drawerTweenID = 0;
    private void DrawerClose()
    {
        LeanTween.cancel(drawerTweenID);

        drawerTweenID = LeanTween.moveLocalY(drawer.gameObject, 0, Mathf.Abs(drawer.localPosition.y / drawerDepth) * drawerTime).setEase(drawerType).setOnComplete(() => isOpen = false).id;
    }
    private void DrawerClose(int nextDrawerIndex)
    {
        LeanTween.cancel(drawerTweenID);

        drawerTweenID = LeanTween.moveLocalY(drawer.gameObject, 0, Mathf.Abs(drawer.localPosition.y / drawerDepth) * drawerTime).setEase(drawerType).setOnComplete(() => DrawerOpen(nextDrawerIndex)).id;
    }

    private void DrawerOpen(int drawerIndex)
    {
        LeanTween.cancel(drawerTweenID);

        isOpen = true;

        GetCurDrawer().SetActive(false);

        curDrawerIndex = drawerIndex;
        GetCurDrawer().SetActive(true);

        drawerTweenID = LeanTween.moveLocalY(drawer.gameObject, -drawerDepth, (1f - Mathf.Abs(drawer.localPosition.y / drawerDepth)) * drawerTime).setEase(drawerType).id;
    }

    public GameObject GetCurDrawer()
    {
        return drawerObjects[curDrawerIndex];
    }

    [Header("Drawer Setting")]
    [SerializeField, Tooltip("서랍을 여닫는 시간")]
    private float drawerTime = 0.5f;
    [SerializeField, Tooltip("서랍을 여닫는 방법")]
    private LeanTweenType drawerType = LeanTweenType.linear;
    [SerializeField, Tooltip("서랍의 깊이")]
    private float drawerDepth = 6f;
    public void ButtonClick(DrawerButton button)
    {
        foreach(var btn in drawerButtons) // 지금 눌린 버튼을 제외한 모든 버튼 올리기
        {
            if (btn.Key != button)
                btn.Key.ButtonOFF();
        }
        if (drawerButtons.TryGetValue(button, out int buttonIndex))
        {
            if (isOpen) // 이미 열려 있었을 경우
            {
                if (buttonIndex == curDrawerIndex) // 열려 있던 서랍이 지금 누른 버튼과 일치할 경우
                {
                    DrawerClose();
                    button.ButtonOFF();
                }
                else // 일치하지 않을 경우
                {
                    DrawerClose(buttonIndex);
                    button.ButtonON();
                }
            }
            else // 닫혀 있었을 경우
            {
                DrawerOpen(buttonIndex);
                button.ButtonON();
            }
        }
    }

    protected override void SceneChanged(Scene replacedScene, Scene newScene)
    {

    }
}
