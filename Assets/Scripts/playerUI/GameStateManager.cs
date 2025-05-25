using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    public HashSet<string> collectedItems = new HashSet<string>();
    public HashSet<string> inventoryItems = new HashSet<string>();
    public HashSet<string> usedItems = new HashSet<string>();
    public HashSet<string> destroyedObjects = new HashSet<string>();
    public HashSet<string> defeatedEnemies = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // zachowuje dane miêdzy scenami
    }

    public void CollectItem(string itemID)
    {
        collectedItems.Add(itemID);
        inventoryItems.Add(itemID);
        Debug.Log($"[GameState] Zebrano item: {itemID}");
    }

    public bool HasItem(string itemID)
    {
        return inventoryItems.Contains(itemID);
    }

    public void UseItem(string itemID)
    {
        if (inventoryItems.Contains(itemID))
        {
            inventoryItems.Remove(itemID);
            usedItems.Add(itemID);
            Debug.Log($"[GameState] U¿yto item: {itemID}");
        }
        else
        {
            Debug.LogWarning($"[GameState] Próbujesz u¿yæ itemu, którego nie masz: {itemID}");
        }
    }

    public bool IsUsed(string itemID) => usedItems.Contains(itemID);
    public bool IsCollected(string itemID) => collectedItems.Contains(itemID);

    public void MarkObjectDestroyed(string objectID)
    {
        destroyedObjects.Add(objectID);
        Debug.Log($"[GameState] Obiekt zniszczony/otwarty: {objectID}");
    }

    public bool IsObjectDestroyed(string objectID) => destroyedObjects.Contains(objectID);

    public void MarkEnemyDefeated(string enemyID)
    {
        defeatedEnemies.Add(enemyID);
        Debug.Log($"[GameState] Pokonano przeciwnika: {enemyID}");
    }

    public bool IsEnemyDefeated(string enemyID) => defeatedEnemies.Contains(enemyID);

    public void ResetState()
    {
        collectedItems.Clear();
        inventoryItems.Clear();
        usedItems.Clear();
        destroyedObjects.Clear();
        defeatedEnemies.Clear();
        Debug.Log("[GameState] Stan gry zresetowany.");
    }
}
