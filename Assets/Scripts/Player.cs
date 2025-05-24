using UnityEngine;

public class Player : Mover
{
    public static Player Instance { get; private set; }
    private SpriteRenderer spriteRenderer;

    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;

    public GameObject deathPanel;

    [SerializeField] private GameObject inventoryUI;
    private bool isInventoryOpen = false;

    protected override void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (inventoryUI == null)
            Debug.LogWarning("inventoryUI NIE jest przypisany!");

        ResetPlayerState();

        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    public void ResetPlayerState()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth);

        if (deathPanel != null)
            deathPanel.SetActive(false);

        Time.timeScale = 1f;
        isInventoryOpen = false;
        canMove = true; // użycie canMove z Mover
    }

    private void Update()
    {
        if (deathPanel != null && deathPanel.activeSelf)
        {
            canMove = false;
            return;
        }
        else
        {
            canMove = true;
        }

        if (!canMove)
            return;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        UpdateMotor(new Vector3(x, y, 0));

        if (Input.GetKeyDown(KeyCode.I))
            ToggleInventory();

        if (Input.GetKeyDown(KeyCode.P))
            UseSpeedPotionIfAvailable();

        if (Input.GetKeyDown(KeyCode.N))
            ToggleInvNotes();

        if (Input.GetKeyDown(KeyCode.O))
        {
            InventoryManager inventory = InventoryManager.Instance;
            if (inventory.HasItem("health_potion"))
            {
                Heal(30);
                inventory.RemoveItem("health_potion");
                Debug.Log("Użyto mikstury zdrowia!");
            }
            else
            {
                Debug.Log("Nie masz mikstury zdrowia!");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);
    }

    void Die()
    {
        Debug.Log("Gracz zmarł!");
        if (deathPanel != null)
            deathPanel.SetActive(true);
        canMove = false;
    }

    void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        if (inventoryUI != null)
        {
            Transform infoPanel = inventoryUI.transform.Find("info_and_items");
            if (infoPanel != null)
                infoPanel.gameObject.SetActive(isInventoryOpen);
        }
        Time.timeScale = isInventoryOpen ? 0f : 1f;
    }

    void ToggleInvNotes()
    {
        isInventoryOpen = !isInventoryOpen;
        if (inventoryUI != null)
        {
            Transform infoPanel = inventoryUI.transform.Find("collected_notes");
            if (infoPanel != null)
                infoPanel.gameObject.SetActive(isInventoryOpen);
        }
        Time.timeScale = isInventoryOpen ? 0f : 1f;
    }

    void UseSpeedPotionIfAvailable()
    {
        if (InventoryManager.Instance.HasItem("speed_potion"))
        {
            InventoryManager.Instance.RemoveItem("speed_potion");

            PlayerSpeedBoost speedBoost = GetComponent<PlayerSpeedBoost>();
            if (speedBoost != null)
                speedBoost.ApplySpeedBoost(2f, 5f);
        }
        else
        {
            Debug.Log("Nie masz mikstury prędkości!");
        }
    }
}
