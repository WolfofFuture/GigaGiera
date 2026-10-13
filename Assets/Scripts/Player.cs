using System.Linq;
using UnityEngine;

public class Player : Mover
{
    public static Player Instance { get; private set; }
    private SpriteRenderer spriteRenderer;

    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;
    public Animator animator;

    public GameObject deathPanel;

    [SerializeField] private GameObject inventoryUI;
    private float horizontalMove;
    private float verticalMove;
    private bool isDead = false;

    public GameObject InventoryUI => inventoryUI;

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

        if (healthBar == null)
        {
            healthBar = FindAnyObjectByType<HealthBar>();
            if (healthBar == null)
                Debug.LogWarning("Nie znaleziono HealthBar w scenie!");
        }

        isDead = false;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }

        if (deathPanel != null)
            deathPanel.SetActive(false);

        PauseLock.Clear();
        canMove = true;
    }

    private void Update()
    {
        if (isDead || (deathPanel != null && deathPanel.activeSelf))
        {
            canMove = false;
            return;
        }
        else
        {
            canMove = true;
        }

        PauseLock.Set("inventory", IsInventoryShown());
        if (PauseLock.IsLocked("pause") || PauseLock.IsLocked("note"))
            return;

        if (!canMove)
            return;

        Vector2 moveInput = GameInput.Move;
        horizontalMove = moveInput.x;
        verticalMove = moveInput.y;
        animator.SetFloat("X Speed", Mathf.Abs(horizontalMove));
        animator.SetFloat("Y Speed", Mathf.Abs(verticalMove));
        UpdateMotor(new Vector3(horizontalMove, verticalMove, 0));

        if (GameInput.Pressed(GameAction.Inventory))
            ToggleInventoryPanel("info_and_items");

        if (GameInput.Pressed(GameAction.SpeedPotion))
            UseSpeedPotionIfAvailable();

        if (GameInput.Pressed(GameAction.Notes))
            ToggleInventoryPanel("collected_notes");

        if (GameInput.Pressed(GameAction.HealPotion))
        {
            InventoryManager inventory = InventoryManager.Instance;
            if (inventory != null && inventory.itemIDs != null)
            {
                string match = inventory.itemIDs.FirstOrDefault(id => id != null && id.Contains("health_potion"));
                if (match != null)
                {
                    Heal(30);
                    inventory.RemoveItem(match);
                    Debug.Log($"Użyto mikstury zdrowia: {match}");
                }
                else
                {
                    Debug.Log("Nie masz mikstury zdrowia!");
                }
            }
            else
            {
                Debug.LogWarning("InventoryManager lub itemIDs jest null!");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (healthBar != null) healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        animator.SetTrigger("TakeDamage");
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
        isDead = true;
        Debug.Log("Gracz zmarł!");
        if (deathPanel != null)
            deathPanel.SetActive(true);

        animator.SetTrigger("Death");
        canMove = false;
    }

    bool IsInventoryShown()
    {
        if (inventoryUI == null || !inventoryUI.activeSelf)
            return false;

        return IsPanelActive("info_and_items") || IsPanelActive("collected_notes");
    }

    bool IsPanelActive(string panelName)
    {
        Transform panel = inventoryUI.transform.Find(panelName);
        return panel != null && panel.gameObject.activeSelf;
    }

    void ToggleInventoryPanel(string panelName)
    {
        if (inventoryUI == null)
            return;

        bool closing = inventoryUI.activeSelf && IsPanelActive(panelName);
        inventoryUI.SetActive(!closing);

        foreach (string name in new[] { "info_and_items", "collected_notes" })
        {
            Transform panel = inventoryUI.transform.Find(name);
            if (panel != null)
                panel.gameObject.SetActive(!closing && name == panelName);
        }

        PauseLock.Set("inventory", !closing);
    }

    void UseSpeedPotionIfAvailable()
    {
        if (InventoryManager.Instance == null || InventoryManager.Instance.itemIDs == null)
        {
            Debug.LogWarning("InventoryManager.Instance lub itemIDs jest null!");
            return;
        }

        string match = InventoryManager.Instance.itemIDs
            .FirstOrDefault(id => id != null && id.Contains("speed_potion"));

        if (match != null)
        {
            InventoryManager.Instance.RemoveItem(match);

            PlayerSpeedBoost speedBoost = GetComponent<PlayerSpeedBoost>();
            if (speedBoost != null)
                speedBoost.ApplySpeedBoost(2f, 5f);

            Debug.Log($"Użyto mikstury prędkości: {match}");
        }
        else
        {
            Debug.Log("Nie masz mikstury prędkości!");
        }
    }
}
