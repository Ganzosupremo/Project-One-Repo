using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
public class MusicManager : SingletonMonoBehaviour<MusicManager>
{
    private AudioSource musicSource = null;
    private AudioClip currentmusicClip = null;

    private Coroutine fadeOutMusicCoroutine;
    private Coroutine fadeInMusicCoroutine;

    public int musicVolume;

    protected override void Awake()
    {
        base.Awake();

        musicSource = GetComponent<AudioSource>();

        //Start with the music off
        GameResources.Instance.musicOff.TransitionTo(0f);
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey(nameof(musicVolume)))
        {
            musicVolume = PlayerPrefs.GetInt(nameof(musicVolume));
        }

        SetMusicVolume(musicVolume);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetInt(nameof(musicVolume), musicVolume);
    }

    public void PlayMusic(MusicTrackSO musicTrackSO, float fadeOutTime = Settings.musicFadeOutTiem, float fadeInTime = Settings.musicFadeInTime)
    {
        StartCoroutine(PlayMusicRoutine(musicTrackSO, fadeOutTime, fadeInTime));
    }

    private IEnumerator PlayMusicRoutine(MusicTrackSO musicTrackSO, float fadeOutTime, float fadeInTime)
    {
        if (fadeOutMusicCoroutine != null)
        {
            StopCoroutine(fadeOutMusicCoroutine);
        }

        if (fadeInMusicCoroutine != null)
        {
            StopCoroutine(fadeInMusicCoroutine);
        }

        int index = Random.Range(0, musicTrackSO.musicClip.Length);

        //If the music track changed, play the new music
        if (musicTrackSO.musicClip[index] != currentmusicClip)
        {
            currentmusicClip = musicTrackSO.musicClip[index];

            yield return fadeOutMusicCoroutine = StartCoroutine(FadeOutMusic(fadeOutTime));

            yield return fadeInMusicCoroutine = StartCoroutine(FadeInMusic(musicTrackSO, fadeInTime, index));
        }

        yield return null;
    }

    /// <summary>
    /// Fades out the music
    /// </summary>
    private IEnumerator FadeOutMusic(float fadeOutTime)
    {
        GameResources.Instance.musicOnLow.TransitionTo(fadeOutTime);

        yield return new WaitForSeconds(fadeOutTime);
    }

    /// <summary>
    /// Fades in the music
    /// </summary>
    private IEnumerator FadeInMusic(MusicTrackSO musicTrackSO, float fadeInTime, int index)
    {
        //Set the clip and play it
        musicSource.clip = musicTrackSO.musicClip[index];
        musicSource.volume = musicTrackSO.musicVolume;
        musicSource.Play();

        GameResources.Instance.musicOnFull.TransitionTo(fadeInTime);

        yield return new WaitForSeconds(fadeInTime);
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

/// <summary>
/// Increase the music volume
/// </summary>
public void IncreaseMusicVolume()
    {
        int maxVolume = 20;

        if (musicVolume >= maxVolume) return;

        musicVolume += 1;
        SetMusicVolume(musicVolume);
    }

    /// <summary>
    /// Decrease the music volume
    /// </summary>
    public void DecreaseMusicVolume()
    {
        if (musicVolume == 0) return;

        musicVolume -= 1;

        SetMusicVolume(musicVolume);
    }

    /// <summary>
    /// Set the volume of the music
    /// </summary>
    private void SetMusicVolume(int musicVolume)
    {
        float muteDecibels = -80f;

        if (musicVolume == 0)
        {
            GameResources.Instance.musicMasterMixerGroup.audioMixer.SetFloat("MusicVolume", muteDecibels);
        }
        else
        {
            GameResources.Instance.musicMasterMixerGroup.audioMixer.SetFloat("MusicVolume", HelperUtilities.LinearToDecibels(musicVolume));
        }
    }
}
