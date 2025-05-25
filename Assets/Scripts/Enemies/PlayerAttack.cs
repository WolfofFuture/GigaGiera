using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int damage = 10;
    public float attackRange = 0.2f;
    public float attackCooldown = 1f;
    private float lastAttackTime = -Mathf.Infinity;

    private CooldownBar cooldownBar;
    private Animator anim;

    public LayerMask enemyLayer; // ustaw na "Enemy" w inspektorze

    private Vector2 attackDirection = Vector2.right; // domyœlnie atak w prawo

    void Start()
    {
        anim = GetComponent<Animator>();

        GameObject playerUi = GameObject.Find("playerUi");
        if (playerUi != null)
        {
            Transform cooldownBarTransform = playerUi.transform.Find("CooldownBar");
            if (cooldownBarTransform != null)
            {
                cooldownBar = cooldownBarTransform.GetComponent<CooldownBar>();
                cooldownBar?.SetCooldownProgress(1f);
            }
        }
    }

    void Update()
    {
        float timeSinceLastAttack = Time.time - lastAttackTime;
        float progress = Mathf.Clamp01(timeSinceLastAttack / attackCooldown);
        cooldownBar?.SetCooldownProgress(progress);

        // Ruch kierunku ataku (mo¿esz to powi¹zaæ z ruchem gracza)
        if (Input.GetKey(KeyCode.W)) attackDirection = Vector2.up;
        if (Input.GetKey(KeyCode.S)) attackDirection = Vector2.down;
        if (Input.GetKey(KeyCode.A)) attackDirection = Vector2.left;
        if (Input.GetKey(KeyCode.D)) attackDirection = Vector2.right;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (timeSinceLastAttack >= attackCooldown)
            {
                anim?.SetTrigger("swing");
                Attack();
                lastAttackTime = Time.time;
            }
        }
    }

    void Attack()
    {
        Vector2 attackOrigin = (Vector2)transform.position + attackDirection.normalized * attackRange;

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackOrigin, attackRange, enemyLayer);

        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Gracz zaatakowa³ potwora za " + damage + " dmg.");
                break;
            }
        }

        // Debug gizmo (widzisz w Scene)
        Debug.DrawLine(transform.position, attackOrigin, Color.red, 0.2f);
    }

    void OnDrawGizmosSelected()
    {
        Vector2 attackOrigin = (Vector2)transform.position + attackDirection.normalized * attackRange;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackOrigin, attackRange);
    }
}
