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

    protected new void Awake()
    {
        base.Awake();

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
        int profit = DataManager.GameData.money - startMoney;
        if (profit >= 0)
        {
            profitText.text = "+" + MyCalculator.AddComma(profit);
            profitText.color = Color.green;
        }
        else
        {
            profitText.text = MyCalculator.AddComma(profit);
            profitText.color = Color.red;
        }

        dayEndText.text = DataManager.GameData.day + "일차 완료!";
        dangerText.text = Mathf.RoundToInt((DataManager.GameData.danger - startDanger) * 100f) + "%";

        int newsIndex = (int)NewsType.None;
        switch (DataManager.GameData.day)
        {
            case 1:
                newsIndex = (int)NewsType.GrandOpen;
                break;

            case 2:
                if (DataManager.GameData.isSpyOwnered) // 스파이 주문을 들어줬었을 경우
                    newsIndex = (int)NewsType.SpyAppear;
                break;
            
            case 3:
                if (DataManager.GameData.isSpyDead)
                    newsIndex = (int)NewsType.SpyDead;
                break;

            case 4:
                if (DataManager.GameData.DangerBySpy())
                    newsIndex = (int)NewsType.DangerSpy;
                break;

            case 5:
                newsIndex = (int)NewsType.CrownSale;
                break;
        }
        MyCalculator.ChangeUISprite(newsImage, newsArr[newsIndex]);

        Blind(true);
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
        Blind(false);
        PopupFadeIn(fadeImage);
        PopupFadeOut(receipt);

        Invoke("DayMove", fadeTime);
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