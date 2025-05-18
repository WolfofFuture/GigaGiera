using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoSceneLoader : MonoBehaviour
{
    private void Awake()
    {
        bool playerLoaded = false;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == "Player")
            {
                playerLoaded = true;
                break;
            }
        }

        if (!playerLoaded)
        {
            SceneManager.LoadScene("Player", LoadSceneMode.Additive);
        }
    }
}
