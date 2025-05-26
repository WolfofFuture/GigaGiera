using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance;

    private bool sessionActive = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartNewGame()
    {
        sessionActive = true;
        GameStateManager.Instance?.ResetState();
        InventoryManager.Instance?.ResetInventory();
        Player.Instance?.ResetPlayerState();

        GameManager.Instance.experience = 0;
        GameManager.Instance.canTeleport = true;
        GameManager.Instance.enterFromTag = "";

        GameObject.Destroy(GameManager.Instance.player?.gameObject);

        // Rozpocznij coroutine
        StartCoroutine(LoadNewGame());
    }


    public void ContinueGame()
    {
        if (!sessionActive)
        {
            Debug.LogWarning("[SessionManager] Brak aktywnej sesji. Nie można kontynuować.");
            return;
        }

        Debug.Log("[SessionManager] Kontynuuję grę");
        // Nic nie resetujemy! Wracamy do aktualnej sceny
        UnloadMenuAndResumeGame();
    }

    public void QuitToMainMenu()
    {
        Debug.Log("[SessionManager] Powrót do menu z aktywnej sesji");
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
    }

    public void UnloadMenuAndResumeGame()
    {
        if (SceneManager.GetSceneByName("MainMenu").isLoaded)
        {
            SceneManager.UnloadSceneAsync("MainMenu");
        }
    }
    private IEnumerator LoadNewGame()
    {
        SceneManager.LoadScene("Player", LoadSceneMode.Single);
        yield return null;

        AsyncOperation loadMain = SceneManager.LoadSceneAsync("Main", LoadSceneMode.Additive); //JunRoom1
        while (!loadMain.isDone) yield return null;

        yield return new WaitForSeconds(0.2f); // krótki delay na inicjalizację spawn pointa

        GameManager.Instance.RespawnPlayer();
    }

    public bool IsSessionActive() => sessionActive;
}
