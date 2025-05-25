using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAttack : MonoBehaviour
{
    public int damage = 10;
    public float attackRange = 0.2f;
    public float attackCooldown = 1f;

    public AudioClip swingSound; // dŸwiêk ataku, przypisz w Inspectorze
    private AudioSource audioSource;

    private float lastAttackTime = -Mathf.Infinity;

    private CooldownBar cooldownBar;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        // Pobranie AudioSource i ustawienie g³oœnoœci z PlayerPrefs
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        audioSource.enabled = audioSource.volume > 0f;

        // Je¿eli jest komponent SFXVolumeSetter – przypisz
        SFXVolumeSetter sfxSetter = GetComponent<SFXVolumeSetter>();
        if (sfxSetter == null)
        {
            sfxSetter = gameObject.AddComponent<SFXVolumeSetter>();
        }

        // Cooldown bar
        GameObject playerUi = GameObject.Find("playerUi");
        if (playerUi != null)
        {
            Transform cooldownBarTransform = playerUi.transform.Find("CooldownBar");
            if (cooldownBarTransform != null)
            {
                cooldownBar = cooldownBarTransform.GetComponent<CooldownBar>();
                if (cooldownBar != null)
                {
                    cooldownBar.SetCooldownProgress(1f);
                }
                else
                {
                    Debug.LogError("Brak skryptu CooldownBar na CooldownBar!");
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
                    anim.SetTrigger("swing");
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

                break; // tylko pierwszy trafiony wróg
            }
        }

        // DŸwiêk ataku zaczynaj¹cy siê od 0.2 sekundy
        if (swingSound != null && audioSource != null && audioSource.enabled)
        {
            audioSource.clip = swingSound;
            audioSource.time = 0.2f;
            audioSource.Play();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
