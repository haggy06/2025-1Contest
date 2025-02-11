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

    private DrawerManager drawerManager;
    protected SpriteRenderer sRenderer;
    private void Awake()
    {
        sRenderer = GetComponent<SpriteRenderer>();
        drawerManager = GetComponentInParent<DrawerManager>();
    }

    public void ButtonON()
    {
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
        print(gameObject.name + " Ŭ��");
        // Todo : ���� ���

        drawerManager.ButtonClick(this);
    }
}
