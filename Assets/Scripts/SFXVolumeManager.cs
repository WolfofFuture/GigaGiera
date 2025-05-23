using UnityEngine;
using UnityEngine.UI;

public class SFXVolumeManager : MonoBehaviour
{
    public Slider sfxSlider;
    public AudioSource stepAudioSource;
    public AudioSource monsterAudioSource;

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        sfxSlider.value = savedVolume;
        SetSFXVolume(savedVolume);

        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
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
    }
}
