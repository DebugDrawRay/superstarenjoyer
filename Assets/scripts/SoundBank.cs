using UnityEngine;
using System;
using System.Collections;

public class SoundBank : MonoBehaviour
{
    public enum Music
    {
        Intro,
        Loop,
        End
    }
    public AudioClip[] MusicTracks;

    public enum SoundEffects
    {
        StarGood = 0,
        StarBad = 1,
        ConstellationHit = 2,
        ConstellationSent = 3,
        ConstellationComplete = 4,
        ConstellationBroken = 5,
        StarGood00 = 6,
        StarGood01 = 7,
        StarGood02 = 8,
        StarGood03 = 9,
        StarGood04 = 10,
        StarGood05 = 11,
        StarGood06 = 12,
        StarGood07 = 13,
        StarGood08 = 14,
        StarGood09 = 15,
        StarGood10 = 16,
        StarGood11 = 17,
        StarGood12 = 18,
        StarGood13 = 19,
        StarGood14 = 20,
        StarTouch = 21,
        NewLink = 22

		//Remember, if you're adding a new sound,
		//You still have to add it in the inspector to the 'Sound Effect Clips' array.
		//Make sure that the number above matches the element number in the array.
    }
    public AudioClip[] SoundEffectClips;

    public enum DynamicSounds
    {
        Comet

		//Remember, if you're adding a new sound,
		//You still have to add it in the inspector to the 'Dynamic Sound Clips' array.
		//Make sure that the number above matches the element number in the array.
	}
	public AudioClip[] DynamicSoundClips;

    public static SoundBank Instance;

    void Awake()
    {
        Instance = this;
    }

    public AudioClip Request(Music musicClip)
    {
        int index = (int)musicClip;
        return MusicTracks[index];
    }

    public AudioClip Request(SoundEffects soundEffect)
    {
        int index = (int)soundEffect;
        return SoundEffectClips[index];
    }

    public AudioClip Request(DynamicSounds dynamic)
    {
        int index = (int)dynamic;
        return DynamicSoundClips[index];
    }
}
