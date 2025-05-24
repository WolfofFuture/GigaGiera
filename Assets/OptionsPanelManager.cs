using UnityEngine;
using UnityEngine.UI;

public class OptionsPanelManager : MonoBehaviour
{
    public GameObject optionsPanel; // Podstawowy panel opcji (Canvas > OptionsPanel)
    public Slider sfxSlider;
    public AudioSource stepAudioSource;
    public AudioSource monsterAudioSource;

    void Start()
    {
        optionsPanel.SetActive(false); // Panel domyœlnie ukryty

        float savedVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        sfxSlider.value = savedVolume;
        SetSFXVolume(savedVolume);

        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    public void ToggleOptionsPanel()
    {
        bool isActive = optionsPanel.activeSelf;
        optionsPanel.SetActive(!isActive);
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
            stepAudioSource.volume = value;
        if (monsterAudioSource != null)
            monsterAudioSource.volume = value;
    }
}
