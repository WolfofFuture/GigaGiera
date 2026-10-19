using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public enum InputKind
{
    KeyboardMouse,
    Gamepad,
    Touch
}

public enum GameAction
{
    Attack,
    Interact,
    Inventory,
    Notes,
    HealPotion,
    SpeedPotion,
    Pause
}

// Jedno miejsce, z ktorego gra czyta sterowanie (klawiatura, pad Xbox/PlayStation, pozniej dotyk).
public static class GameInput
{
    private static InputAction move;
    private static InputAction attack;
    private static InputAction interact;
    private static InputAction inventory;
    private static InputAction notes;
    private static InputAction healPotion;
    private static InputAction speedPotion;
    private static InputAction pause;

    private static bool initialized;

    public static InputKind LastUsed { get; private set; } = InputKind.KeyboardMouse;

    public static event System.Action<InputKind> LastUsedChanged;

    public static Vector2 Move
    {
        get
        {
            Vector2 v = move != null ? move.ReadValue<Vector2>() : Vector2.zero;
            return v.sqrMagnitude > 0.04f ? v : TouchInput.Move;
        }
    }

    public static bool Pressed(GameAction action)
    {
        InputAction a = Get(action);
        return (a != null && a.WasPressedThisFrame()) || TouchInput.Pressed(action);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (initialized)
            return;

        initialized = true;

        move = new InputAction("Move", InputActionType.Value, expectedControlType: "Vector2");
        move.AddCompositeBinding("2DVector(mode=1)")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        move.AddCompositeBinding("2DVector(mode=1)")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");
        move.AddBinding("<Gamepad>/leftStick");
        move.AddCompositeBinding("2DVector(mode=1)")
            .With("Up", "<Gamepad>/dpad/up")
            .With("Down", "<Gamepad>/dpad/down")
            .With("Left", "<Gamepad>/dpad/left")
            .With("Right", "<Gamepad>/dpad/right");
        move.Enable();

        attack = Button("Attack", "<Keyboard>/space", "<Gamepad>/buttonWest", "<Gamepad>/rightTrigger");
        interact = Button("Interact", "<Keyboard>/e", "<Gamepad>/buttonSouth");
        inventory = Button("Inventory", "<Keyboard>/i", "<Gamepad>/buttonNorth");
        notes = Button("Notes", "<Keyboard>/n", "<Gamepad>/select");
        healPotion = Button("HealPotion", "<Keyboard>/o", "<Gamepad>/leftShoulder");
        speedPotion = Button("SpeedPotion", "<Keyboard>/p", "<Gamepad>/rightShoulder");
        pause = Button("Pause", "<Keyboard>/m", "<Keyboard>/escape", "<Gamepad>/start");

        InputSystem.onActionChange += OnActionChange;
        SceneManager.sceneLoaded += (scene, mode) => EnsureUIModule();
    }

    private static InputAction Button(string name, params string[] bindings)
    {
        InputAction action = new InputAction(name, InputActionType.Button);
        foreach (string binding in bindings)
            action.AddBinding(binding);
        action.Enable();
        return action;
    }

    private static InputAction Get(GameAction action)
    {
        switch (action)
        {
            case GameAction.Attack: return attack;
            case GameAction.Interact: return interact;
            case GameAction.Inventory: return inventory;
            case GameAction.Notes: return notes;
            case GameAction.HealPotion: return healPotion;
            case GameAction.SpeedPotion: return speedPotion;
            case GameAction.Pause: return pause;
        }
        return null;
    }

    // Wlaczamy akcje ruchu dopiero tu, bo Enable w Init przed scena potrafi byc pominiete po przeladowaniu.
    public static void EnsureEnabled()
    {
        if (move != null && !move.enabled)
            move.Enable();
    }

    private static void OnActionChange(object obj, InputActionChange change)
    {
        if (change != InputActionChange.ActionPerformed)
            return;

        InputAction action = obj as InputAction;
        if (action == null || action.activeControl == null)
            return;

        if (action.actionMap != null)
            return;

        // Drazek pada zawsze ma lekki dryf (np. po puszczeniu klawisza akcja ruchu przelicza sie z drazka),
        // wiec male wychylenia nie moga sie liczyc jako uzycie pada.
        float magnitude = action.activeControl.EvaluateMagnitude();
        if (magnitude >= 0f && magnitude < 0.4f)
            return;

        SetLastUsed(Classify(action.activeControl.device));
    }

    private static InputKind Classify(InputDevice device)
    {
        if (device is Gamepad || device is Joystick)
            return InputKind.Gamepad;
        if (device is Touchscreen)
            return InputKind.Touch;
        return InputKind.KeyboardMouse;
    }

    public static void SetLastUsed(InputKind kind)
    {
        if (kind == LastUsed)
            return;

        LastUsed = kind;
        if (LastUsedChanged != null)
            LastUsedChanged(kind);
    }

    // Tekst przycisku do podpowiedzi, zalezny od ostatnio uzywanego urzadzenia.
    public static string Prompt(GameAction action)
    {
        if (LastUsed == InputKind.Gamepad)
        {
            bool playStation = Gamepad.current is DualShockGamepad;
            switch (action)
            {
                case GameAction.Attack: return playStation ? "Square" : "X";
                case GameAction.Interact: return playStation ? "Cross" : "A";
                case GameAction.Inventory: return playStation ? "Triangle" : "Y";
                case GameAction.Notes: return playStation ? "Share" : "View";
                case GameAction.HealPotion: return "L1";
                case GameAction.SpeedPotion: return "R1";
                case GameAction.Pause: return playStation ? "Options" : "Menu";
            }
        }

        if (LastUsed == InputKind.Touch)
        {
            switch (action)
            {
                case GameAction.Interact: return "the button";
                default: return "the button";
            }
        }

        switch (action)
        {
            case GameAction.Attack: return "Space";
            case GameAction.Interact: return "E";
            case GameAction.Inventory: return "I";
            case GameAction.Notes: return "N";
            case GameAction.HealPotion: return "O";
            case GameAction.SpeedPotion: return "P";
            case GameAction.Pause: return "M";
        }
        return "";
    }


    // Wbudowane akcje UI trzymamy sami. Nie uzywamy AssignDefaultActions(), bo gdy w projekcie sa przypisane
    // "project-wide actions", podpina ono menu do nich i w buildzie WebGL menu przestaje reagowac.
    private static DefaultInputActions uiActions;

    private static void AssignUiActions(InputSystemUIInputModule module)
    {
        if (uiActions == null)
            uiActions = new DefaultInputActions();

        uiActions.UI.Enable();

        module.actionsAsset = uiActions.asset;
        module.point = InputActionReference.Create(uiActions.UI.Point);
        module.leftClick = InputActionReference.Create(uiActions.UI.Click);
        module.middleClick = InputActionReference.Create(uiActions.UI.MiddleClick);
        module.rightClick = InputActionReference.Create(uiActions.UI.RightClick);
        module.scrollWheel = InputActionReference.Create(uiActions.UI.ScrollWheel);
        module.move = InputActionReference.Create(uiActions.UI.Navigate);
        module.submit = InputActionReference.Create(uiActions.UI.Submit);
        module.cancel = InputActionReference.Create(uiActions.UI.Cancel);
        module.trackedDevicePosition = InputActionReference.Create(uiActions.UI.TrackedDevicePosition);
        module.trackedDeviceOrientation = InputActionReference.Create(uiActions.UI.TrackedDeviceOrientation);
    }

    // Menu ma sie dac obslugiwac padem, wiec zamieniamy stary modul wejscia UI na modul nowego Input System.
    public static void EnsureUIModule()
    {
        EnsureEnabled();

        foreach (EventSystem es in Object.FindObjectsByType<EventSystem>())
        {
            if (es.GetComponent<InputSystemUIInputModule>() != null)
                continue;

            StandaloneInputModule old = es.GetComponent<StandaloneInputModule>();
            if (old != null)
                Object.Destroy(old);

            InputSystemUIInputModule module = es.gameObject.AddComponent<InputSystemUIInputModule>();
            AssignUiActions(module);
        }
    }

    // Pomocnik dla menu: zaznacza przycisk, zeby pad mogl od razu nawigowac.
    public static void Select(GameObject target)
    {
        if (EventSystem.current == null)
            return;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(target);
    }
}
