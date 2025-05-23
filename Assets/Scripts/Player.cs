using UnityEngine;

public class Player : Mover
{
    private static Player instance;
    private SpriteRenderer spriteRenderer;

    //health
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;

    //inventory
    public GameObject inventoryUI;
    private bool isInventoryOpen = false;


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

        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }


        if (Input.GetKeyDown(KeyCode.P))
        {
            UseSpeedPotionIfAvailable();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            ToggleInvNotes();
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

    void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;

        Transform infoPanel = inventoryUI.transform.Find("info_and_items");
        if (infoPanel != null)
            infoPanel.gameObject.SetActive(isInventoryOpen);

        Time.timeScale = isInventoryOpen ? 0f : 1f;
    }
    
    void ToggleInvNotes()
    {
        isInventoryOpen = !isInventoryOpen;
        Transform infoPanel = inventoryUI.transform.Find("collected_notes");
        if (infoPanel != null)
            infoPanel.gameObject.SetActive(isInventoryOpen);
        Time.timeScale = isInventoryOpen ? 0f : 1f;
    }

    void UseSpeedPotionIfAvailable()
    {
        if (InventoryManager.Instance.HasItem("speed_potion"))
        {
            InventoryManager.Instance.RemoveItem("speed_potion");

            PlayerSpeedBoost speedBoost = GetComponent<PlayerSpeedBoost>();
            if (speedBoost != null)
            {
                speedBoost.ApplySpeedBoost(2f, 5f); // 2x szybkoœci przez 5 sekund
            }
        }
        else
        {
            Debug.Log("Nie masz mikstury prêdkoœci w ekwipunku!");
        }
    }
}
