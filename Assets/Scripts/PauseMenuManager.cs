using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pausePanel;

    private bool isPaused = false;
    private bool keyHandled = false;

    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        PauseLock.Clear();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && !keyHandled)
        {
            keyHandled = true;
            TogglePause();
        }

        if (Input.GetKeyUp(KeyCode.M))
        {
            keyHandled = false;
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (pausePanel != null)
            pausePanel.SetActive(isPaused);

        PauseLock.Set("pause", isPaused);
    }

    public void OnResumeButton()
    {
        isPaused = false;
        if (pausePanel != null)
            pausePanel.SetActive(false);
        PauseLock.Set("pause", false);
    }

    public void QuitToMainMenu()
    {
        PauseLock.Clear();
        SessionManager.Instance.QuitToMainMenu();
    }


}
