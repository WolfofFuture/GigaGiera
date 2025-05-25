using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int damage = 10;
    public float attackRange = 1.5f;

    public float attackCooldown = 1f;
    private float lastAttackTime = -Mathf.Infinity;

    private CooldownBar cooldownBar;
    private Animator anim; 

    void Start()
    {
        anim = GetComponent<Animator>(); // tutaj aniamtor ok?

        GameObject playerUi = GameObject.Find("playerUi");
        if (playerUi != null)
        {
            Transform cooldownBarTransform = playerUi.transform.Find("CooldownBar");
            if (cooldownBarTransform != null)
            {
                cooldownBar = cooldownBarTransform.GetComponent<CooldownBar>();
                if (cooldownBar == null)
                {
                    Debug.LogError("Brak skryptu CooldownBar na CooldownBar!");
                }
                else
                {
                    cooldownBar.SetCooldownProgress(1f); 
                }
            }
            else
            {
                Debug.LogError("Nie znaleziono CooldownBar w playerUi!");
            }
        }
        else
        {
            Debug.LogError("Nie znaleziono playerUi w scenie!");
        }
    }

    void Update()
    {
        float timeSinceLastAttack = Time.time - lastAttackTime;
        float progress = Mathf.Clamp01(timeSinceLastAttack / attackCooldown);

        if (cooldownBar != null)
        {
            cooldownBar.SetCooldownProgress(progress);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (timeSinceLastAttack >= attackCooldown)
            {
                if (anim != null)
                {
                    anim.SetTrigger("swing"); //uruchomia animacje na spacji
                }

                Attack();
                lastAttackTime = Time.time;
            }
            else
            {
                Debug.Log("Cooldown active, cannot attack yet!");
            }
        }
    }

    void Attack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
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
    }
}
