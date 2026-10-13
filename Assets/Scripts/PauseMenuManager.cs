using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pausePanel;

    private bool isPaused = false;

    private Button resumeButton;
    private Button saveButton;
    private TMP_Text saveLabel;
    private string saveLabelDefault = "Save";
    private Coroutine saveFeedback;

    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        PauseLock.Clear();
        CreateSaveButton();
    }

    void Update()
    {
        if (!GameInput.Pressed(GameAction.Pause))
            return;

        // Przy ekranie smierci pauza nie ma sensu.
        Player player = Player.Instance;
        if (player != null && player.deathPanel != null && player.deathPanel.activeSelf)
            return;

        TogglePause();
    }

    // Dokladamy przycisk "Save" miedzy Resume a Quit (klonujemy wyglad przycisku Resume).
    private void CreateSaveButton()
    {
        if (pausePanel == null)
            return;

        Transform resume = pausePanel.transform.Find("Resume");
        if (resume == null)
            return;

        resumeButton = resume.GetComponent<Button>();

        Button source = resumeButton;
        GameObject clone = Instantiate(resume.gameObject, pausePanel.transform);
        clone.name = "Save";

        // Klon ma te same trwale zdarzenia co Resume, wiec podmieniamy komponent Button na nowy.
        Button cloneOldButton = clone.GetComponent<Button>();
        if (cloneOldButton != null)
            DestroyImmediate(cloneOldButton);

        saveButton = clone.AddComponent<Button>();
        saveButton.targetGraphic = clone.GetComponent<Graphic>();
        saveButton.transition = source.transition;
        saveButton.colors = source.colors;
        saveButton.spriteState = source.spriteState;
        saveButton.onClick.AddListener(OnSaveButton);

        saveLabel = clone.GetComponentInChildren<TMP_Text>(true);
        if (saveLabel != null)
        {
            saveLabel.text = saveLabelDefault;
        }

        RectTransform resumeRect = resume as RectTransform;
        RectTransform saveRect = clone.transform as RectTransform;
        if (resumeRect != null && saveRect != null)
        {
            float step = 44.6f;
            saveRect.anchoredPosition = resumeRect.anchoredPosition;
            resumeRect.anchoredPosition = resumeRect.anchoredPosition + new Vector2(0f, step);
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (pausePanel != null)
            pausePanel.SetActive(isPaused);

        PauseLock.Set("pause", isPaused);

        if (isPaused)
        {
            if (resumeButton != null)
                GameInput.Select(resumeButton.gameObject);
        }
        else if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void OnResumeButton()
    {
        isPaused = false;
        if (pausePanel != null)
            pausePanel.SetActive(false);
        PauseLock.Set("pause", false);

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnSaveButton()
    {
        bool ok = SaveSystem.Save();

        if (saveLabel == null)
            return;

        if (saveFeedback != null)
            StopCoroutine(saveFeedback);
        saveFeedback = StartCoroutine(ShowSaveFeedback(ok ? "Saved!" : "Save failed"));
    }

    private IEnumerator ShowSaveFeedback(string message)
    {
        saveLabel.text = message;
        yield return new WaitForSecondsRealtime(1.5f);
        saveLabel.text = saveLabelDefault;
        saveFeedback = null;
    }

    public void QuitToMainMenu()
    {
        PauseLock.Clear();

        if (SessionManager.Instance != null)
            SessionManager.Instance.ExitToMenu();
    }
}
