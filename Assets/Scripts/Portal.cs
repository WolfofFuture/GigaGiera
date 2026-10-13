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
            Player player = FindAnyObjectByType<Player>();
            if (player != null)
            {
                player.transform.position = transform.position;
                GameManager.Instance.StartTeleportCooldown(cooldownTime);
            }
        }
        else if (GameManager.Instance != null)
        {
            GameManager.Instance.StartTeleportCooldown(1f);
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

    private void PlaceAtArrival(Scene scene)
    {
        Player player = FindAnyObjectByType<Player>();
        if (player == null)
            return;

        foreach (Portal portal in FindObjectsByType<Portal>(FindObjectsInactive.Include))
        {
            if (portal.gameObject.scene == scene && portal.myTag == destinationTag)
            {
                player.transform.position = portal.transform.position;
                break;
            }
        }

        GameManager.Instance.enterFromTag = "";
    }

    private IEnumerator SwitchRoom()
    {
        isTeleporting = true;
        GameManager.Instance.enterFromTag = destinationTag;
        GameManager.Instance.StartTeleportCooldown(cooldownTime);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);
        if (asyncLoad == null)
        {
            isTeleporting = false;
            yield break;
        }

        while (!asyncLoad.isDone)
            yield return null;

        Scene loadedScene = SceneManager.GetSceneByName(targetScene);
        if (!loadedScene.IsValid() || !loadedScene.isLoaded)
        {
            isTeleporting = false;
            yield break;
        }

        SceneManager.SetActiveScene(loadedScene);
        PlaceAtArrival(loadedScene);

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
