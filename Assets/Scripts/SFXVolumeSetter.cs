using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXVolumeSetter : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        float volume = PlayerPrefs.GetFloat("SFXVolume", 1f); // domyœlnie pe³na g³oœnoœæ
        audioSource.volume = volume;
    }
}
