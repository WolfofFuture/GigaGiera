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
    private bool isInventoryOpen = false;
    private float horizontalMove;
    private float verticalMove;

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

        // ✅ Dodane przypisanie healthBar, jeśli null
        if (healthBar == null)
        {
            healthBar = FindObjectOfType<HealthBar>();
            if (healthBar == null)
                Debug.LogWarning("Nie znaleziono HealthBar w scenie!");
        }

        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth);

        if (deathPanel != null)
            deathPanel.SetActive(false);

        Time.timeScale = 1f;
        isInventoryOpen = false;
        canMove = true;
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

        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");
        animator.SetFloat("X Speed", Mathf.Abs(horizontalMove));
        animator.SetFloat("Y Speed", Mathf.Abs(verticalMove));
        UpdateMotor(new Vector3(horizontalMove, verticalMove, 0));

        if (Input.GetKeyDown(KeyCode.I))
            ToggleInventory();

        if (Input.GetKeyDown(KeyCode.P))
            UseSpeedPotionIfAvailable();

        if (Input.GetKeyDown(KeyCode.N))
            ToggleInvNotes();

        if (Input.GetKeyDown(KeyCode.O))
        {
            InventoryManager inventory = InventoryManager.Instance;
            string match = inventory.itemIDs.FirstOrDefault(id => id.Contains("health_potion"));
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
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
            Die();

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
        Debug.Log("Gracz zmarł!");
        if (deathPanel != null)
            deathPanel.SetActive(true);

        animator.SetTrigger("Death");
        canMove = false;
    }

    void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;

        if (inventoryUI != null)
        {
            // Włącz cały panel ekwipunku
            inventoryUI.SetActive(isInventoryOpen);

            // (opcjonalnie) podpanel info_and_items
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
            // Włącz cały panel ekwipunku
            inventoryUI.SetActive(isInventoryOpen);

            // (opcjonalnie) podpanel collected_notes
            Transform notesPanel = inventoryUI.transform.Find("collected_notes");
            if (notesPanel != null)
                notesPanel.gameObject.SetActive(isInventoryOpen);
        }

        Time.timeScale = isInventoryOpen ? 0f : 1f;
    }

    void UseSpeedPotionIfAvailable()
    {
        string match = InventoryManager.Instance.itemIDs.FirstOrDefault(id => id.Contains("speed_potion"));
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
