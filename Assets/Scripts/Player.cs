using UnityEngine;

public class Player : Mover
{
    private static Player instance;
    private SpriteRenderer spriteRenderer;

    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;

    protected override void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;

        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth);
    }

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        UpdateMotor(new Vector3(x, y, 0));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(20);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        healthBar.SetHealth(currentHealth);
    }
}
