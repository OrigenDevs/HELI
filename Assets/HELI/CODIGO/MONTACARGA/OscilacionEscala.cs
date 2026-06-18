using UnityEngine;

/// <summary>
/// Oscilación automática de escala con efecto cartoon ajustable
/// </summary>
public class OscilacionEscala : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Velocidad de oscilación")]
    public float velocidad = 1f;
    
    [Tooltip("Exageración del movimiento (1 = normal, >1 = más cartoon)")]
    [Range(1f, 5f)]
    public float exageracion = 1f;

    [Header("Rango por Eje")]
    [Tooltip("Rango de oscilación en X (±escala)")]
    public float rangoX = 0.5f;
    
    [Tooltip("Rango de oscilación en Y (±escala)")]
    public float rangoY = 0.5f;
    
    [Tooltip("Rango de oscilación en Z (±escala)")]
    public float rangoZ = 0.5f;

    [Header("Ejes de Oscilación")]
    public bool oscilarX = true;
    public bool oscilarY = true;
    public bool oscilarZ = true;

    private Vector3 escalaInicial;
    private float tiempo = 0f;

    void Start()
    {
        escalaInicial = transform.localScale;
    }

    void Update()
    {
        tiempo += Time.deltaTime * velocidad;

        // Aplicar exageración a la función seno
        float seno = Mathf.Sin(tiempo);
        float valorOscilacion = Mathf.Pow(Mathf.Abs(seno), 1f / exageracion) * Mathf.Sign(seno);

        Vector3 offset = new Vector3(
            oscilarX ? valorOscilacion * rangoX : 0f,
            oscilarY ? valorOscilacion * rangoY : 0f,
            oscilarZ ? valorOscilacion * rangoZ : 0f
        );

        transform.localScale = escalaInicial + offset;
    }

    public void ResetearOscilacion()
    {
        tiempo = 0f;
        transform.localScale = escalaInicial;
    }
}