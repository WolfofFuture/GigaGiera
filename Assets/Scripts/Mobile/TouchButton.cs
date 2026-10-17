using UnityEngine;
using UnityEngine.EventSystems;

public class TouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameAction action;
    [SerializeField] private float pressedScale = 0.9f;

    public void OnPointerDown(PointerEventData e)
    {
        TouchInput.Press(action);
        transform.localScale = Vector3.one * pressedScale;
    }

    public void OnPointerUp(PointerEventData e) => transform.localScale = Vector3.one;
    private void OnDisable() => transform.localScale = Vector3.one;
}