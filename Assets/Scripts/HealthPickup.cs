using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 20;  // ile HP dodaje serduszko
    public AudioClip pickupSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (pickupSound != null)
        {
            audioSource.clip = pickupSound;
        }

        // Ustaw g³oœnoœæ zgodnie z PlayerPrefs (SFXVolume)
        float volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        audioSource.volume = volume;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player playerScript = other.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.Heal(healAmount);  // zak³adam ¿e masz tak¹ metodê
                if (pickupSound != null)
                    audioSource.Play();

                // Ukryj sprite i collider ¿eby nie da³o siê podnosiæ wielokrotnie
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<Collider2D>().enabled = false;

                // Zniszcz obiekt po czasie trwania dŸwiêku
                Destroy(gameObject, pickupSound.length);
            }
        }
    }
}
