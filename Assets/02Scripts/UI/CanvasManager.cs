using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip bgm;

    [Space(5)]
    [SerializeField]
    protected float fadeTime = 0.5f;
    [SerializeField]
    protected CanvasGroup fadeImage;

    [Space(5)]
    [SerializeField]
    protected CanvasGroup[] closedPopupWhenAwake;

    public void ChangeBGMVolume(float newVolume)
    {
        AudioManager.Inst.ChangeVolume(VolumChannel.BGM, Mathf.RoundToInt(newVolume));
        //DataManager.SettingData.bgmVolume = Mathf.RoundToInt(newVolume);

        //DataManager.SaveSettingData();
    }
    public void ChangeSFXVolume(float newVolume)
    {
        AudioManager.Inst.ChangeVolume(VolumChannel.SFX, Mathf.RoundToInt(newVolume));
        //DataManager.SettingData.sfxVolume = Mathf.RoundToInt(newVolume);

        //DataManager.SaveSettingData();
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

        SetBGM();
    }
    protected virtual void SetBGM()
    { 
        if (bgm != null)
            AudioManager.Inst.ChangeBGM(bgm, false);
    }

    protected virtual void PopupHide(CanvasGroup popup)
    {
        popup.blocksRaycasts = false;
        popup.alpha = 0f;
    }
    protected virtual void PopupShow(CanvasGroup popup) 
    {
        popup.blocksRaycasts = true;
        popup.alpha = 1f;
    }

    public void PlaySound(AudioClip audioClip)
    {
        AudioManager.Inst.PlaySFX(audioClip);
    }

    private Dictionary<CanvasGroup, int> popupIDs = new Dictionary<CanvasGroup, int>();
    public void PopupFadeIn(CanvasGroup group)
    {
        group.blocksRaycasts = true;

        if (popupIDs.ContainsKey(group))
        {
            LeanTween.cancel(popupIDs[group]);
            popupIDs[group] = LeanTween.alphaCanvas(group, 1f, fadeTime * (1f - group.alpha)).id;
        }
        else
        {
            popupIDs.Add(group, LeanTween.alphaCanvas(group, 1f, fadeTime * (1f - group.alpha)).id);
        }
    }
    public void PopupFadeOut(CanvasGroup group)
    {
        group.blocksRaycasts = false;

        if (popupIDs.ContainsKey(group))
        {
            LeanTween.cancel(popupIDs[group]);
            popupIDs[group] = LeanTween.alphaCanvas(group, 0f, fadeTime * group.alpha).id;
        }
        else
        {
            popupIDs.Add(group, LeanTween.alphaCanvas(group, 0f, fadeTime * group.alpha).id);
        }
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
