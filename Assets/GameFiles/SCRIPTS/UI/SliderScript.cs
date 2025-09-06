using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SliderScript : MonoBehaviour
{
    public AudioSource[] audioSources;
    private const string VolumeKey = "AudioVolume";
    private float lastVolume;

    Slider volumeSlider;
    AudioSource[] allAudioSources;

    void Start()
    {

        volumeSlider = GetComponent<Slider>();

        float savedVolume = PlayerPrefs.GetFloat("VolumeLevel", 1f);
        volumeSlider.value = savedVolume;

        allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in allAudioSources)
        {
            audio.volume = savedVolume;
        }

        volumeSlider.onValueChanged.AddListener(UpdateVolume);
    }
    private void Awake()
    {
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        SetAllVolumes(savedVolume);
        lastVolume = savedVolume;
    }
    private void Update()
    {
        if (audioSources.Length == 0) return;

        float currentVolume = audioSources[0].volume;

        if (Mathf.Abs(currentVolume - lastVolume) > 0.001f)
        {
            SetAllVolumes(currentVolume);
            PlayerPrefs.SetFloat(VolumeKey, currentVolume);
            PlayerPrefs.Save();
            lastVolume = currentVolume;
        }
    }
    private void SetAllVolumes(float volume)
    {
        foreach (var source in audioSources)
        {
            if (source != null)
                source.volume = volume;
        }
    }

    void UpdateVolume(float newVolume)
    {
        foreach (AudioSource audio in allAudioSources)
        {
            audio.volume = newVolume;
        }

        PlayerPrefs.SetFloat("VolumeLevel", newVolume);
    }
}