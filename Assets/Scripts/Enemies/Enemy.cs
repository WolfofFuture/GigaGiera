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

    void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();

        if (CollectibleManager.Instance.IsCollected(enemyID))
        {
            Debug.Log($"Enemy {enemyID} został już zabity, usuwam go.");
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        if (isDead) return;    
        if (!playerInRoom) return;

        if (!playerInRoom) return;

        Vector3 direction = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            transform.position += direction * moveSpeed * Time.deltaTime;
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);

            if (Time.time - lastAttackTime > attackCooldown)
            {
                AttackPlayer();
                lastAttackTime = Time.time;
                animator.SetTrigger("Attack");
            }
        }
    }

    void AttackPlayer()
    {
        Player playerScript = player.GetComponent<Player>();
        if (playerScript != null)
        {
            playerScript.TakeDamage(damageToPlayer);
            Debug.Log("Enemy zaatakowa� gracza za " + damageToPlayer + " dmg.");
        }
    }


    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("TakeHit");
        Debug.Log("Enemy otrzyma� " + damage + " dmg.");

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
        CollectibleManager.Instance.Collect(enemyID);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // Jeśli lootPrefab jest przypisany, twórz item
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
