using System.Collections;
using UnityEngine;

public class InfoCrystal : MonoBehaviour, IDragable
{
    [field: SerializeField]
    public bool Active { get; set; } = true;

    [SerializeField]
    private float ClickTime = 2f;
    [SerializeField]
    private SpriteRenderer slider;
    [SerializeField]
    private SpriteRenderer fill;

    private AudioSource speaker;
    private void Awake()
    {
        slider.gameObject.SetActive(false);
        speaker = GetComponent<AudioSource>();
    }

    private void OnMouseDown()
    {
        if (TalkManager.Inst.curTalk == null || GameManager.Inst.tutorialState == TutorialState.GetInfo) // 현재 대화가 없거나 튜토리얼 진행중일 경우
        {
            DragStart();
        }
    }
    private void OnMouseUp()
    {
        DragEnd();
    }

    public void DragEnd()
    {
        slider.gameObject.SetActive(false);
        speaker.Stop();
        StopCoroutine("ClickCor");
    }

    public void Dragging()
    {

    }

    public void DragStart()
    {
        speaker.Play();
        StartCoroutine("ClickCor");
    }

    private IEnumerator ClickCor()
    {
        Vector2 fillSize = new Vector2(0f, 0.36f);
        slider.gameObject.SetActive(true);
        fill.size = fillSize;

        float progress = 0f;
        while (progress < ClickTime)
        {
            progress += Time.deltaTime;

            fillSize.x = progress * slider.size.x / ClickTime;
            fill.size = fillSize;

            yield return null;
        }

        slider.gameObject.SetActive(false);
        if (TalkManager.Inst.curTalk == null || GameManager.Inst.tutorialState == TutorialState.GetInfo) // 현재 대화가 없거나 튜토리얼 진행중일 경우
        {
            GameManager.Inst.InfoCrystal();
        }
    }
}
