using UnityEngine;
using UnityEngine.UI;
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
        BuildExtraButtons();

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


    // Dodatkowe przyciski ekranowe: ekwipunek, pauza i dwie mikstury (klonujemy wyglad istniejacego przycisku).
    private void BuildExtraButtons()
    {
        if (root == null)
            return;

        Transform template = root.transform.Find("UseBurtrton");
        if (template == null)
            template = root.transform.Find("AttackButton");
        if (template == null)
            return;

        ItemIcons icons = ItemIcons.Load();

        CreateExtraButton(template, "InventoryButton", GameAction.Inventory, new Vector2(1f, 1f), new Vector2(-240f, -100f), 110f,
            new Color(0.35f, 0.5f, 0.75f, 1f), null, 3);
        CreateExtraButton(template, "PauseButton", GameAction.Pause, new Vector2(1f, 1f), new Vector2(-100f, -100f), 110f,
            new Color(0.5f, 0.5f, 0.58f, 1f), null, 2);
        CreateExtraButton(template, "HealPotionButton", GameAction.HealPotion, new Vector2(1f, 0f), new Vector2(-470f, 160f), 120f,
            new Color(0.4f, 0.7f, 0.45f, 1f), icons != null ? icons.healthPotion : null, 0);
        CreateExtraButton(template, "SpeedPotionButton", GameAction.SpeedPotion, new Vector2(1f, 0f), new Vector2(-470f, 300f), 120f,
            new Color(0.4f, 0.6f, 0.85f, 1f), icons != null ? icons.speedPotion : null, 0);
    }

    private void CreateExtraButton(Transform template, string buttonName, GameAction action, Vector2 anchor, Vector2 position,
        float size, Color color, Sprite iconSprite, int bars)
    {
        if (root.transform.Find(buttonName) != null)
            return;

        GameObject go = Instantiate(template.gameObject, root.transform);
        go.name = buttonName;
        foreach (Transform child in go.transform)
            Destroy(child.gameObject);

        RectTransform rt = (RectTransform)go.transform;
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(size, size);
        rt.localScale = Vector3.one;

        Image background = go.GetComponent<Image>();
        if (background != null)
            background.color = color;

        TouchButton button = go.GetComponent<TouchButton>();
        if (button != null)
            button.action = action;

        if (iconSprite != null)
        {
            Image icon = NewIcon(rt, "Icon", new Vector2(size * 0.62f, size * 0.62f), Vector2.zero);
            icon.sprite = iconSprite;
            icon.preserveAspect = true;
        }

        // Ikona z poziomych (menu) albo pionowych (pauza) kresek.
        for (int i = 0; i < bars; i++)
        {
            Image bar;
            if (bars == 3)
            {
                float y = (i - 1) * size * 0.2f;
                bar = NewIcon(rt, "Bar" + i, new Vector2(size * 0.5f, size * 0.09f), new Vector2(0f, y));
            }
            else
            {
                float x = (i - 0.5f) * size * 0.24f;
                bar = NewIcon(rt, "Bar" + i, new Vector2(size * 0.1f, size * 0.42f), new Vector2(x, 0f));
            }
            bar.color = Color.white;
        }
    }

    private static Image NewIcon(RectTransform parent, string iconName, Vector2 size, Vector2 position)
    {
        GameObject go = new GameObject(iconName, typeof(RectTransform), typeof(Image));
        RectTransform rt = (RectTransform)go.transform;
        rt.SetParent(parent, false);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
        rt.anchoredPosition = position;

        Image img = go.GetComponent<Image>();
        img.raycastTarget = false;
        return img;
    }

    private void OnSceneLoaded(Scene s, LoadSceneMode m) => Refresh();
    private void OnLastUsedChanged(InputKind k) => Refresh();

    private void Refresh()
    {
        bool inMenu = SceneManager.GetActiveScene().name == menuSceneName;
        root.SetActive(GameInput.LastUsed == InputKind.Touch && !inMenu);
    }
}