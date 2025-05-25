using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Transform playerSpawnPoint;
    public GameObject playerPrefab;

    private GameObject currentPlayer;

    public int experience = 0;

    public Player player => currentPlayer != null ? currentPlayer.GetComponent<Player>() : null;

    public string enterFromTag = "";
    public bool canTeleport = true;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        RespawnPlayer();
    }

    public void RespawnPlayer()
    {
        if (currentPlayer != null)
            Destroy(currentPlayer);

        currentPlayer = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);

        //GameStateManager.Instance.ResetCollected();
        //InventoryManager.Instance.ResetInventory();

        // *** ZMIANA: Zast¹piono przestarza³e FindObjectsOfType now¹ wersj¹ ***
        CollectableItem[] allCollectables = FindObjectsByType<CollectableItem>(FindObjectsSortMode.None);
        foreach (var collectable in allCollectables)
        {
            collectable.ResetCollectable();
        }

        Player playerComp = currentPlayer.GetComponent<Player>();
        if (playerComp != null)
            playerComp.ResetPlayerState();

        canTeleport = true;
        enterFromTag = "";
    }

    public void StartTeleportCooldown(float cooldownTime)
    {
        if (canTeleport)
            StartCoroutine(TeleportCooldownCoroutine(cooldownTime));
    }

    private IEnumerator TeleportCooldownCoroutine(float cooldownTime)
    {
        canTeleport = false;
        yield return new WaitForSeconds(cooldownTime);
        canTeleport = true;
    }

    public void ShowText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        Debug.Log($"ShowText: {msg} at {position}");
    }

    // *** NOWA METODA ***
    public void ResetGame()
    {
        // Wyzeruj doœwiadczenie
        experience = 0;

        // Reset teleportacji
        canTeleport = true;
        enterFromTag = "";

        // Reset inventory, collectables, zdrowie itp.
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.ResetInventory();

        //if (CollectibleManager.Instance != null)
        //    CollectibleManager.Instance.ResetCollected();

        // Usuñ aktualnego gracza jeœli jest
        if (currentPlayer != null)
            Destroy(currentPlayer);

        // Prze³aduj scenê MainMenu na œwie¿o
        SceneManager.LoadScene("MainMenu");
    }
}
