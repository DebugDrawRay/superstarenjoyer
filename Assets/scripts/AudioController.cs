using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class AudioController : MonoBehaviour
{
    public AudioSource musicBus;
//    public AudioSource[] effectBus;
    public AudioSource dynamicBus;

	protected List<AudioSource> sfxBusses; 

    public static AudioController Instance;

    private SoundBank bank;

    void Awake()
    {
        Instance = this;
        bank = GetComponent<SoundBank>();
		sfxBusses = new List<AudioSource>();
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

    public void PlayAtEnd(SoundBank.SoundEffects clipToReplace, SoundBank.SoundEffects nextClip, bool loop)
    {
		AudioSource source = GetSourcePlayingSfx(clipToReplace);
		AudioClip next = bank.Request(nextClip);
		if (source != null)
		{
			//If there is something to wait for, wait for it.
			StartCoroutine(CheckForEnd(source, next, loop));
		}
		else
		{
			//Just play it
			PlaySfx(clipToReplace, loop: loop);
		}

	}

    IEnumerator CheckForEnd(AudioSource source, AudioClip next, bool loop)
    {
        yield return new WaitUntil(() => !source.isPlaying);
        source.clip = next;
        source.loop = loop;
        source.Play();
    }

	public void PlaySfx(SoundBank.SoundEffects clip, SoundBank.SoundEffects? interrupt = null, bool loop = false)
	{
		AudioSource bus = GetFreeSfxBus();
		bus.clip = bank.Request(clip);
		bus.Play();
		bus.loop = loop;

		if (interrupt != null)
		{
			InterruptSfx((SoundBank.SoundEffects)interrupt);
		}
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

	protected AudioSource GetFreeSfxBus()
	{
		for (int i = 0; i < sfxBusses.Count; i++)
		{
			if (!sfxBusses[i].isPlaying)
				return sfxBusses[i];
		}

		//No busses available. Create.
		GameObject go = new GameObject("SFXAudioSource");
		go.transform.SetParent(transform);

		AudioSource audioSrc = go.AddComponent<AudioSource>();
		audioSrc.loop = false;
		audioSrc.playOnAwake = false;
		audioSrc.priority = 10;

		sfxBusses.Add(audioSrc);
		return audioSrc;
	}

	protected void InterruptSfx(SoundBank.SoundEffects clip)
	{
		AudioSource source = GetSourcePlayingSfx(clip);
		if (source != null)
			source.Stop();
	}

	protected AudioSource GetSourcePlayingSfx(SoundBank.SoundEffects clip)
	{
		AudioClip sfx = bank.Request(clip);
		for (int i = 0; i < sfxBusses.Count; i++)
		{
			if (sfxBusses[i].clip == sfx && sfxBusses[i].isPlaying)
				return sfxBusses[i];
		}
		return null;
	}
}
