using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public Button continueButton;

    private void Start()
    {
        if (continueButton != null)
            continueButton.interactable = SessionManager.Instance != null && SessionManager.Instance.IsSessionActive();
    }

    public void PlayGame()
    {
        SessionManager.Instance.StartNewGame();
    }

    public void ContinueGame()
    {
        SessionManager.Instance.ContinueGame();
    }



    private IEnumerator LoadGame()
    {
        // Najpierw ³adujemy scenê Player (additive)
        AsyncOperation loadPlayer = SceneManager.LoadSceneAsync("Player", LoadSceneMode.Additive);
        while (!loadPlayer.isDone) yield return null;

        // Nastêpnie ³adujemy scenê Main (additive)
        AsyncOperation loadRoom = SceneManager.LoadSceneAsync("Main", LoadSceneMode.Additive);
        while (!loadRoom.isDone) yield return null;

        // Usuwamy scenê menu (MainMenu)
        SceneManager.UnloadSceneAsync("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
