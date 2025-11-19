using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using ToolBox.Utils;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int startingAudioObjectsCount;

    [SerializeField] float transitionFadeOutDelay;
    [SerializeField] float transitionFadeInDelay;

    [Space(10)]
    [SerializeField] Playlist[] playlist;

    [Header("References")]
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup soundMixerGroup;


    private Transform playlistParent = null;
    private Transform soundParent = null;
    private readonly Queue<AudioSource> soundsQueue = new();
    private List<AudioSource> audios = new();
    List<AudioSource> playlistAudios = new();
    int initialMusicCount;

    public static AudioManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetupAudioParent();

        SetupPlaylist(playlist);

        for (int i = 0; i < startingAudioObjectsCount; i++)
        {
            soundsQueue.Enqueue(CreateAudioSource(soundParent));
        }
    }

    private void SetupAudioParent()
    {
        playlistParent = new GameObject("PLAYLIST").transform;
        playlistParent.parent = transform;

        soundParent = new GameObject("SOUNDS").transform;
        soundParent.parent = transform;
    }

    private void ClearAllAudio()
    {
        foreach (AudioSource audio in audios)
        {
            audio.Stop();
        }
    }

    public void PlayClipAt(Sound sound, Vector3 position)
    {
        AudioSource audioSource;

        if (soundsQueue.Count <= 0)
        {
            audioSource = CreateAudioSource(soundParent);
        }
        else
        {
            audioSource = soundsQueue.Dequeue();
        }

        audioSource.transform.position = position;
        audioSource.clip = sound.clips.GetRandom();
        audioSource.volume = Mathf.Clamp(sound.volumeMultiplier, 0, 1);
        audioSource.spatialBlend = sound.spatialBlend;

        audioSource.Play();
        StartCoroutine(AddAudioSourceToQueue(audioSource));
    }

    private IEnumerator AddAudioSourceToQueue(AudioSource current)
    {
        float cooldown = current.clip.length;
        float timer = 0f;

        while (timer < cooldown)
        {
            yield return null;

            timer += Time.deltaTime;

            if (!current.isPlaying)
            {
                current.UnPause();
            }
        }

        soundsQueue.Enqueue(current);
    }

    private AudioSource CreateAudioSource(Transform parent)
    {
        AudioSource audioSource = new GameObject("AudioSource").AddComponent<AudioSource>();
        audioSource.transform.SetParent(parent);
        audioSource.outputAudioMixerGroup = soundMixerGroup;
        audios.Add(audioSource);
        return audioSource;
    }

    public void SetupPlaylist(Playlist[] playlists)
    {
        foreach (Playlist playlist in playlists)
        {
            AudioSource audioSource = CreatePlaylistAudioSource();
            audioSource.volume = playlist.volumMultiplier;
            audioSource.loop = playlist.isLooping;
            audioSource.clip = playlist.clip;
            audioSource.Play();
            playlistAudios.Add(audioSource);
        }
        initialMusicCount = playlists.Length;
    }
    AudioSource CreatePlaylistAudioSource()
    {
        AudioSource audioSource = new GameObject("Playlist").AddComponent<AudioSource>();
        audioSource.transform.SetParent(playlistParent);
        audioSource.outputAudioMixerGroup = musicMixerGroup;
        return audioSource;
    }

    void ChangeAmbianceMusic(Playlist[] playlists)
    {
        StartCoroutine(ChangeAmbianceMusicDelay(playlists));
    }
    IEnumerator ChangeAmbianceMusicDelay(Playlist[] playlists)
    {
        if (playlistAudios.Count > initialMusicCount)
        {
            for (int i = initialMusicCount; i < playlistAudios.Count; i++)
            {
                playlistAudios[i].DOFade(0, transitionFadeOutDelay).OnComplete(() =>
                {
                    Destroy(playlistAudios[i].gameObject);
                });
            }
        }

        yield return new WaitForSeconds(transitionFadeOutDelay);

        foreach (Playlist playlist in playlists)
        {
            AudioSource audioSource = CreatePlaylistAudioSource();
            audioSource.DOFade(playlist.volumMultiplier, transitionFadeInDelay);
            audioSource.loop = playlist.isLooping;
            audioSource.clip = playlist.clip;
            audioSource.Play();
            playlistAudios.Add(audioSource);
        }
    }
}