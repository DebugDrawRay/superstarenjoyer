using UnityEngine;
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
        StarGood,
        StarBad,
        ConstellationHit,
        ConstellationSent,
        ConstellationComplete,
        ConstellationBroken
    }
    public AudioClip[] SoundEffectClips;

    public enum DynamicSounds
    {
        Comet
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
