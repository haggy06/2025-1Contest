using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DrawerButton : ObjectButtonBase
{
    [SerializeField]
    protected AudioClip clickSound;

    [Space(5)]
    [SerializeField]
    protected Sprite normalSprite;
    [SerializeField]
    protected Color normalColor = Color.white;

    [Space(5)]
    [SerializeField]
    protected Sprite pressSprite;
    [SerializeField]
    protected Color pressColor = Color.white;

    [SerializeField]
    private bool isPressed = false;
    public bool IsPressed => isPressed;
    [SerializeField]
    private GameObject bangMark;

    public void BangActive(bool active)
    {
        bangMark.SetActive(active);
    }

    private DrawerManager drawerManager;
    protected SpriteRenderer sRenderer;
    private void Awake()
    {
        BangActive(false);
        sRenderer = GetComponent<SpriteRenderer>();
        drawerManager = GetComponentInParent<DrawerManager>();
    }

    public void ButtonON()
    {
        BangActive(false);
        isPressed = true;

        if (pressSprite)
            sRenderer.sprite = pressSprite;
        sRenderer.color = pressColor;
    }
    public void ButtonOFF()
    {
        isPressed = false;

        if (normalSprite)
            sRenderer.sprite = normalSprite;
        sRenderer.color = normalColor;
    }

    protected override void ButtonPress()
    {

    }

    protected override void ButtonClick()
    {
        print(gameObject.name + " 클릭");
        // Todo : 사운드 재생

        drawerManager.ButtonClick(this);
    }
}
