using UnityEngine;

[ExecuteInEditMode]
public class CardMaskSetup : MonoBehaviour
{
    private static readonly int RectMinID = Shader.PropertyToID("_RectMin");
    private static readonly int RectSizeID = Shader.PropertyToID("_RectSize");

    void Start()
    {
        ApplySpriteRect();
    }

    void ApplySpriteRect()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null || spriteRenderer.sprite == null) return;

        Sprite sprite = spriteRenderer.sprite;
        Texture2D atlas = sprite.texture;

        // Obtenemos las coordenadas del rect en el atlas normalizadas de 0 a 1
        Rect rect = sprite.rect;
        Vector2 rectMin = new Vector2(rect.x / atlas.width, rect.y / atlas.height);
        Vector2 rectSize = new Vector2(rect.width / atlas.width, rect.height / atlas.height);

        // Usamos MaterialPropertyBlock para no duplicar el material en memoria
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(mpb);

        mpb.SetVector(RectMinID, rectMin);
        mpb.SetVector(RectSizeID, rectSize);

        spriteRenderer.SetPropertyBlock(mpb);
    }
}