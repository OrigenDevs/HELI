using UnityEngine;

/// <summary>
/// Oscilación automática suave con control independiente por ejes
/// Usa funciones seno para crear movimiento natural con aceleración/desaceleración
/// </summary>
public class OscilacionAutomatica : MonoBehaviour
{
    [Header("Configuración de Oscilación")]
    [Tooltip("Velocidad de oscilación (mayor = más rápido)")]
    [Range(0.1f, 10f)]
    public float velocidad = 1f;
    
    [Tooltip("Rango de oscilación (distancia desde el centro en cada dirección)")]
    [Range(0f, 50f)]
    public float rango = 2f;

    [Header("Ejes de Oscilación")]
    [Tooltip("Oscilar en el eje X")]
    public bool oscilarX = false;
    
    [Tooltip("Oscilar en el eje Y")]
    public bool oscilarY = true;
    
    [Tooltip("Oscilar en el eje Z")]
    public bool oscilarZ = false;

    // Variables privadas
    private Vector3 posicionInicial;
    private float tiempo = 0f;

    void Start()
    {
        // Guardar la posición inicial del objeto
        posicionInicial = transform.position;
    }

    void Update()
    {
        // Incrementar el tiempo
        tiempo += Time.deltaTime * velocidad;

        // Calcular el offset de oscilación usando seno (suave y natural)
        float offsetOscilacion = Mathf.Sin(tiempo) * rango;

        // Crear vector de offset según los ejes activados
        Vector3 offset = new Vector3(
            oscilarX ? offsetOscilacion : 0f,
            oscilarY ? offsetOscilacion : 0f,
            oscilarZ ? offsetOscilacion : 0f
        );

        // Aplicar la nueva posición
        transform.position = posicionInicial + offset;
    }

    // Método para resetear la oscilación
    public void ResetearOscilacion()
    {
        tiempo = 0f;
        transform.position = posicionInicial;
    }
}