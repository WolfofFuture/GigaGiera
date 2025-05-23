using UnityEngine;

public class CollectablePickupSound : MonoBehaviour
{
    public AudioClip pickupClip; // przypisz dŸwiêk w inspektorze
    private AudioSource audioSource;

    private void Awake()
    {
        // Dodajemy AudioSource dynamicznie, jeœli nie ma
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.clip = pickupClip;
    }

    public void PlayPickupSound()
    {
        if (audioSource == null || pickupClip == null) return;

        float volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        audioSource.volume = volume;

        audioSource.time = 0.5f; // start od 0.5 sekundy
        audioSource.Play();
    }
}
