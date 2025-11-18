using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    //[Header("Settings")]

    private float musicVolume, sfxVolume = 1f;

    //[Header("References")]

    [SerializeField] AudioMixer audioMixer;

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        audioMixer.GetFloat("MusicVolume", out musicVolume);

        audioMixer.GetFloat("SFXVolume", out sfxVolume);
    }

    public void SetMusicVolume(float volume)
    {
        float volumeInDB = Mathf.Log10(volume) * 20;
        if (volume <= 0.0001f)
        {
            audioMixer.SetFloat("MusicVolume", -80f);
            musicVolume = 0f;
        }
        else
        {
            audioMixer.SetFloat("MusicVolume", volumeInDB);
            musicVolume = volume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        float volumeInDB = Mathf.Log10(volume) * 20;
        if (volume <= 0.0001f)
        {
            audioMixer.SetFloat("SFXVolume", -80f);
            sfxVolume = 0f;
        }
        else
        {
            audioMixer.SetFloat("SFXVolume", volumeInDB);
            sfxVolume = volume;
        }
    }
}