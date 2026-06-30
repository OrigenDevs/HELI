using UnityEngine;

public class OutlineSceneConfig : MonoBehaviour
{
    public Material materialOutline;
    public float lineWidth = 1f;

    void Awake()
    {
        if (materialOutline == null)
            materialOutline = Resources.Load<Material>("Shaders/POSTPROCESS/m_Outline");

        if (materialOutline != null)
            materialOutline.SetFloat("_LineWidth", lineWidth);
        else
            Debug.LogWarning("m_Outline no encontrado");
    }
}
