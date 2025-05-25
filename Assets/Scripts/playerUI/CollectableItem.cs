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

        if (GameStateManager.Instance != null && GameStateManager.Instance.IsCollected(itemID))
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
                GameStateManager.Instance.CollectItem(itemID);
            }


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

    public void ResetCollectable()
    {
        gameObject.SetActive(true);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = true;
    }
}
