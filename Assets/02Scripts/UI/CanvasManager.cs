using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField]
    protected float fadeTime = 0.5f;
    [SerializeField]
    protected CanvasGroup fadeImage;

    [Space(5)]
    [SerializeField]
    protected CanvasGroup[] closedPopupWhenAwake;

    public void ChangeBGMVolume(float newVolume)
    {
        DataManager.SettingData.bgmVolume = Mathf.RoundToInt(newVolume);
    }
    public void ChangeSFXVolume(float newVolume)
    {
        DataManager.SettingData.sfxVolume = Mathf.RoundToInt(newVolume);
    }
    protected void Awake()
    {
        if (fadeImage) // 페이드 이미지가 있을 경우
        {
            fadeImage.alpha = 1f;
            PopupFadeOut(fadeImage);
        }

        foreach(CanvasGroup popup in closedPopupWhenAwake)
        {
            PopupHide(popup);
        }
    }
    protected void OnDestroy()
    {
        DataManager.SaveSettingData();
    }

    protected virtual void PopupHide(CanvasGroup popup)
    {
        popup.blocksRaycasts = false;
        popup.alpha = 0f;
    }

    public void PopupFadeIn(CanvasGroup group)
    {
        group.blocksRaycasts = true;
        LeanTween.alphaCanvas(group, 1f, fadeTime * (1f - group.alpha));
    }
    public void PopupFadeOut(CanvasGroup group)
    {
        group.blocksRaycasts = false;
        LeanTween.alphaCanvas(group, 0f, fadeTime * group.alpha);
    }

    public void MoveScene(int sceneIndex)
    {
        SCENE scene = (SCENE)sceneIndex;
        if (fadeImage) // 페이드 이미지가 있을 경우
            StartCoroutine(MoveSceneAfterFade(scene));
        else // 없을 경우
            SceneMover.MoveScene(scene);
    }

    private IEnumerator MoveSceneAfterFade(SCENE scene)
    {
        PopupFadeIn(fadeImage);

        yield return YieldReturn.WaitForSeconds(fadeTime);

        SceneMover.MoveScene(scene);
    }
}
