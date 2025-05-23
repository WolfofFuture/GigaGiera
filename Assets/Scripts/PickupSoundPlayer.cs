using UnityEngine;

public class PickupSoundPlayer : MonoBehaviour
{
    public AudioSource pickupAudioSource;

    public void PlayPickupSound()
    {
        if (pickupAudioSource == null || pickupAudioSource.clip == null)
            return;

        pickupAudioSource.time = 0.5f;
        pickupAudioSource.Play();
    }
}
