using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Portal : MonoBehaviour
{
    public string targetScene = "Room2";
    public string myTag;
    public string destinationTag;
    public float cooldownTime = 3f;

    private bool isTeleporting = false;

    private void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.enterFromTag == myTag)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = transform.position;
                GameManager.Instance.StartTeleportCooldown(cooldownTime);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTeleporting || !GameManager.Instance.canTeleport)
            return;

        if (other.CompareTag("Player"))
        {
            StartCoroutine(SwitchRoom());
        }
    }

    private IEnumerator SwitchRoom()
    {
        isTeleporting = true;
        GameManager.Instance.enterFromTag = destinationTag;
        GameManager.Instance.StartTeleportCooldown(cooldownTime);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);
        if (asyncLoad == null)
            yield break;

        while (!asyncLoad.isDone)
            yield return null;

        Scene loadedScene = SceneManager.GetSceneByName(targetScene);
        if (!loadedScene.IsValid() || !loadedScene.isLoaded)
            yield break;

        SceneManager.SetActiveScene(loadedScene);

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);
            if (s.name != "Player" && s.name != targetScene)
            {
                yield return SceneManager.UnloadSceneAsync(s);
            }
        }

        isTeleporting = false;
    }
}
