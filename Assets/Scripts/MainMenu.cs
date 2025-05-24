using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        Debug.Log("[MainMenu] PLAY GAME klikniêty");

        if (CollectibleManager.Instance != null)
        {
            Debug.Log("[MainMenu] Resetujê CollectibleManager");
            CollectibleManager.Instance.ResetCollected();
        }

        if (InventoryManager.Instance != null)
        {
            Debug.Log("[MainMenu] Resetujê InventoryManager");
            InventoryManager.Instance.ResetInventory();
        }

        if (Player.Instance != null)
        {
            Debug.Log("[MainMenu] Resetujê stan gracza");
            Player.Instance.ResetPlayerState();
        }
        else
        {
            Debug.LogWarning("[MainMenu] Player.Instance jest null przy próbie resetu stanu gracza");
        }

        StartCoroutine(LoadGame());
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
