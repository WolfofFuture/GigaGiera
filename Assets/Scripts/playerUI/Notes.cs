using UnityEngine;
using UnityEngine.UI;

public class NotePickup : MonoBehaviour
{
    public string noteName; // eg. "note_1"

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        GameObject noteUI = GameObject.Find("NoteUI");
        if (noteUI != null)
        {
            Transform note = noteUI.transform.Find(noteName);
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
        //show note in inventory
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
                        Transform collectedNote = FindDeepChild(page, noteName + "_col");
                        if (collectedNote != null)
                        {
                            collectedNote.gameObject.SetActive(true);
                            break;
                        }
                    }
                }
            }
        }

        Destroy(gameObject);
    }
    //find note helper
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



