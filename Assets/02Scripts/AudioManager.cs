using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;

public class AudioManager : Singleton<AudioManager>
{
    protected override void SceneChanged(Scene replacedScene, Scene newScene)
    {

    }

    [SerializeField]
    private AudioMixer audioMixer;

    [Space(5)]
    [SerializeField]
    private AudioMixerGroup sfxChannel;

    [Space(10)]
    [SerializeField]
    private AudioSource bgmSpeacker;
    [SerializeField]
    private Transform speackerBox;
    [SerializeField]
    private List<AudioSource> fbxSpeakers = new List<AudioSource>();

    private void Start()
    {
        string channelName;

        channelName = VolumChannel.BGM.ToString();
        audioMixer.SetFloat(channelName, DB(GetVolume(channelName)));

        channelName = VolumChannel.SFX.ToString();
        audioMixer.SetFloat(channelName, DB(GetVolume(channelName)));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetVolume(string channelName)
    {
        if (!PlayerPrefs.HasKey(channelName)) // 키가 없었을 경우 10으로 초기값 지정
        {
            PlayerPrefs.SetInt(channelName, 7);
            return 10;
        }
        else
        {
            return PlayerPrefs.GetInt(channelName);
        }

    }
    public void ChangeVolume(VolumChannel channel, int volume) // volume은 1~10의 정수(자료형은 실수지만 아무튼)
    {
        string channelName = channel.ToString();

        PlayerPrefs.SetInt(channelName, volume);
        audioMixer.SetFloat(channelName, DB(volume));
    }
    private float DB(float volume)
    {
        if (Mathf.Approximately(volume, 0f))
        {
            return -80f;
        }
        else
        {
            return 20 * Mathf.Log10(volume / 10f);
        }
    }

    [SerializeField]
    private AudioClip nextBGM = null;
    public void ChangeBGM(AudioClip bgm, bool useFade = true)
    {
        print("BGM Change");
        if (bgmSpeacker.clip == bgm) // BGM이 같을 경우 return
            return;

        if (useFade)
        {
            if (bgmSpeacker == null) // 현재 BGM이 없었을 경우
            {
                BgmChangeAndPlay(bgm);
                StartCoroutine("BGM_FadeIn");
            }
            else
            {
                nextBGM = bgm;
                StartCoroutine("BGM_FadeOut");
            }
        }
        else
        {
            BgmChangeAndPlay(bgm);
        }
    }
    private IEnumerator BGM_FadeOut()
    {
        print("BGM FadeOut");
        float progress = 1f;
        while (progress > 0.5f)
        {
            progress -= Time.deltaTime;
            bgmSpeacker.volume = progress;

            yield return null;
        }

        StartCoroutine("BGM_FadeIn");
    }
    private IEnumerator BGM_FadeIn()
    {
        BgmChangeAndPlay(nextBGM);

        float progress = 0.5f;
        while (progress < 1f)
        {
            progress += Time.deltaTime;
            bgmSpeacker.volume = progress;

            yield return null;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void BgmChangeAndPlay(AudioClip clip)
    {
        bgmSpeacker.Pause();

        bgmSpeacker.clip = clip;
        bgmSpeacker.Play();
        nextBGM = null;
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        AudioSource emptySource = null;
        foreach (AudioSource source in fbxSpeakers)
        {
            if (!source.isPlaying) // 사운드 재생 중이 아닐 경우
            {
                emptySource = source;

                break;
            }
        }

        if (!emptySource) // 빈 스피커가 없었다면
        {
            emptySource = CreateSpeacker(); // 새 스피커를 만들어 참조
        }

        emptySource.clip = clip;
        emptySource.volume = volume;
        emptySource.Play();
    }

    private AudioSource CreateSpeacker()
    {
        GameObject obj = new GameObject("Speaker " + fbxSpeakers.Count);
        obj.transform.parent = speackerBox;

        AudioSource speacker = obj.AddComponent<AudioSource>();

        speacker.outputAudioMixerGroup = sfxChannel;
        speacker.playOnAwake = false;
        speacker.loop = false;

        fbxSpeakers.Add(speacker);

        return speacker;
    }
}

public enum VolumChannel 
{
    BGM,
    SFX,
}
