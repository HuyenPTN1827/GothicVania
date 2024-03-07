using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicAudioSource;
    public AudioSource sfxAudioSource;
    public AudioSource flameAudioSource;

    public AudioClip musicClip;
    public AudioClip attackClip;
    public AudioClip hurtClip;
    public AudioClip jumpClip;
    public AudioClip dashClip;
    public AudioClip killClip;
    public AudioClip catDashClip;
    public AudioClip potionClip;
    public AudioClip flameClip;
    public AudioClip lightningClip;

    void Start()
    {
        musicAudioSource.clip = musicClip;
        musicAudioSource.Play();
    }

    public void PlaySfx(AudioClip sfxClip)
    {
        sfxAudioSource.clip = sfxClip;
        sfxAudioSource.PlayOneShot(sfxClip);
    }

    public void StartBreathFire(AudioClip fireClip)
    {
        flameAudioSource.clip = fireClip;
        flameAudioSource.loop = true;
        flameAudioSource.Play();
    }

    public void StopBreathFire(AudioClip fireClip)
    {
        flameAudioSource.clip = fireClip;
        flameAudioSource.loop = false;
        flameAudioSource.Stop();
    }
}
