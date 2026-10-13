using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Zarzadza cala sesja gry: nowa gra, wczytanie zapisu i wyjscie do menu.
// Obiekty gry przezywajace zmiane sceny (gracz, interfejs, kamera) sa niszczone przy wyjsciu,
// zeby ponowne wczytanie sceny Player nie tworzylo duplikatow ze zniszczonymi referencjami.
public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance;

    private bool sessionActive = false;
    private bool busy = false;

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

    public bool IsSessionActive() => sessionActive;

    public bool IsBusy() => busy;

    public bool CanContinue() => sessionActive || SaveSystem.Exists();

    public void StartNewGame()
    {
        if (busy) return;
        StartCoroutine(NewGameRoutine());
    }

    public void ContinueGame()
    {
        if (busy) return;

        if (!SaveSystem.Exists())
        {
            Debug.LogWarning("[SessionManager] Brak zapisu gry. Nie mozna kontynuowac.");
            return;
        }

        StartCoroutine(LoadGameRoutine());
    }

    // Po smierci: wracamy do ostatniego zapisu, a gdy go nie ma - do menu.
    public void RespawnAfterDeath()
    {
        if (busy) return;

        if (SaveSystem.Exists())
            StartCoroutine(LoadGameRoutine());
        else
            StartCoroutine(ExitRoutine());
    }

    public void ExitToMenu()
    {
        if (busy) return;
        StartCoroutine(ExitRoutine());
    }

    private void TearDownSession()
    {
        PauseLock.Clear();

        if (GameStateManager.Instance != null)
            GameStateManager.Instance.ResetState();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ClearPlayerRef();
            GameManager.Instance.experience = 0;
            GameManager.Instance.canTeleport = true;
            GameManager.Instance.enterFromTag = "";
            GameManager.Instance.spawnOverridePosition = null;
        }

        if (Player.Instance != null)
            Destroy(Player.Instance.gameObject);

        if (UI.Instance != null)
            Destroy(UI.Instance.gameObject);

        if (CameraMotor.Instance != null)
            Destroy(CameraMotor.Instance.gameObject);

        foreach (FloatingTextManager ftm in FindObjectsByType<FloatingTextManager>(FindObjectsInactive.Include))
            Destroy(ftm.gameObject);
    }

    private IEnumerator NewGameRoutine()
    {
        busy = true;
        TearDownSession();
        yield return null;

        SceneManager.LoadScene("Player", LoadSceneMode.Single);
        yield return null;

        AsyncOperation loadMain = SceneManager.LoadSceneAsync("Main", LoadSceneMode.Additive);
        while (!loadMain.isDone) yield return null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Main"));

        yield return new WaitForSeconds(0.2f);

        GameManager.Instance.RespawnPlayer();

        sessionActive = true;
        busy = false;
    }

    private IEnumerator LoadGameRoutine()
    {
        busy = true;

        SaveData data = SaveSystem.Read();
        if (data == null || string.IsNullOrEmpty(data.scene))
        {
            Debug.LogError("[SessionManager] Zapis jest uszkodzony.");
            busy = false;
            yield break;
        }

        TearDownSession();
        yield return null;

        SceneManager.LoadScene("Player", LoadSceneMode.Single);
        yield return null;

        // Gracz i kamera od razu na zapisanym miejscu, zeby po wczytaniu nie mignal w domyslnym punkcie sceny.
        Player earlyPlayer = FindAnyObjectByType<Player>();
        if (earlyPlayer != null)
            earlyPlayer.transform.position = new Vector3(data.posX, data.posY, 0f);

        CameraMotor earlyCamera = FindAnyObjectByType<CameraMotor>();
        if (earlyCamera != null)
            earlyCamera.transform.position = new Vector3(data.posX, data.posY, earlyCamera.transform.position.z);

        // Stan swiata musi byc przywrocony przed zaladowaniem pokoju (Start wrogow i przedmiotow go czyta).
        GameStateManager state = GameStateManager.Instance;
        state.ResetState();
        state.collectedItems = new HashSet<string>(data.collectedItems);
        state.inventoryItems = new HashSet<string>(data.inventoryItems);
        state.usedItems = new HashSet<string>(data.usedItems);
        state.destroyedObjects = new HashSet<string>(data.destroyedObjects);
        state.defeatedEnemies = new HashSet<string>(data.defeatedEnemies);
        state.collectedNotes = new HashSet<string>(data.collectedNotes);

        GameManager.Instance.enterFromTag = "";
        GameManager.Instance.spawnOverridePosition = new Vector3(data.posX, data.posY, 0f);

        AsyncOperation loadRoom = SceneManager.LoadSceneAsync(data.scene, LoadSceneMode.Additive);
        while (!loadRoom.isDone) yield return null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(data.scene));

        yield return new WaitForSeconds(0.2f);

        GameManager.Instance.RespawnPlayer();

        RestorePlayerState(data);

        sessionActive = true;
        busy = false;
    }

    private void RestorePlayerState(SaveData data)
    {
        Player player = Player.Instance;
        if (player == null)
            return;

        player.currentHealth = Mathf.Clamp(data.health, 1, player.maxHealth);
        if (player.healthBar != null)
            player.healthBar.SetHealth(player.currentHealth);

        InventoryManager inventory = InventoryManager.Instance;
        ItemIcons icons = ItemIcons.Load();
        if (inventory != null)
        {
            foreach (string id in data.inventorySlots)
            {
                Sprite icon = icons != null ? icons.Get(id) : null;
                if (icon == null)
                {
                    Debug.LogWarning("[SessionManager] Brak ikony dla przedmiotu " + id);
                    continue;
                }
                inventory.AddItem(icon, id);
            }
        }

        if (player.InventoryUI != null)
        {
            foreach (string noteName in data.collectedNotes)
            {
                Transform note = FindDeep(player.InventoryUI.transform, noteName);
                if (note != null)
                    note.gameObject.SetActive(true);
            }
        }
    }

    private static Transform FindDeep(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform result = FindDeep(child, name);
            if (result != null)
                return result;
        }
        return null;
    }

    private IEnumerator ExitRoutine()
    {
        busy = true;
        TearDownSession();
        yield return null;

        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        yield return null;

        sessionActive = false;
        busy = false;
    }
}
