using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngameCanvasManager : CanvasManager
{
    [SerializeField]
    private ObjectSlider bgmSlider;
    [SerializeField]
    private ObjectSlider sfxSlider;

    [Header("Components Reference")]
    [SerializeField]
    private TextMeshProUGUI _dayText;
    private static TextMeshProUGUI dayText;

    [SerializeField]
    private TextMeshProUGUI _moneyText;
    private static TextMeshProUGUI moneyText;

    [SerializeField]
    private Image _dangerGauge;
    private static Image dangerGauge;

    [Header("Order Setting")]
    [SerializeField]
    private Image itemIcon;
    [SerializeField]
    private Image itemName;
    [SerializeField]
    private TextMeshProUGUI priceText;

    [Header("Receipt Setting")]
    [SerializeField]
    private CanvasGroup receipt;
    [SerializeField]
    private AudioClip receiptSound;
    [SerializeField]
    private TextMeshProUGUI dayEndText;
    [SerializeField]
    private TextMeshProUGUI profitText;
    [SerializeField]
    private TextMeshProUGUI dangerText;
    [SerializeField]
    private Image newsImage;

    [Header("Ect CanvasGroup")]
    [SerializeField]
    private CanvasGroup order;
    [SerializeField]
    private CanvasGroup blind;
    [SerializeField]
    private CanvasGroup blackScreen;

    [Header("More Ects")]
    [SerializeField]
    private Sprite[] newsArr;
    [SerializeField]
    private DrawerButton infoBtn;

    protected new void Awake()
    {
        base.Awake();

        infoBtn.BangActive(DataManager.GameData.day == 3); // 3일차엔 수정구 ! 띄우기

        bgmSlider.onValueChanged.AddListener(ChangeBGMVolume);
        bgmSlider.value = AudioManager.GetVolume(VolumChannel.BGM.ToString());
        sfxSlider.onValueChanged.AddListener(ChangeSFXVolume);
        sfxSlider.value = AudioManager.GetVolume(VolumChannel.SFX.ToString());

        dayText = _dayText;
        moneyText = _moneyText;
        dangerGauge = _dangerGauge;

        RefreshDay();
        RefreshMoney();
        RefreshDanger();

        GameManager.Inst.Subscribe(EventType.Order, SetOrderScroll);
        GameManager.Inst.Subscribe(EventType.DayFinish, ReceiptOpen);
        GameManager.Inst.Subscribe(EventType.DayStart, DayStart);
    }

    private int startMoney = 0;
    private float startDanger = 0f;
    private void DayStart()
    {
        PopupHide(order);

        startMoney = DataManager.GameData.money;
        startDanger = DataManager.GameData.danger;

        RefreshDay();
    }
    private void ReceiptOpen()
    {
        print("ReceiptOpen");

        dayEndText.text = DataManager.GameData.day + "일차 완료!";


        profitText.text = "순 이익 : ";
        int profit = DataManager.GameData.money - startMoney;
        if (profit >= 0)
            profitText.text += "+";
        profitText.text += MyCalculator.AddComma(profit);


        dangerText.text = "위험도 : ";
        int danger = Mathf.RoundToInt((DataManager.GameData.danger - startDanger) * 100f);
        if (danger >= 0)
            dangerText.text += "+";
        dangerText.text += danger;
        dangerText.text += "%";

        int newsIndex = (int)NewsType.None;
        switch (DataManager.GameData.day)
        {
            case 1:
                newsIndex = (int)NewsType.GrandOpen;
                break;

            case 2:
                newsIndex = (int)NewsType.DangerSpy;
                break;
            
            case 3:
                if (DataManager.GameData.isSpyOwnered && DataManager.GameData.isSpyKnown)
                {
                    if (DataManager.GameData.isSpyDead)
                    {
                        if (!DataManager.GameData.spyDeadNews)
                        {
                            DataManager.GameData.spyDeadNews = true;

                            newsIndex = (int)NewsType.SpyDead;
                        }
                    }
                    else
                    {
                        if (!DataManager.GameData.spyAppearNews)
                        {
                            DataManager.GameData.spyAppearNews = true;

                            newsIndex = (int)NewsType.SpyAppear;
                        }
                    }
                }
                break;

            case 4:
                if (DataManager.GameData.isSpyOwnered && DataManager.GameData.isSpyKnown)
                {
                    if (DataManager.GameData.isSpyDead)
                    {
                        if (!DataManager.GameData.spyDeadNews)
                        {
                            DataManager.GameData.spyDeadNews = true;

                            newsIndex = (int)NewsType.SpyDead;
                        }
                    }
                    else
                    {
                        if (!DataManager.GameData.spyAppearNews)
                        {
                            DataManager.GameData.spyAppearNews = true;

                            newsIndex = (int)NewsType.SpyAppear;
                        }
                    }
                }
                break;

            case 5:
                newsIndex = (int)NewsType.CrownSale;
                break;
        }
        MyCalculator.ChangeUISprite(newsImage, newsArr[newsIndex]);

        Blind(true);

        AudioManager.Inst.PlaySFX(receiptSound);
        PopupFadeIn(receipt);
    }
    public void Blind(bool isOn, bool instantFade = false)
    {
        if (instantFade)
        {
            if (isOn)
                PopupShow(blind);
            else
                PopupHide(blind);
        }
        else
        {
            if (isOn)
                PopupFadeIn(blind);
            else
                PopupFadeOut(blind);
        }
    }
    public void ScreenFade(bool isOn, bool instantFade = false)
    {
        if (instantFade)
        {
            if (isOn)
                PopupShow(blackScreen);
            else
                PopupHide(blackScreen);
        }
        else
        {
            if (isOn)
                PopupFadeIn(blackScreen);
            else
                PopupFadeOut(blackScreen);
        }
    }

    public void MoveToNextDay()
    {
        print("MoveToNextDay");
        Blind(false);
        PopupFadeIn(fadeImage);
        PopupFadeOut(receipt);

        infoBtn.BangActive(DataManager.GameData.day == 3); // 3일차엔 수정구 ! 띄우기
        Invoke("DayMove", fadeTime + 1f);
    }
    private void DayMove()
    {
        PopupFadeOut(fadeImage);
        GameManager.Inst.DayStartInvoke();
    }

    private void SetOrderScroll()
    {
        ItemData orderItem = GameManager.Inst.orderItem;

        MyCalculator.ChangeUISprite(itemName, orderItem.nameSprite);
        MyCalculator.ChangeUISprite(itemIcon, orderItem.iconSprite);
        priceText.text = "보수 : " + MyCalculator.AddComma(orderItem.price);
    }
        
    public static void RefreshDay()
    {
        dayText.text = "Day " + DataManager.GameData.day;
    }
    public static void RefreshMoney()
    {
        moneyText.text = MyCalculator.AddComma(DataManager.GameData.money);
    }
    public static void RefreshDanger()
    {
        LeanTween.value(dangerGauge.fillAmount, DataManager.GameData.danger, 1f).setEase(LeanTweenType.easeOutQuint).setOnUpdate((value) => dangerGauge.fillAmount = value);
    }
}

public enum NewsType
{
    None,

    GrandOpen,

    SpyAppear,
    DangerSpy,
    SpyDead,

    CrownSale,
}