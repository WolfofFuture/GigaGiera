using UnityEngine;
using UnityEngine.SceneManagement;

public class TouchControlsUI : MonoBehaviour
{
    [SerializeField] private GameObject root;              
    [SerializeField] private bool forceShowInEditor = true; 
    [SerializeField] private string menuSceneName = "MainMenu";

    private static TouchControlsUI instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
        GameInput.LastUsedChanged += OnLastUsedChanged;
    }

    private void Start()
    {
        bool wantTouch = Application.isMobilePlatform;
#if UNITY_EDITOR
        wantTouch |= forceShowInEditor;
#endif
        if (wantTouch)
            GameInput.SetLastUsed(InputKind.Touch);

        Refresh();
    }

    private void OnDestroy()
    {
        if (instance != this) return;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameInput.LastUsedChanged -= OnLastUsedChanged;
    }

    private void OnSceneLoaded(Scene s, LoadSceneMode m) => Refresh();
    private void OnLastUsedChanged(InputKind k) => Refresh();

    private void Refresh()
    {
        bool inMenu = SceneManager.GetActiveScene().name == menuSceneName;
        root.SetActive(GameInput.LastUsed == InputKind.Touch && !inMenu);
    }
}