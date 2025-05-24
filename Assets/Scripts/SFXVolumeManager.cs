using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SFXVolumeManager : MonoBehaviour
{
    public Slider sfxSlider;
    public AudioSource stepAudioSource;
    public AudioSource monsterAudioSource;

    // Lista audio Ÿróde³ przeciwników, które chcemy kontrolowaæ globalnie
    private List<AudioSource> enemyAudioSources = new List<AudioSource>();

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        sfxSlider.value = savedVolume;
        SetSFXVolume(savedVolume);

        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        // ZnajdŸ wszystkich przeciwników z komponentem SFXVolumeSetter i dodaj ich AudioSource
        SFXVolumeSetter[] enemySFXs = FindObjectsOfType<SFXVolumeSetter>();
        foreach (var sfxSetter in enemySFXs)
        {
            if (sfxSetter.AudioSourceComponent != null)
                enemyAudioSources.Add(sfxSetter.AudioSourceComponent);
        }
    }

    public void OnSFXVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
        SetSFXVolume(value);
    }

    private void SetSFXVolume(float value)
    {
        if (stepAudioSource != null)
        {
            stepAudioSource.volume = value;
            stepAudioSource.enabled = value > 0f;
        }

        if (monsterAudioSource != null)
        {
            monsterAudioSource.volume = value;
            monsterAudioSource.enabled = value > 0f;
        }

        foreach (var audioSrc in enemyAudioSources)
        {
            audioSrc.volume = value;
            audioSrc.enabled = value > 0f;
        }
    }
}
