using UnityEngine;
using System.Collections;
using DG.Tweening;

public class AudioController : MonoBehaviour
{
    public AudioSource musicBus;
    public AudioSource[] effectBus;
    public AudioSource dynamicBus;

    public static AudioController Instance;

    private SoundBank bank;

    void Awake()
    {
        Instance = this;
        bank = GetComponent<SoundBank>();
    }

    void Start()
    {
        dynamicBus.clip = bank.Request(SoundBank.DynamicSounds.Comet);
        dynamicBus.loop = true;
        dynamicBus.Play();
    }
    public void StartMusic()
    {
        musicBus.clip = bank.Request(SoundBank.Music.Intro);
        musicBus.Play();
        musicBus.loop = false;

        StartCoroutine(CheckForEnd(musicBus, bank.Request(SoundBank.Music.Loop), true));
    }

    public void EndMusic()
    {
        musicBus.clip = bank.Request(SoundBank.Music.End);
        dynamicBus.Stop();
        musicBus.volume = .6f;
        musicBus.Play();
        musicBus.loop = false;
    }

    public void PlayAtEnd(AudioSource source, AudioClip next, bool loop)
    {
        StartCoroutine(CheckForEnd(source, next, loop));
    }

    IEnumerator CheckForEnd(AudioSource source, AudioClip next, bool loop)
    {
        yield return new WaitUntil(() => !source.isPlaying);
        source.clip = next;
        source.loop = loop;
        source.Play();
    }

    public void PlaySfx(SoundBank.SoundEffects clip)
    {
        AudioSource bus = effectBus[(int)clip];

        bus.clip = bank.Request(clip);
        bus.loop = false;
        bus.Play();
    }

    public void PlaySfx(SoundBank.SoundEffects clip, int busIndex)
    {
        AudioSource bus = effectBus[busIndex];

        bus.clip = bank.Request(clip);
        bus.loop = false;
        bus.Play();
    }

    public void FadeToDanger(bool inDanger)
    {
        if(inDanger)
        {
            dynamicBus.DOFade(.6f, .25f);
            musicBus.DOFade(0f, .25f);
        }
        else
        {
            dynamicBus.DOFade(0f, .25f);
            musicBus.DOFade(.6f, .25f);
        }
    }

}
