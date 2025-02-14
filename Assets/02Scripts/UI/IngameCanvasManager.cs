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

    [Header("Drawer Setting")]
    [SerializeField]
    private float drawerTime = 0.5f;
    [SerializeField]
    private Vector2 closePos;
    [SerializeField]
    private Vector2 openPos;

    protected new void Awake()
    {
        base.Awake();

        bgmSlider.onValueChanged.AddListener(ChangeBGMVolume);
        bgmSlider.value = DataManager.SettingData.bgmVolume;
        sfxSlider.onValueChanged.AddListener(ChangeSFXVolume);
        sfxSlider.value = DataManager.SettingData.sfxVolume;

        dayText = _dayText;
        moneyText = _moneyText;
        dangerGauge = _dangerGauge;

        RefreshDay();
        RefreshMoney();
        RefreshDanger();
    }
    
    public void DrawerPopupOpen(CanvasGroup popup)
    {
        popup.blocksRaycasts = true;
        LeanTween.moveLocal(popup.gameObject, openPos, drawerTime);
    }
    public void DrawerPopupClose(CanvasGroup popup)
    {
        popup.blocksRaycasts = false;
        LeanTween.moveLocal(popup.gameObject, openPos, drawerTime);
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
        dangerGauge.fillAmount = DataManager.GameData.danger;
    }
}
