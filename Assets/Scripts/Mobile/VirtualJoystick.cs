using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform background; 
    [SerializeField] private RectTransform handle; 
    [SerializeField, Range(0f, 0.5f)] private float deadZone = 0.15f;

    private float radius;

    private void Awake()
    {
        if (background == null) background = (RectTransform)transform;
        radius = background.rect.width * 0.5f;
    }

    public void OnPointerDown(PointerEventData e)
    {
        GameInput.SetLastUsed(InputKind.Touch);
        OnDrag(e);
    }

    public void OnDrag(PointerEventData e)
    {
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
        TouchInput.Move = Vector2.zero;
        if (handle != null) handle.anchoredPosition = Vector2.zero;
    }
}