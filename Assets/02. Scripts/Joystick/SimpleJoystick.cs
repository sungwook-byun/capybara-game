using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleJoystick : MonoBehaviour,
    IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("UI")]
    public RectTransform joystickBackground;
    public RectTransform joystickHandle;

    private Vector2 inputVector;
    private float radius;
    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();

        joystickBackground.gameObject.SetActive(false);
        joystickHandle.gameObject.SetActive(false);

        radius = joystickBackground.sizeDelta.x * 0.5f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        joystickBackground.gameObject.SetActive(true);
        joystickHandle.gameObject.SetActive(true);

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPos
        );

        joystickBackground.anchoredPosition = localPos;
        joystickHandle.anchoredPosition = Vector2.zero;
        inputVector = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground,
            eventData.position,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPos
        );

        Vector2 clamped = Vector2.ClampMagnitude(localPos, radius);
        inputVector = clamped / radius;

        joystickHandle.anchoredPosition = clamped;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        joystickBackground.gameObject.SetActive(false);
        joystickHandle.gameObject.SetActive(false);

        inputVector = Vector2.zero;
    }

    public Vector2 Direction => inputVector;
}
