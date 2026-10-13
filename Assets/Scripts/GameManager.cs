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

    public Vector3? spawnOverridePosition = null;


    public Player player => currentPlayer != null ? currentPlayer.GetComponent<Player>() : null;

    public string enterFromTag = "";
    public bool canTeleport = true;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Po załadowaniu każdej sceny respawnuj gracza, jeśli jest aktywny
        if (scene.name != "MainMenu" && currentPlayer == null)
        {
            RespawnPlayer();
        }
    }


    public void RespawnPlayer()
    {
        currentPlayer = GameObject.FindGameObjectWithTag("Player");

        if (currentPlayer == null)
        {
            Debug.LogError("[GameManager] Nie znaleziono gracza!");
            return;
        }

        Player playerComp = currentPlayer.GetComponent<Player>();
        if (playerComp != null)
            playerComp.ResetPlayerState();

        // 📷 ustaw kamerze nowy cel
        CameraMotor cam = FindAnyObjectByType<CameraMotor>();
        if (cam != null)
            cam.SetTarget(currentPlayer.transform);

        canTeleport = true;
        enterFromTag = "";

        if (spawnOverridePosition != null)
        {
            currentPlayer.transform.position = spawnOverridePosition.Value;
            Debug.Log($"[GameManager] Przeniesiono gracza do pozycji: {spawnOverridePosition.Value}");
            spawnOverridePosition = null;
        }

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
        // Wyzeruj doświadczenie
        experience = 0;

        // Reset teleportacji
        canTeleport = true;
        enterFromTag = "";

        // Reset inventory, collectables, zdrowie itp.
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.ResetInventory();

        //if (CollectibleManager.Instance != null)
        //    CollectibleManager.Instance.ResetCollected();

        // Usuń aktualnego gracza jeśli jest
        if (currentPlayer != null)
            Destroy(currentPlayer);

        // Przeładuj scenę MainMenu na świeżo
        SceneManager.LoadScene("MainMenu");
    }
}
