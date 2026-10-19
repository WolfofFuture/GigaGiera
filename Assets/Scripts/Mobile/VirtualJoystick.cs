using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Pływający joystick: pojawia się tam, gdzie dotkniesz lewej połowy ekranu, i znika (wraca na miejsce) po puszczeniu.
public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
    [SerializeField, Range(0f, 0.5f)] private float deadZone = 0.15f;
    [SerializeField, Range(0.1f, 1f)] private float restAlpha = 0.45f;
    [SerializeField, Range(0.5f, 1f)] private float activeAlpha = 0.9f;
    [SerializeField, Range(0f, 1f)] private float zoneWidth = 0.5f;   // jaka czesc szerokosci ekranu reaguje na dotyk

    private float radius;
    private RectTransform parentRect;
    private Vector2 restPosition;
    private CanvasGroup group;
    private Image zoneImage;
    private bool dragging;

    private void Awake()
    {
        if (background == null) background = (RectTransform)transform;
        radius = background.rect.width * 0.5f;
        parentRect = background.parent as RectTransform;
        restPosition = background.anchoredPosition;

        group = background.GetComponent<CanvasGroup>();
        if (group == null) group = background.gameObject.AddComponent<CanvasGroup>();
        group.blocksRaycasts = false;
        group.alpha = restAlpha;

        // Sam joystick nie lapie dotyku - robi to przezroczysta strefa na calej lewej polowie ekranu.
        foreach (Graphic g in background.GetComponentsInChildren<Graphic>(true))
            g.raycastTarget = false;

        CreateZone();
    }

    private void CreateZone()
    {
        if (parentRect == null) return;

        GameObject go = new GameObject("JoystickZone", typeof(RectTransform), typeof(Image), typeof(JoystickZone));
        RectTransform rt = (RectTransform)go.transform;
        rt.SetParent(parentRect, false);
        rt.SetAsFirstSibling();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = new Vector2(zoneWidth, 1f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        zoneImage = go.GetComponent<Image>();
        zoneImage.color = new Color(1f, 1f, 1f, 0f);
        zoneImage.raycastTarget = true;

        go.GetComponent<JoystickZone>().owner = this;
    }

    private void Update()
    {
        // Gdy otwarty jest ekwipunek, pauza, notatka albo ekran smierci, strefa nie moze zaslaniac przyciskow menu.
        if (zoneImage == null) return;

        Player p = Player.Instance;
        bool dead = p != null && p.deathPanel != null && p.deathPanel.activeSelf;
        bool blocked = PauseLock.AnyLocked || dead;

        if (zoneImage.raycastTarget == blocked)
        {
            zoneImage.raycastTarget = !blocked;
            if (blocked) Release();
        }
    }

    public void OnPointerDown(PointerEventData e)
    {
        GameInput.SetLastUsed(InputKind.Touch);
        dragging = true;

        // Joystick wskakuje pod palec.
        if (parentRect != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect, e.position, e.pressEventCamera, out Vector2 local);

            Rect r = parentRect.rect;
            float x = Mathf.Clamp(local.x - r.xMin, radius * 0.6f, r.width * zoneWidth);
            float y = Mathf.Clamp(local.y - r.yMin, radius * 0.6f, r.height - radius * 0.6f);
            background.anchoredPosition = new Vector2(x, y);
        }

        group.alpha = activeAlpha;
        OnDrag(e);
    }

    public void OnDrag(PointerEventData e)
    {
        if (!dragging) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background, e.position, e.pressEventCamera, out Vector2 local);

        Vector2 v = Vector2.ClampMagnitude(local / radius, 1f);
        if (handle != null) handle.anchoredPosition = v * radius;

        float mag = v.magnitude;
        TouchInput.Move = mag < deadZone
            ? Vector2.zero
            : v.normalized * ((mag - deadZone) / (1f - deadZone));
    }

    public void OnPointerUp(PointerEventData e) => Release();
    private void OnDisable() => Release();

    private void Release()
    {
        dragging = false;
        TouchInput.Move = Vector2.zero;
        if (handle != null) handle.anchoredPosition = Vector2.zero;
        if (background != null) background.anchoredPosition = restPosition;
        if (group != null) group.alpha = restAlpha;
    }

    // Przezroczysta strefa dotyku przekazujaca zdarzenia do joysticka.
    private class JoystickZone : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public VirtualJoystick owner;

        public void OnPointerDown(PointerEventData e) { if (owner != null) owner.OnPointerDown(e); }
        public void OnDrag(PointerEventData e) { if (owner != null) owner.OnDrag(e); }
        public void OnPointerUp(PointerEventData e) { if (owner != null) owner.OnPointerUp(e); }
    }
}
