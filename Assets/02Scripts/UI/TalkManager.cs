using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TalkManager : Singleton<TalkManager>
{
    [SerializeField]
    private AudioClip[] endBGMs;
    [SerializeField]
    private AudioClip talkNextSound;
    [SerializeField]
    private bool _writing = false;
    public bool writing => _writing;

    [SerializeField]
    private TalkExcel talkExcel;
    [SerializeField]
    private Sprite[] face;

    [Space(5)]
    [SerializeField]
    private TalkUI[] talkUI;

    [Space(5)]
    [SerializeField]
    private IngameCanvasManager canvasManager;
    [SerializeField]
    private ShopManager shop;

    [Header("Tutorial Refs")]
    [SerializeField]
    private QuestManager questManager;
    [SerializeField]
    private ItemData orderItem;
    [SerializeField]
    private ItemData buyItem;

    [Header("Spy")]
    [SerializeField]
    private string[] spyExplain;
    [SerializeField, Range(0, 10)]
    private int spyWaningPercent = 3;

    private int curIndex = 0;
    private TalkSheet _curTalk;
    public TalkSheet curTalk => _curTalk;
    private SheetType _curSheet = SheetType.Story;

    protected new void Awake()
    {
        base.Awake();
        foreach (var talk in talkUI)
        {
            talk.gameObject.SetActive(false);
        }

        if (DataManager.GameData.day == 1) // 첫째날일 경우
            TalkStart(0);

        GameManager.Inst.Subscribe(EventType.TutorialClear, NextTutorial);

        GameManager.Inst.Subscribe(EventType.End, End);
    }
    private void End()
    {
        switch (GameManager.Inst.endingType)
        {
            case StoryTalkIndex.TrapEnd:
                AudioManager.Inst.ChangeBGM(endBGMs[0]);
                break;
            case StoryTalkIndex.BankruptcyEnd:
                AudioManager.Inst.ChangeBGM(endBGMs[1]);
                break;
            case StoryTalkIndex.NormalEnd:
                AudioManager.Inst.ChangeBGM(endBGMs[2]);
                break;
            case StoryTalkIndex.HappyEnd:
                AudioManager.Inst.ChangeBGM(endBGMs[3]);
                break;
        }

        Invoke("EndTalk", 0f);
    }
    private void EndTalk()
    {
        TalkStart((int)GameManager.Inst.endingType);
    }

    private void NextTutorial()
    {
        print("NextTutorial");
        if (_curTalk.next)
        {
            TalkStart(curIndex + 1);
            GameManager.Inst.TutorialChange((TutorialState)_curTalk.parameter); // 엑셀 파라미터를 여기에 쓰자
        }
        else
        {
            talkUI[_curTalk.speaker].gameObject.SetActive(false);
            _curTalk = null;

            GameManager.Inst.TutorialClear();
        }
    }

    public event System.Action<Speaker> TalkFinish;
    public void TalkStart(int talkIndex, SheetType sheetType = SheetType.Story)
    {
        StopAllCoroutines();

        _curSheet = sheetType;
        curIndex = talkIndex;

        if (_curTalk != null)
        {
            talkUI[_curTalk.speaker].gameObject.SetActive(false);
        }

        if (_curSheet == SheetType.Story)
            _curTalk = talkExcel.StoryTalk[curIndex];
        else if (_curSheet == SheetType.NPC)
            _curTalk = talkExcel.NPCTalk[curIndex];
        else if (_curSheet == SheetType.Info)
            _curTalk = talkExcel.InfoTalk[curIndex];

        if (_curTalk.startInvoke && !_curTalk.methodName.Equals(""))
        {
            Invoke(_curTalk.methodName, 0f);
        }

        talkUI[_curTalk.speaker].gameObject.SetActive(true);
        talkUI[_curTalk.speaker].ChangeFace(face[_curTalk.face]);

        StartCoroutine("WriteTextCor");
    }
    public void TalkStart(TalkSheet sheet, int talkIndex)
    {
        StopAllCoroutines();

        curIndex = talkIndex;

        if (_curTalk != null)
        {
            talkUI[_curTalk.speaker].gameObject.SetActive(false);
        }

        _curTalk = sheet;

        talkUI[_curTalk.speaker].gameObject.SetActive(true);
        talkUI[_curTalk.speaker].ChangeFace(face[_curTalk.face]);

        StartCoroutine("WriteTextCor");
    }
    private IEnumerator WriteTextCor()
    {
        string text = _curTalk.text;

        talkUI[_curTalk.speaker].text.text = "";
        _writing = true;

        for (int i = 0; i < text.Length; i++)
        {
            if (text != null)
            {
                talkUI[_curTalk.speaker].text.text += text[i];

                if (text[i] == '\\') // \n일 테니 한 단어 더 출력
                    talkUI[_curTalk.speaker].text.text += text[++i];
                else if (text[i] != ' ') // 띄어쓰기가 아니었을 경우 잠시 대기
                    yield return YieldReturn.waitForFixedUpdate;
            }
        }

        _writing = false;
    }

    public void InstantWriteText()
    {
        StopCoroutine("WriteTextCor");
        talkUI[_curTalk.speaker].text.text = _curTalk.text;

        _writing = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_curTalk != null)
            {
                TalkInteract(_curTalk.speaker <= (int)Speaker.Info);
            }
        }
    }

    TalkSheet lastSheet = null;
    public void TalkInteract(bool nextTalk)
    {
        lastSheet = _curTalk;

        if (_writing)
            InstantWriteText();
        else
        {
            if (GameManager.Inst.tutorialState == TutorialState.None || !GameManager.Inst.TutorialCheck(TutorialState.JustTalk))
            {
                bool isMethod = !_curTalk.startInvoke && !_curTalk.methodName.Equals("");
                if (isMethod)
                    Invoke(_curTalk.methodName, 0f);

                if (nextTalk)
                {
                    AudioManager.Inst.PlaySFX(talkNextSound);
                    StartCoroutine("NextTalk");
                }
            }
        }
    }
    private IEnumerator NextTalk()
    {
        yield return null;
        if (lastSheet == _curTalk)
        {
            if (_curTalk.next)
                TalkStart(curIndex + 1, _curSheet);
            else
            {
                TalkFinish?.Invoke((Speaker)_curTalk.speaker);

                talkUI[_curTalk.speaker].gameObject.SetActive(false);

                _curTalk = null;
            }
        }
    }

    #region _Excel Methods_

    private void NPCTalk()
    {
        TalkStart(_curTalk.parameter, SheetType.NPC);
    }
    private void Clue()
    {
        if (DataManager.GameData.DangerBySpy() && Random.Range(0, 10) < spyWaningPercent) // 스파이가 위협적일 때 확률적으로 단서 줌(or 까먹음)
        {
            SpyWarning_NPC();
        }
        else
        {
            TalkStart(_curTalk.parameter - 1, SheetType.NPC);
        }
    }
    private void SpyWarning_Info()
    {
        #region _Deep Copy_
        TalkSheet originalTalk = talkExcel.InfoTalk[_curTalk.parameter]; // 파라미터를 목표 인덱스로 사용
        TalkSheet newTalk = new TalkSheet();

        newTalk.next = originalTalk.next;
        newTalk.speaker = originalTalk.speaker;
        newTalk.face = originalTalk.face;
        newTalk.text = spyExplain[DataManager.GameData.spyNPC.faceIndex - 3] + originalTalk.text;
        newTalk.methodName = originalTalk.methodName;
        newTalk.parameter = originalTalk.parameter;
        newTalk.startInvoke = originalTalk.startInvoke;
        #endregion

        GameManager.Inst.spyInformed = true;
        DataManager.GameData.isSpyKnown = true;
        TalkStart(newTalk, _curTalk.parameter);
    }
    private void SpyWarning_NPC()
    {
        #region _Deep Copy_
        TalkSheet originalTalk = talkExcel.NPCTalk[_curTalk.parameter]; // 파라미터를 목표 인덱스로 사용
        TalkSheet newTalk = new TalkSheet();

        newTalk.next = originalTalk.next;
        newTalk.speaker = originalTalk.speaker;
        newTalk.face = originalTalk.face;
        newTalk.text = spyExplain[DataManager.GameData.spyNPC.faceIndex - 3] + originalTalk.text;
        newTalk.methodName = originalTalk.methodName;
        newTalk.parameter = originalTalk.parameter;
        newTalk.startInvoke = originalTalk.startInvoke;
        #endregion

        GameManager.Inst.spyInformed = true;
        DataManager.GameData.isSpyKnown = true;
        TalkStart(newTalk, _curTalk.parameter);
    }

    private void Request()
    {
        if (DataManager.GameData.DangerBySpy()) // 스파이 땜시 위험할 경우
        {
            if (!DataManager.GameData.isSpyKnown) // 스파이를 모르고 있거나 주문을 들어줬을 경우
            {
                TalkStart((int)NPCTalkIndex.Spy_UnknownSpy, SheetType.NPC);
            }
            else if (DataManager.GameData.money < 8000) // 돈이 모자랄 경우
            {
                TalkStart((int)NPCTalkIndex.Spy_LowMoney, SheetType.NPC);
            }
            else if (DataManager.GameData.isSpyOwnered) // 스파이 퀘스트를 들어 줬었을 경우
            {
                TalkStart((int)NPCTalkIndex.Spy_Accept, SheetType.NPC);
            }
        }
        else
        {
            TalkStart((int)NPCTalkIndex.Spy_UnknownSpy, SheetType.NPC);
        }
    }
    private void KillSpy()
    {
        MoneyAdd();
        IngameCanvasManager.RefreshMoney();

        DataManager.GameData.danger = Mathf.Clamp(DataManager.GameData.danger - 0.3f, 0f, 1f);
        IngameCanvasManager.RefreshDanger();

        DataManager.GameData.isSpyDead = true;
    }

    private void Restock()
    {
        shop.Init();
    }

    private void MoneyAdd()
    {
        print("MoneyAdd : " + _curTalk.parameter);
        DataManager.GameData.money += _curTalk.parameter;
        IngameCanvasManager.RefreshMoney();
    }

    private void TutorialStart()
    {
        canvasManager.Blind(false);

        questManager.OrderStart(orderItem);
        GameManager.Inst.TutorialStart();

        TalkStart((int)StoryTalkIndex.Tutorial);
    }
    private void TutorialFinish()
    {
        GameManager.Inst.TutorialClear();
    }

    private void ScreenHide()
    {
        switch (_curTalk.parameter)
        {
            case 0:
                canvasManager.ScreenFade(true, true);
                break;

            case 1:
                canvasManager.Blind(true, true);
                break;

            default:
                break;
        }
    }
    private void ScreenFadeOut()
    {
        switch (_curTalk.parameter)
        {
            case 0:
                canvasManager.ScreenFade(false);
                break;

            case 1:
                canvasManager.Blind(false);
                break;

            default:
                break;
        }
    }
    private void ScreenFadeIn()
    {
        switch (_curTalk.parameter)
        {
            case 0:
                canvasManager.ScreenFade(true);
                break;

            case 1:
                canvasManager.Blind(true);
                break;

            default:
                break;
        }
    }

    private void MoveScene()
    {
        canvasManager.MoveScene(_curTalk.parameter);
    }
    #endregion
    protected override void SceneChanged(Scene replacedScene, Scene newScene) { }
}
public enum SheetType
{
    Story,
    NPC,
    Info,
}