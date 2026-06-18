using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo")]
    [Tooltip("El personaje que la cámara seguirá")]
    public Transform objetivo;

    [Header("Configuración de Seguimiento")]
    [Tooltip("Velocidad de seguimiento (menor = más delay/suave, mayor = más rápido)")]
    [Range(0.1f, 20f)]
    public float velocidadSeguimiento = 5f;
    
    [Tooltip("Usar suavizado con Lerp (recomendado)")]
    public bool usarSuavizado = true;

    [Header("Offset y Distancia")]
    [Tooltip("Mantener el offset inicial automáticamente al iniciar")]
    public bool mantenerOffsetInicial = true;
    
    [Tooltip("Offset manual (solo si 'Mantener Offset Inicial' está desactivado)")]
    public Vector3 offsetManual = new Vector3(0f, 5f, -7f);

    [Header("Límites de Posición (Opcional)")]
    [Tooltip("Activar límites de posición")]
    public bool usarLimites = false;
    
    [Tooltip("Límite mínimo en X")]
    public float limiteMinX = -50f;
    
    [Tooltip("Límite máximo en X")]
    public float limiteMaxX = 50f;
    
    [Tooltip("Límite mínimo en Z")]
    public float limiteMinZ = -50f;
    
    [Tooltip("Límite máximo en Z")]
    public float limiteMaxZ = 50f;

    [Header("Debug")]
    [Tooltip("Mostrar información de debug")]
    public bool mostrarDebug = false;

    // Variables privadas
    private Vector3 offsetInicial;
    private Vector3 posicionObjetivo;

    void Start()
    {
        // Verificar que hay un objetivo asignado
        if (objetivo == null)
        {
            Debug.LogError("⚠️ CAMERAFOLLOW: No se ha asignado un objetivo! Asigna el personaje en el Inspector.");
            enabled = false;
            return;
        }

        // Guardar el offset inicial (distancia entre cámara y personaje al comenzar)
        if (mantenerOffsetInicial)
        {
            offsetInicial = transform.position - objetivo.position;
            
            if (mostrarDebug)
            {
                Debug.Log($"=== CAMERAFOLLOW INICIADA ===");
                Debug.Log($"Offset inicial calculado: {offsetInicial}");
                Debug.Log($"Posición inicial cámara: {transform.position}");
                Debug.Log($"Posición inicial objetivo: {objetivo.position}");
            }
        }
        else
        {
            offsetInicial = offsetManual;
            
            if (mostrarDebug)
            {
                Debug.Log($"=== CAMERAFOLLOW INICIADA ===");
                Debug.Log($"Usando offset manual: {offsetManual}");
            }
        }
    }

    void LateUpdate()
    {
        // Verificar que el objetivo existe
        if (objetivo == null)
            return;

        // Calcular la posición objetivo de la cámara
        // IMPORTANTE: Solo usamos la POSICIÓN del objetivo, NO su rotación
        posicionObjetivo = objetivo.position + offsetInicial;

        // Aplicar límites si están activados
        if (usarLimites)
        {
            posicionObjetivo.x = Mathf.Clamp(posicionObjetivo.x, limiteMinX, limiteMaxX);
            posicionObjetivo.z = Mathf.Clamp(posicionObjetivo.z, limiteMinZ, limiteMaxZ);
        }

        // Mover la cámara hacia la posición objetivo
        if (usarSuavizado)
        {
            // Movimiento suave con Lerp
            transform.position = Vector3.Lerp(
                transform.position, 
                posicionObjetivo, 
                velocidadSeguimiento * Time.deltaTime
            );
        }
        else
        {
            // Movimiento directo (sin delay)
            transform.position = posicionObjetivo;
        }

        // Debug info
        if (mostrarDebug && Time.frameCount % 60 == 0)
        {
            float distancia = Vector3.Distance(transform.position, posicionObjetivo);
            Debug.Log($"Distancia al objetivo: {distancia:F2} | Velocidad: {velocidadSeguimiento}");
        }
    }

    // Método público para cambiar el objetivo en runtime
    public void CambiarObjetivo(Transform nuevoObjetivo)
    {
        if (nuevoObjetivo != null)
        {
            objetivo = nuevoObjetivo;
            
            // Recalcular offset si es necesario
            if (mantenerOffsetInicial)
            {
                offsetInicial = transform.position - objetivo.position;
            }
            
            if (mostrarDebug)
            {
                Debug.Log($"Objetivo cambiado a: {nuevoObjetivo.name}");
            }
        }
    }

    // Método público para ajustar el offset manualmente
    public void AjustarOffset(Vector3 nuevoOffset)
    {
        offsetInicial = nuevoOffset;
        
        if (mostrarDebug)
        {
            Debug.Log($"Offset ajustado a: {nuevoOffset}");
        }
    }

    // Visualización en el editor
    void OnDrawGizmos()
    {
        if (objetivo == null)
            return;

        // Línea desde la cámara al objetivo
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, objetivo.position);

        // Esfera en la posición objetivo de la cámara
        Gizmos.color = Color.yellow;
        Vector3 offset = mantenerOffsetInicial && Application.isPlaying ? offsetInicial : offsetManual;
        Vector3 targetPos = objetivo.position + offset;
        Gizmos.DrawWireSphere(targetPos, 0.5f);

        // Dibujar límites si están activados
        if (usarLimites)
        {
            Gizmos.color = Color.red;
            
            // Esquinas del área limitada (en el plano XZ)
            Vector3 esquina1 = new Vector3(limiteMinX, transform.position.y, limiteMinZ);
            Vector3 esquina2 = new Vector3(limiteMaxX, transform.position.y, limiteMinZ);
            Vector3 esquina3 = new Vector3(limiteMaxX, transform.position.y, limiteMaxZ);
            Vector3 esquina4 = new Vector3(limiteMinX, transform.position.y, limiteMaxZ);

            // Dibujar rectángulo de límites
            Gizmos.DrawLine(esquina1, esquina2);
            Gizmos.DrawLine(esquina2, esquina3);
            Gizmos.DrawLine(esquina3, esquina4);
            Gizmos.DrawLine(esquina4, esquina1);
        }
    }
}