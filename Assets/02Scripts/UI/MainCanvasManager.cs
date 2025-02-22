using UnityEngine;
using UnityEngine.UI;

public class MainCanvasManager : CanvasManager
{
    [SerializeField]
    private Slider bgmSlider;
    [SerializeField]
    private Slider sfxSlider;

    [Header("Button Setting")]
    [SerializeField]
    private Button newGameBtn;
    [SerializeField]
    private CanvasGroup resetWarningPopup;

    [Space(5)]
    [SerializeField]
    private Button loadGameBtn;
    protected new void Awake()
    {
        base.Awake();
        bgmSlider.onValueChanged.AddListener(ChangeBGMVolume);
        bgmSlider.value = AudioManager.GetVolume(VolumChannel.BGM.ToString());
        sfxSlider.onValueChanged.AddListener(ChangeSFXVolume);
        sfxSlider.value = AudioManager.GetVolume(VolumChannel.SFX.ToString());

        if (DataManager.GameData.day <= 1) // 저장된 데이터가 없을 경우(게임 데이터 세이브는 2일차부터 되므로 저장된 데이터의 날짜가 1일 이하일 경우 저장된 데이터가 없단 뜻임)
        {
            newGameBtn.onClick.AddListener(() => MoveScene((int)SCENE.Ingame)); // 누를 시 Ingame 화면으로 이동
            loadGameBtn.interactable = false; // 버튼 비활성화
        }
        else // 저장된 데이터가 있을 경우
        {
            newGameBtn.onClick.AddListener(() => PopupFadeIn(resetWarningPopup)); // 누를 시 초기화 경고 팝업 오픈
            loadGameBtn.interactable = true; // 버튼 활성화
        }
    }
    public void LoadGame()
    {
        DataManager.RefreshGameData();
        MoveScene((int)SCENE.Ingame);
    }

    public void ResetAndPlay()
    {
        DataManager.ResetGameData();

        MoveScene((int)SCENE.Ingame);
    }

    public void GameQuit()
    {
        PopupFadeIn(fadeImage);
        Invoke("GameQuitInvoke", fadeTime);
    }
    private void GameQuitInvoke()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
