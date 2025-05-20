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

        StartCoroutine(LoadGame());
    }


    private IEnumerator LoadGame()
    {
        // Load the Player scene first (additive)
        AsyncOperation loadPlayer = SceneManager.LoadSceneAsync("Player", LoadSceneMode.Additive);
        while (!loadPlayer.isDone) yield return null;

        // Load the Main room scene (also additive)
        AsyncOperation loadRoom = SceneManager.LoadSceneAsync("Main", LoadSceneMode.Additive);
        while (!loadRoom.isDone) yield return null;

        // Unload the menu scene to clean up
        SceneManager.UnloadSceneAsync("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
