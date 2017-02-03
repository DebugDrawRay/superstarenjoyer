using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class AudioController : MonoBehaviour
{
    public AudioSource musicBus;
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

    protected IEnumerator CheckForEnd(AudioSource source, AudioClip next, bool loop)
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

	public void PlayStarObtainedSFX(int starCount)
	{
		if (starCount == 1)
		{
			PlaySfx(SoundBank.SoundEffects.StarGood01);
		}
		else if (starCount == 2)
		{
			PlaySfx(SoundBank.SoundEffects.StarGood02);
		}
		else if (starCount == 3)
		{
			PlaySfx(SoundBank.SoundEffects.StarGood03);
		}
		else if (starCount == 4)
		{
			PlaySfx(SoundBank.SoundEffects.StarGood04);
		}
		else if (starCount == 5)
		{
			PlaySfx(SoundBank.SoundEffects.StarGood05);
		}
		else if (starCount == 6)
		{
			PlaySfx(SoundBank.SoundEffects.StarGood06);
		}
		else if (starCount == 7)
		{
			PlaySfx(SoundBank.SoundEffects.StarGood07);
		}
		else if (starCount == 8)
		{
			PlaySfx(SoundBank.SoundEffects.StarGood08);
		}
		else if (starCount == 9)
		{
			PlaySfx(SoundBank.SoundEffects.StarGood09);
		}
		else if (starCount == 10)
		{
			PlaySfx(SoundBank.SoundEffects.StarGood10);
		}
		else if (starCount == 11)
		{
			PlaySfx(SoundBank.SoundEffects.StarGood11);
		}
		else if (starCount == 12)
		{
			PlaySfx(SoundBank.SoundEffects.StarGood12);
		}
		else if (starCount == 13)
		{
			PlaySfx(SoundBank.SoundEffects.StarGood13);
		}
		else
		{
			PlaySfx(SoundBank.SoundEffects.StarGood14);
		}
	}
}
