using System.Xml;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
public class ObjectTriggerButton : ObjectButtonBase
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

    [Space(5)]
    [SerializeField]
    protected UnityEvent clickEvent;

    protected SpriteRenderer sRenderer;
    protected void Awake()
    {
        sRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void ButtonClick()
    {
        PlaySound();

        clickEvent?.Invoke();

        if (normalSprite)
            sRenderer.sprite = normalSprite;
        sRenderer.color = normalColor;
    }
    protected virtual void PlaySound()
    {
        //Todo : 소리 출력
    }

    protected override void ButtonPress()
    {
        if (pressSprite)
            sRenderer.sprite = pressSprite;
        sRenderer.color = pressColor;
    }
}
