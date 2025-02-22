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
        if (!PlayerPrefs.HasKey(channelName)) // Ű�� ������ ��� 10���� �ʱⰪ ����
        {
            PlayerPrefs.SetInt(channelName, 10);
            return 10;
        }
        else
        {
            return PlayerPrefs.GetInt(channelName);
        }

    }
    public void ChangeVolume(VolumChannel channel, int volume) // volume�� 1~10�� ����(�ڷ����� �Ǽ����� �ƹ�ư)
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

    public void ChangeBGM(AudioClip bgm)
    {
        if (bgmSpeacker.clip == bgm) // BGM�� ���� ��� return
            return;

        bgmSpeacker.Pause();

        bgmSpeacker.clip = bgm;
        bgmSpeacker.Play();

        StopCoroutine("VolumeCor");
        StartCoroutine("VolumeCor");
    }
    private IEnumerator VolumeCor()
    {
        float progress = 0.5f;
        while (progress < 1f) 
        {
            progress += Time.deltaTime;
            bgmSpeacker.volume = progress;

            yield return null;
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        AudioSource emptySource = null;
        foreach (AudioSource source in fbxSpeakers)
        {
            if (!source.isPlaying) // ���� ��� ���� �ƴ� ���
            {
                emptySource = source;

                break;
            }
        }

        if (!emptySource) // �� ����Ŀ�� �����ٸ�
        {
            emptySource = CreateSpeacker(); // �� ����Ŀ�� ����� ����
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
