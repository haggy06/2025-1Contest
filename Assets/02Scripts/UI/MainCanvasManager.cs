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

        if (DataManager.GameData.day <= 1) // ����� �����Ͱ� ���� ���(���� ������ ���̺�� 2�������� �ǹǷ� ����� �������� ��¥�� 1�� ������ ��� ����� �����Ͱ� ���� ����)
        {
            newGameBtn.onClick.AddListener(() => MoveScene((int)SCENE.Ingame)); // ���� �� Ingame ȭ������ �̵�
            loadGameBtn.interactable = false; // ��ư ��Ȱ��ȭ
        }
        else // ����� �����Ͱ� ���� ���
        {
            newGameBtn.onClick.AddListener(() => PopupFadeIn(resetWarningPopup)); // ���� �� �ʱ�ȭ ��� �˾� ����
            loadGameBtn.interactable = true; // ��ư Ȱ��ȭ
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
