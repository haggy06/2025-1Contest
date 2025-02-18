using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TalkManager : MonoBehaviour
{
    private const int tutorialIndex = 4;

    [SerializeField]
    private TalkExcel talkExcel;
    [SerializeField]
    private Sprite[] face;

    [Space(5)]
    [SerializeField]
    private TalkUI[] talkUI;

    [Space(5)]
    [SerializeField]
    private float fadeTime = 0.5f;
    [SerializeField]
    private Image block_2D;
    [SerializeField]
    private Image block_3D;

    [Header("Tutorial Refs")]
    [SerializeField]
    private QuestManager questManager;

    private int curIndex = 0;
    private TalkSheet curTalk;

    private void Awake()
    {
        foreach (var talk in talkUI)
        {
            talk.gameObject.SetActive(false);
        }

        TalkStart(0);
        GameManager.Inst.Subscribe(EventType.NextTutorial, NextTutorial);
    }

    private void NextTutorial()
    {
        if (curTalk.next)
        {
            TalkStart(curIndex + 1);
            GameManager.Inst.ChangeTutorial((TutorialState)curTalk.parameter); // 엑셀 파라미터를 여기에 쓰자
        }
        else
        {
            talkUI[curTalk.speaker].gameObject.SetActive(false);
            curTalk = null;

            GameManager.Inst.TutorialClear();
        }
    }

    public void TalkStart(int talkIndex)
    {
        StopAllCoroutines();

        if (curTalk != null)
        {
            talkUI[curTalk.speaker].gameObject.SetActive(false);
        }

        curIndex = talkIndex;
        curTalk = talkExcel.Text[curIndex];

        if (curTalk.startInvoke && !curTalk.methodName.Equals(""))
        {
            Invoke(curTalk.methodName, 0f);
        }

        talkUI[curTalk.speaker].gameObject.SetActive(true);
        talkUI[curTalk.speaker].ChangeFace(face[curTalk.face]);

        StartCoroutine("WriteTextCor");
    }

    private bool writing = false;
    private IEnumerator WriteTextCor()
    {        
        string text = curTalk.text;

        talkUI[curTalk.speaker].text.text = "";
        writing = true;

        for (int i = 0; i < text.Length; i++)
        {
            if (text != null)
            {
                talkUI[curTalk.speaker].text.text += text[i];

                if (text[i] != ' ') // 띄어쓰기가 아니었을 경우 잠시 대기
                    yield return YieldReturn.waitForFixedUpdate;
            }
        }

        writing = false;
    }

    public void InstantWriteText()
    {
        StopCoroutine("WriteTextCor");
        talkUI[curTalk.speaker].text.text = curTalk.text;

        writing = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && curTalk != null)
        {
            if (curTalk.speaker <= (int)Speaker.Info)
                ScreenClick();
        }
    }
    public void ScreenClick()
    {
        if (writing)
            InstantWriteText();
        else
        {
            if (!curTalk.startInvoke && !curTalk.methodName.Equals(""))
            {
                Invoke(curTalk.methodName, 0f);
            }

            if (curTalk.next)
                TalkStart(curIndex + 1);
            else
            {
                talkUI[curTalk.speaker].gameObject.SetActive(false);
                curTalk = null;
            }
        }
    }

    #region _Excel Methods_
    private void TutorialStart()
    {
        questManager.OrderStart(curIndex);
        GameManager.Inst.ChangeTutorial(TutorialState.OrderGet);

        TalkStart(tutorialIndex);
    }

    private void ScreenHide()
    {
        switch (curTalk.parameter)
        {
            case 0:
                block_2D.color = Color.black;
                block_3D.color = Color.black;
                break;

            case 1:
                block_2D.color = Color.black;
                break;

            case 2:
                block_3D.color = Color.black;
                break;

            default:
                break;
        }
    }

    private int fadeTweenID_2D = 0;
    private int fadeTweenID_3D = 0;
    private void ScreenFadeOut()
    {
        switch (curTalk.parameter)
        {
            case 0:
                LeanTween.cancel(fadeTweenID_2D);
                LeanTween.cancel(fadeTweenID_3D);

                fadeTweenID_2D = LeanTween.alpha(block_2D.rectTransform, 0f, block_2D.color.a * fadeTime).id;
                fadeTweenID_3D = LeanTween.alpha(block_3D.rectTransform, 0f, block_3D.color.a * fadeTime).id;
                break;

            case 1:
                LeanTween.cancel(fadeTweenID_2D);
                
                fadeTweenID_2D = LeanTween.alpha(block_2D.rectTransform, 0f, block_2D.color.a * fadeTime).id;
                break;

            case 2:
                LeanTween.cancel(fadeTweenID_3D);

                fadeTweenID_3D = LeanTween.alpha(block_3D.rectTransform, 0f, block_3D.color.a * fadeTime).id;
                break;

            default:
                break;
        }
    }
    private void ScreenFadeIn()
    {
        switch (curTalk.parameter)
        {
            case 0:
                LeanTween.cancel(fadeTweenID_2D);
                LeanTween.cancel(fadeTweenID_3D);

                fadeTweenID_2D = LeanTween.alpha(block_2D.rectTransform, 0f, (1f - block_2D.color.a) * fadeTime).id;
                fadeTweenID_3D = LeanTween.alpha(block_2D.rectTransform, 0f, (1f - block_2D.color.a) * fadeTime).id;
                break;

            case 1:
                LeanTween.cancel(fadeTweenID_2D);

                fadeTweenID_2D = LeanTween.alpha(block_2D.rectTransform, 0f, (1f - block_2D.color.a) * fadeTime).id;
                break;

            case 2:
                LeanTween.cancel(fadeTweenID_3D);

                fadeTweenID_3D = LeanTween.alpha(block_2D.rectTransform, 0f, (1f - block_2D.color.a) * fadeTime).id;
                break;

            default:
                break;
        }
    }
    #endregion
}
