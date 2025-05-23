using UnityEngine;

public class MenuMusicPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        SetVolume(savedVolume);
    }

    void Update()
    {
        // Wymuszamy stan przy ka¿dej klatce - zapobiegnie nadpisaniu przez inne skrypty
        float volume = PlayerPrefs.GetFloat("MusicVolume", 1f);

        if (volume <= 0f)
        {
            if (audioSource.isPlaying)
                audioSource.Pause();
            audioSource.mute = true;
        }
        else
        {
            audioSource.mute = false;
            audioSource.volume = volume;
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
    }

    public void SetVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
}
