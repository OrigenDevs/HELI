using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MascaraSpriteButtom : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Tooltip("Alpha mínimo para que el toque/click se registre.")]
    public float umbralAlpha = 0.1f;

    [Tooltip("Se ejecuta solo si el toque cayó dentro del alpha del sprite.")]
    public UnityEvent OnClickValido;

    private Image imagen;
    private Texture2D textura;

    void Awake()
    {
        imagen = GetComponent<Image>();
        if (imagen != null && imagen.sprite != null)
            textura = imagen.sprite.texture;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (textura == null)
        {
            OnClickValido?.Invoke();
            return;
        }

        if (ContienePunto(eventData))
            OnClickValido?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData) { }

    public bool ContienePunto(PointerEventData eventData)
    {
        if (textura == null || imagen == null) return true;

        RectTransform rectTransform = transform as RectTransform;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint)) return false;

        Vector2 size = rectTransform.rect.size;
        Vector2 pivot = rectTransform.pivot;
        Vector2 offset = localPoint + size * pivot;

        float u = offset.x / size.x;
        float v = offset.y / size.y;

        if (u < 0f || u > 1f || v < 0f || v > 1f) return false;

        Sprite sprite = imagen.sprite;
        Rect rect = sprite.rect;

        int px = Mathf.FloorToInt((u * rect.width) + rect.x);
        int py = Mathf.FloorToInt((v * rect.height) + rect.y);

        return textura.GetPixel(px, py).a > umbralAlpha;
    }
}