using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public enum ObjectType { Door, DestructibleBox }

    [Header("Typ obiektu")]
    public ObjectType objectType;

    [Header("ID przedmiotu potrzebnego do odblokowania/wybuchu")]
    public string requiredItemID;

    [Header("SpriteRenderer na tym obiekcie")]
    public SpriteRenderer spriteRenderer;  // Przypisz tutaj komponent SpriteRenderer

    [Header("Sprite'y do zmiany")]
    public Sprite closedSprite;   // Sprite przed odblokowaniem (np. zdrowy box)
    public Sprite openSprite;     // Sprite po odblokowaniu (np. zniszczony box)

    [Header("Collider rodzica (fizyczny)")]
    public Collider2D parentCollider; // Fizyczny collider na rodzicu

    [Header("Unikalny identyfikator tego obiektu (np. 'door_1')")]
    public string uniqueID;


    private Collider2D col; // Trigger collider (dziecka z tym skryptem)
    private bool isUnlocked = false;

    private bool playerNearby = false;
    private FloatingTextManager floatingTextManager;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        if (col == null)
            Debug.LogError($"{gameObject.name}: Brak Collider2D!");


        // Ustaw sprite pocz¹tkowy
        if (spriteRenderer != null && closedSprite != null)
        {
            spriteRenderer.sprite = closedSprite;
        }

        if (GameStateManager.Instance.IsObjectDestroyed(uniqueID))
        {
            Unlock();
        }
        else
        {
            Lock();
        }

    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (floatingTextManager == null)
        {
            floatingTextManager = FindObjectOfType<FloatingTextManager>();
            if (floatingTextManager == null)
            {
                Debug.LogError("Nie znaleziono FloatingTextManager w OnTriggerEnter2D!");
                return;
            }
        }

        if (other.CompareTag("Player") && !isUnlocked)
        {
            playerNearby = true;
            floatingTextManager.Show("Press E to interact", 24, Color.white,
                transform.position + Vector3.up * 1.5f, Vector3.up * 0.5f, 2f);
            Debug.Log($"{gameObject.name}: Wyœwietlam komunikat 'Press E to interact'");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            Debug.Log($"{gameObject.name}: Gracz opuœci³ trigger.");
        }
    }

    private void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"{gameObject.name}: Gracz nacisn¹³ E - próba interakcji.");
            TryInteract();
        }
    }

    private void TryInteract()
    {
        bool hasRequiredItem = false;
        string failMessage = "Hm... Something is missing"; // domyœlny komunikat

        if (objectType == ObjectType.DestructibleBox)
        {
            foreach (string collectedItem in GameStateManager.Instance.inventoryItems)
            {
                if (collectedItem.StartsWith("bomb_"))
                {
                    requiredItemID = collectedItem; // zapamiêtaj, którego u¿ywamy
                    hasRequiredItem = true;
                    break;
                }
            }

            if (!hasRequiredItem)
                failMessage = "Hm... I am too weak"; // komunikat dla boxa
        }
        else if (objectType == ObjectType.Door)
        {
            foreach (string collectedItem in GameStateManager.Instance.inventoryItems)
            {
                if (collectedItem.StartsWith("key_"))
                {
                    requiredItemID = collectedItem; // zapamiêtaj, którego u¿ywamy
                    hasRequiredItem = true;
                    break;
                }
            }

            if (!hasRequiredItem)
                failMessage = "Hm... Something is missing"; // komunikat dla drzwi
        }
        else
        {
            hasRequiredItem = GameStateManager.Instance.IsCollected(requiredItemID);
        }

        if (hasRequiredItem)
        {
            Unlock(); // w œrodku: UseItem i RemoveItem
            GameStateManager.Instance.MarkObjectDestroyed(uniqueID);

        }
        else
        {
            if (floatingTextManager != null)
            {
                floatingTextManager.Show(failMessage, 24, Color.red,
                    transform.position + Vector3.up * 1.5f, Vector3.up * 0.5f, 2f);
            }
            else
            {
                Debug.LogError("floatingTextManager jest null – nie mogê wyœwietliæ komunikatu o s³aboœci.");
            }
        }
    }



    private void Unlock()
    {
        isUnlocked = true;

        if (GameStateManager.Instance != null)
            GameStateManager.Instance.UseItem(requiredItemID);  // U¿yj (usuñ) klucz

        if (InventoryManager.Instance != null)
            InventoryManager.Instance.RemoveItem(requiredItemID); // Usuñ z ekwipunku

        // Zmieñ sprite na openSprite
        if (spriteRenderer != null && openSprite != null)
        {
            spriteRenderer.sprite = openSprite;
        }

        // Wy³¹cz collidery
        if (col != null) col.enabled = false;
        if (parentCollider != null) parentCollider.enabled = false;

        Debug.Log($"{gameObject.name} zosta³ odblokowany / zniszczony.");
    }


    private void Lock()
    {
        isUnlocked = false;

        // Przywróæ sprite closedSprite
        if (spriteRenderer != null && closedSprite != null)
        {
            spriteRenderer.sprite = closedSprite;
        }

        if (col != null) col.enabled = true;
        if (parentCollider != null) parentCollider.enabled = true;
    }
}
