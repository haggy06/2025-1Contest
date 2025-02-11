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
    private TextMeshProUGUI dayText;
    [SerializeField]
    private TextMeshProUGUI moneyText;
    [SerializeField]
    private Image dangerGauge;

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

    public void RefreshDay()
    {
        dayText.text = "Day " + DataManager.GameData.day;
    }
    public void RefreshMoney()
    {
        moneyText.text = string.Format("{0:C0}", DataManager.GameData.money);
    }
    public void RefreshDanger()
    {
        dangerGauge.fillAmount = DataManager.GameData.danger;
    }
}
