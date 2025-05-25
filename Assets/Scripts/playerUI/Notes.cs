using UnityEngine;
using UnityEngine.UI;

public class NoteCollectable : MonoBehaviour
{
    public string itemID; // ex. "note_1"
    public string noteUIName; // ex. "note_1"
    public string inventoryName; // ex. "note_1_col"

    private void Start()
    {
        if (GameStateManager.Instance != null && GameStateManager.Instance.IsCollected(itemID))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // save as collected
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.IsCollected(itemID);

        // Show UI 
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

        // Show note in inventory
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
