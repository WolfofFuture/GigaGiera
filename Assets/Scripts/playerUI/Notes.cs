using UnityEngine;
using UnityEngine.UI;

public class NoteCollectable : MonoBehaviour
{
    [Header("Unikalny ID do zapisu (np. note_enemy1)")]
    public string noteID; // odpowiednik lootID

    [Header("UI ID notatki")]
    public string noteUIName;

    [Header("ID notatki w inventory UI")]
    public string inventoryName;

    private void Start()
    {
        // Jeśli została już zebrana — ukryj
        if (!string.IsNullOrEmpty(noteID) && GameStateManager.Instance.IsObjectDestroyed(noteID))
        {
            Debug.Log($"[GameState] Notatka {noteID} została już zebrana — ukrywam.");
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // 🔹 Zapisz jako zebrana
        if (!string.IsNullOrEmpty(noteID))
        {
            GameStateManager.Instance.MarkObjectDestroyed(noteID);
            Debug.Log($"[GameState] Notatka {noteID} została zebrana — zapisana w stanie.");
        }

        // 🔹 Pokaż treść notatki (UI)
        GameObject noteUI = GameObject.Find("NoteUI");
        if (noteUI != null)
        {
            Transform note = noteUI.transform.Find(noteUIName);
            if (note != null)
            {
                note.gameObject.SetActive(true);
                Time.timeScale = 0f;

                Button closeButton = note.GetComponentInChildren<Button>();
                if (closeButton != null)
                {
                    closeButton.onClick.RemoveAllListeners();
                    closeButton.onClick.AddListener(() =>
                    {
                        note.gameObject.SetActive(false);
                        Time.timeScale = 1f;
                    });
                }
            }
        }

        // 🔹 Odsłoń w inventory
        GameObject inventoryUI = GameObject.Find("InventoryUI");
        if (inventoryUI != null)
        {
            Transform collectedNotes = inventoryUI.transform.Find("collected_notes");

            if (collectedNotes != null)
            {
                foreach (Transform page in collectedNotes)
                {
                    if (page.name.StartsWith("page"))
                    {
                        Transform collectedNote = FindDeepChild(page, inventoryName);
                        if (collectedNote != null)
                        {
                            collectedNote.gameObject.SetActive(true);
                            break;
                        }
                    }
                }
            }
        }

        gameObject.SetActive(false);
    }

    private Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform result = FindDeepChild(child, name);
            if (result != null)
                return result;
        }
        return null;
    }
}
