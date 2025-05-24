using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Trap : MonoBehaviour
{
    public int damageAmount = 20;
    public AudioClip trapSound;
    public Sprite activatedSprite; // sprite po aktywacji
    private bool hasActivated = false;

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        float volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        audioSource.volume = volume;
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasActivated) return;

        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damageAmount);

                if (trapSound != null)
                {
                    audioSource.clip = trapSound;
                    audioSource.time = 2f; // start od 2. sekundy
                    audioSource.Play();
                }

                if (activatedSprite != null)
                    spriteRenderer.sprite = activatedSprite;

                hasActivated = true;
            }
        }
    }
}
