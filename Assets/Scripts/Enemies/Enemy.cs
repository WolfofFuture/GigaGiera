using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 50;
    private int currentHealth;

    public int damageToPlayer = 10;
    public float moveSpeed = 3f;

    private Transform player;
    private bool playerInRoom = false;

    private float attackRange = 0.1f;
    private float attackCooldown = 1.7f;
    private float lastAttackTime;

    public string enemyID;
    private bool isDead = false;
    public GameObject lootObjectOnScene;

    private Animator animator;

    private AudioSource audioSource;

    [Header("Czy potwor moze przechodzic przez sciany (np. latajacy)")]
    public bool ignoreWalls = false;
    private Collider2D bodyCollider;
    private int wallMask;
    public AudioClip punchSound; 

    [Header("Unikalny identyfikator lootu (opcjonalnie taki sam jak enemyID + '_loot')")]
    public string lootID;


    void Start()
    {
        currentHealth = maxHealth;
        Player foundPlayer = FindAnyObjectByType<Player>();
        player = foundPlayer != null ? foundPlayer.transform : null;
        animator = GetComponent<Animator>();
        bodyCollider = GetComponent<Collider2D>();
        wallMask = LayerMask.GetMask("Blocking");
        audioSource = GetComponent<AudioSource>();

        if (GameStateManager.Instance.IsEnemyDefeated(enemyID))
        {
            Debug.Log($"[GameState] Przeciwnik {enemyID} już pokonany.");

            if (lootObjectOnScene != null)
            {
                if (!string.IsNullOrEmpty(lootID) && !GameStateManager.Instance.IsObjectDestroyed(lootID))
                {
                    lootObjectOnScene.SetActive(true);
                    Debug.Log($"[GameState] Loot {lootID} nie został zebrany — pokazuję.");
                }
                else
                {
                    lootObjectOnScene.SetActive(false);
                    Debug.Log($"[GameState] Loot {lootID} został zebrany — nie pokazuję.");
                }
            }

            Destroy(gameObject);
            return;
        }
    }


    void Update()
    {
        if (isDead) return;
        if (!playerInRoom) return;
        if (player == null) return;
        Vector3 scale = transform.localScale;
        if (player.position.x > transform.position.x)
        {
            scale.x = Mathf.Abs(scale.x); // patrzy w prawo
        }
        else
        {
            scale.x = -Mathf.Abs(scale.x); // patrzy w lewo
        }
        transform.localScale = scale;

        Vector3 direction = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            MoveTowards(direction * moveSpeed * Time.deltaTime);
         
        }
        else
        {

            if (Time.time - lastAttackTime > attackCooldown)
            {
                AttackPlayer();
                lastAttackTime = Time.time;
                animator.SetTrigger("Attack");
            }
        }
        
    }

    void MoveTowards(Vector3 step)
    {
        if (ignoreWalls || bodyCollider == null)
        {
            transform.position += step;
            return;
        }

        Physics2D.SyncTransforms();
        if (!IsBlocked(new Vector2(step.x, 0f)))
            transform.position += new Vector3(step.x, 0f, 0f);

        Physics2D.SyncTransforms();
        if (!IsBlocked(new Vector2(0f, step.y)))
            transform.position += new Vector3(0f, step.y, 0f);

        transform.position += new Vector3(0f, 0f, step.z);
    }

    bool IsBlocked(Vector2 move)
    {
        if (move == Vector2.zero)
            return false;

        Bounds bounds = bodyCollider.bounds;

        if (Physics2D.OverlapBox(bounds.center, bounds.size, 0f, wallMask) != null)
            return false;

        return Physics2D.OverlapBox((Vector2)bounds.center + move, bounds.size, 0f, wallMask) != null;
    }

    void AttackPlayer()
    {
        Player playerScript = player.GetComponent<Player>();
        if (playerScript != null)
        {
            playerScript.TakeDamage(damageToPlayer);
            Debug.Log("Enemy zaatakował gracza za " + damageToPlayer + " dmg.");

            if (audioSource != null && punchSound != null)
            {
                audioSource.PlayOneShot(punchSound);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("TakeHit");
        Debug.Log("Enemy otrzymał " + damage + " dmg.");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        animator.SetTrigger("Death");
        Debug.Log("Enemy zginął.");
        GameStateManager.Instance.MarkEnemyDefeated(enemyID);


        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero; // poprawione do .velocity

        if (lootObjectOnScene != null)
        {
            lootObjectOnScene.SetActive(true);
        }

        Destroy(gameObject, 1.5f);
    }

    public void PlayerEnteredRoom()
    {
        playerInRoom = true;
    }
}
