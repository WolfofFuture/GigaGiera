using UnityEngine;

public class MenuMusicPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1f); // domyœlnie 100%
        audioSource.Play();
        Debug.Log("Muzyka powinna siê odtwarzaæ!");
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
}
