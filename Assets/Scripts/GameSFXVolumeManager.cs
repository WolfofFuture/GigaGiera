using UnityEngine;

public class GameSFXVolumeManager : MonoBehaviour
{
    public AudioSource stepAudioSource;      // przypisz w Inspectorze AudioSource kroków
    public AudioSource monsterAudioSource;   // przypisz w Inspectorze AudioSource potwora

    void Start()
    {
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);  // odczytaj wartoœæ z MainMenu
        SetSFXVolume(sfxVolume);
    }

    public void SetSFXVolume(float volume)
    {
        if (stepAudioSource != null)
            stepAudioSource.volume = volume;

        if (monsterAudioSource != null)
            monsterAudioSource.volume = volume;
    }
}
