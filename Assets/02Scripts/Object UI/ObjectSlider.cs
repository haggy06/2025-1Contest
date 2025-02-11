using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Unity.Collections.LowLevel.Unsafe;

[RequireComponent(typeof(SpriteRenderer))]
public class ObjectSlider : MonoBehaviour, IDragable
{
    [field: SerializeField]
    public bool Active { get; set; } = true;

    [SerializeField]
    private SpriteRenderer barBackground;
    [SerializeField]
    private SpriteRenderer fillBar;
    [SerializeField]
    private SpriteRenderer button;

    [Header("Slider Button Setting")]
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

    [Header("Slider Setting")]
    [SerializeField, Tooltip("true = 세로 슬라이더, false = 가로 슬라이더")]
    private bool vertical = false;
    [SerializeField]
    private int minValue = 0;
    [SerializeField]
    private int maxValue = 1;
    [SerializeField]
    private bool wholeNumbers;

    [Space(10)]
    [field: SerializeField]
    private float _value;
    public float value 
    { 
        get => _value;
        set
        {
            _value = value;

            float percent = (_value - minValue) / (maxValue - minValue);

            transform.localPosition = new Vector2(percent * (barBackground.size.x - button.size.x), 0f);
            fillBar.size = new Vector2(percent * barBackground.size.x, 1f);

            onValueChanged?.Invoke(_value);
        }
    }

    private void OnMouseDown()
    {
        if (Active)
        {
            // todo: clickSound 재생
            DragStart();
        }
    }
    private void OnMouseDrag()
    {
        if (Active)
            Dragging();
    }
    private void OnMouseUp()
    {
        if (Active)
            DragEnd();
    }

    public void DragStart()
    {
        if (pressSprite)
            button.sprite = pressSprite;
        button.color = pressColor;
    }

    public void Dragging()
    {
        float valueRange = maxValue - minValue;
        float percent = 0f;
        if (vertical)
            percent = Mathf.Clamp(MyCalculator.GetMousePosition().y - barBackground.transform.position.y, 0f, barBackground.size.x) / barBackground.size.x; // 아래 >> 위
        else
            percent = Mathf.Clamp(MyCalculator.GetMousePosition().x - barBackground.transform.position.x, 0f, barBackground.size.x) / barBackground.size.x; // 왼쪽 >> 오른쪽

        if (wholeNumbers)
            percent = Mathf.Round(percent * valueRange) / valueRange;

        float newValue = percent * valueRange + minValue;
        if (!Mathf.Approximately(value, newValue)) // 값이 변했을 경우
        {
            value = newValue;
        }
    }
    [SerializeField]
    public Slider.SliderEvent onValueChanged;

    public void DragEnd()
    {
        if (normalSprite)
            button.sprite = normalSprite;
        button.color = normalColor;
    }
}
