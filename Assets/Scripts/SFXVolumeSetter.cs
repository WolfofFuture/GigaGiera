using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXVolumeSetter : MonoBehaviour
{
    public AudioSource AudioSourceComponent { get; private set; }

    void Awake()
    {
        AudioSourceComponent = GetComponent<AudioSource>();
    }

    void Start()
    {
        float volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        AudioSourceComponent.volume = volume;
    }
}
