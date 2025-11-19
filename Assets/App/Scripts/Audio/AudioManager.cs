using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //[Header("Settings")]

    public static AudioManager Instance;
    private bool lastMusicWasA;

    //[Header("References")]

    [SerializeField] AudioSource musicSource, sfxSource;
    [SerializeField] AudioClip musicClipA, musicClipB;

    //[Header("Input")]
    //[Header("Output")]

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        int randomStart = Random.Range(0, 2);
        if (randomStart == 0)
        {
            musicSource.clip = musicClipA;
            lastMusicWasA = true;
        } else
        {
            musicSource.clip = musicClipB;
            lastMusicWasA = false;
        }
        musicSource.Play();
        StartCoroutine(WaitForNextMusic());
    }

    IEnumerator WaitForNextMusic()
    {
        float songDuration = musicSource.clip.length;

        yield return new WaitForSeconds(songDuration);
        SelectNextMusic();
    }
    private void SelectNextMusic()
    {
        if (lastMusicWasA)
        {
            musicSource.clip = musicClipB;
        } else
        {
            musicSource.clip = musicClipA;
        }
        musicSource.Play();
        StartCoroutine(WaitForNextMusic());
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PauseMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }
}