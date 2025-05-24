using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    public string itemID;
    public Sprite icon;

    public AudioClip pickupSound;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // Usuwamy automatyczne wy³¹czanie tutaj
        // Stan aktywnoœci bêdzie ustawiany z GameManagera lub CollectibleManagera
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!string.IsNullOrEmpty(itemID) && CollectibleManager.Instance != null)
            {
                CollectibleManager.Instance.Collect(itemID);
            }

            if (InventoryManager.Instance != null)
                InventoryManager.Instance.AddItem(icon, itemID);

            if (pickupSound != null)
            {
                audioSource.clip = pickupSound;
                audioSource.time = 0.5f;
                audioSource.Play();
            }

            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = false;

            Destroy(gameObject, 2f);
        }
    }

    /// <summary>
    /// Ustawia stan itemu na podstawie tego, czy zosta³ zebrany.
    /// </summary>
    public void InitializeCollectable()
    {
        if (CollectibleManager.Instance != null && CollectibleManager.Instance.IsCollected(itemID))
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);

            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = true;

            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = true;
        }
    }

    public void ResetCollectable()
    {
        gameObject.SetActive(true);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = true;
    }
}
