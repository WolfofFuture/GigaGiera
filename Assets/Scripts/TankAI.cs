using UnityEngine;

public class TankAI : MonoBehaviour
{
    public AudioClip TankS1; // ryk potwora
    public AudioClip hitSound; // dŸwiêk uderzenia w gracza
    private AudioSource audioSource;

    private Transform player;
    private Player playerScript; // ¿eby odejmowaæ HP

    public float speed = 1.5f;

    private float attackCooldown = 2f;
    private float lastAttackTime = 0f;
    public int attackDamage = 40;

    void Start()
    {
        // Zmieniamy FindObjectOfType na FindFirstObjectByType - nowa, zalecana metoda
        playerScript = Object.FindFirstObjectByType<Player>();
        if (playerScript != null)
        {
            player = playerScript.transform;
        }
        else
        {
            Debug.LogWarning("COAACH! NIE MA TUTAJ GRACZA!! COAAACH!");
        }

        audioSource = GetComponent<AudioSource>();
        Invoke("Roar", 0.5f);
    }

    void Update()
    {
        if (player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * speed * Time.deltaTime);
    }

    void Roar()
    {
        if (TankS1 != null)
            audioSource.PlayOneShot(TankS1);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TryAttack();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
    }

    void TryAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    void Attack()
    {
        if (playerScript != null)
        {
            playerScript.TakeDamage(attackDamage);

            if (hitSound != null)
                audioSource.PlayOneShot(hitSound);
        }
    }
}
