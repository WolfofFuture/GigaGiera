using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportOnDoorUnlock : MonoBehaviour
{
    [Header("Nazwa sceny docelowej")]
    public string targetScene;

    [Header("Pozycja gracza po teleportacji")]
    public Vector3 spawnPosition = new Vector3(0f, 0f, 0f);

    [Header("Czy teleportacja nast¹pi automatycznie po odblokowaniu drzwi?")]
    public float delayAfterUnlock = 1f;

    private InteractableObject door;
    private bool triggered = false;

    private void Start()
    {
        door = GetComponent<InteractableObject>();
        if (door == null)
        {
            Debug.LogError("[Teleport] Brak komponentu InteractableObject!");
            return;
        }
    }

    private void Update()
    {
        if (!triggered && door != null && IsDoorUnlocked())
        {
            triggered = true;
            Debug.Log("[Teleport] Drzwi zosta³y odblokowane – teleportacja...");
            Invoke(nameof(StartTeleport), delayAfterUnlock);
        }
    }

    private bool IsDoorUnlocked()
    {
        // zak³adamy, ¿e collider zosta³ wy³¹czony przez Unlock()
        return !door.enabled || (door.GetComponent<Collider2D>() != null && !door.GetComponent<Collider2D>().enabled);
    }

    private void StartTeleport()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.enterFromTag = "DOOR_AUTO";
            GameManager.Instance.spawnOverridePosition = spawnPosition;
        }

        SceneManager.LoadScene("Player", LoadSceneMode.Single);
        StartCoroutine(LoadTargetSceneAsync());
    }

    private IEnumerator LoadTargetSceneAsync()
    {
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);
        while (!loadOp.isDone)
            yield return null;

        // Po za³adowaniu sceny respawnuj gracza
        GameManager.Instance.RespawnPlayer();
    }

}
