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

        GameManager.Inst.Subscribe(EventType.Order, SetOrderScroll);
    }

    private void SetOrderScroll()
    {
        ItemData orderItem = GameManager.Inst.orderItem;

        itemIcon.sprite = orderItem.sprite;
        itemIcon.rectTransform.sizeDelta = orderItem.sprite.bounds.size;

        // Todo : itemName.sprite = orderItem.name;

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
