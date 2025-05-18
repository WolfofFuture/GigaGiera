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

        Destroy(gameObject);
    }
}
