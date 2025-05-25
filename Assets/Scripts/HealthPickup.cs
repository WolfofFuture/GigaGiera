using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 20;  // ile HP dodaje serduszko
    public AudioClip pickupSound;

    [Header("Unikalny ID serduszka do zapisu (np. heart_01)")]
    public string heartID;


    private AudioSource audioSource;

    void Start()
    {
        if (!string.IsNullOrEmpty(heartID) &&
        GameStateManager.Instance != null &&
        GameStateManager.Instance.IsCollected(heartID))
        {
            Debug.Log($"[GameState] Serduszko {heartID} ju¿ zebrane – ukrywam.");
            gameObject.SetActive(false);
        }

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
                if (!string.IsNullOrEmpty(heartID) && GameStateManager.Instance != null)
                {
                    GameStateManager.Instance.CollectItem(heartID);
                    Debug.Log($"[GameState] Zebrano serduszko: {heartID}");
                }
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
