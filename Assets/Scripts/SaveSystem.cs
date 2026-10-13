using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class SaveData
{
    public int version = 1;
    public string savedAt;

    public string scene;
    public float posX;
    public float posY;
    public int health;

    public List<string> collectedItems = new List<string>();
    public List<string> inventoryItems = new List<string>();
    public List<string> usedItems = new List<string>();
    public List<string> destroyedObjects = new List<string>();
    public List<string> defeatedEnemies = new List<string>();
    public List<string> collectedNotes = new List<string>();

    // Przedmioty w ekwipunku w kolejnosci slotow.
    public List<string> inventorySlots = new List<string>();
}

public static class SaveSystem
{
    private static string FilePath
    {
        get { return Path.Combine(Application.persistentDataPath, "save.json"); }
    }

    public static bool Exists()
    {
        return File.Exists(FilePath);
    }

    // Pokoj, w ktorym jest gracz (scena inna niz Player i MainMenu).
    public static string CurrentRoomScene()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);
            if (s.isLoaded && s.name != "Player" && s.name != "MainMenu")
                return s.name;
        }
        return null;
    }

    public static bool Save()
    {
        GameStateManager state = GameStateManager.Instance;
        Player player = Player.Instance;
        string room = CurrentRoomScene();

        if (state == null || player == null || string.IsNullOrEmpty(room))
        {
            Debug.LogWarning("[SaveSystem] Nie mozna zapisac: brak stanu gry, gracza albo pokoju.");
            return false;
        }

        SaveData data = new SaveData();
        data.savedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        data.scene = room;
        data.posX = player.transform.position.x;
        data.posY = player.transform.position.y;
        data.health = player.currentHealth;

        data.collectedItems = state.collectedItems.ToList();
        data.inventoryItems = state.inventoryItems.ToList();
        data.usedItems = state.usedItems.ToList();
        data.destroyedObjects = state.destroyedObjects.ToList();
        data.defeatedEnemies = state.defeatedEnemies.ToList();
        data.collectedNotes = state.collectedNotes.ToList();

        if (InventoryManager.Instance != null && InventoryManager.Instance.itemIDs != null)
            data.inventorySlots = InventoryManager.Instance.itemIDs.Where(id => id != null).ToList();

        try
        {
            string temp = FilePath + ".tmp";
            File.WriteAllText(temp, JsonUtility.ToJson(data, true));
            File.Copy(temp, FilePath, true);
            File.Delete(temp);
            Debug.Log("[SaveSystem] Zapisano gre: " + FilePath);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("[SaveSystem] Blad zapisu: " + e.Message);
            return false;
        }
    }

    public static SaveData Read()
    {
        if (!Exists())
            return null;

        try
        {
            return JsonUtility.FromJson<SaveData>(File.ReadAllText(FilePath));
        }
        catch (Exception e)
        {
            Debug.LogError("[SaveSystem] Blad odczytu: " + e.Message);
            return null;
        }
    }

    public static void Delete()
    {
        if (Exists())
            File.Delete(FilePath);
    }
}
