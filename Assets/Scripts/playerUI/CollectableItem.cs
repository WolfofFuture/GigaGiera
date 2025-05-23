using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    public string itemID;
    public Sprite icon;

    public AudioClip pickupSound;
    private AudioSource audioSource;

    private void Start()
    {
        // Pobierz lub dodaj AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Ustaw g³oœnoœæ z PlayerPrefs (z suwaka SFXVolumeManager)
        audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // Jeœli przedmiot ju¿ by³ zebrany wczeœniej – ukryj go
        if (CollectibleManager.Instance != null && CollectibleManager.Instance.IsCollected(itemID))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!string.IsNullOrEmpty(itemID))
            {
                CollectibleManager.Instance.Collect(itemID);
            }

            InventoryManager.Instance.AddItem(icon);

            if (pickupSound != null)
            {
                audioSource.clip = pickupSound;
                audioSource.time = 0.5f;
                audioSource.Play();
            }

            // Natychmiastowe ukrycie obiektu (z mapy)
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = false;

            Destroy(gameObject, 2f); // po 2 sekundach (¿eby dŸwiêk zd¹¿y³ siê odtworzyæ)
        }
    }
}
