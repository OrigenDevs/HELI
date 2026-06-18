using UnityEngine;

/// <summary>
/// Desactiva el objeto automáticamente después de un tiempo
/// </summary>
public class DesactivarTiempo : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Tiempo en segundos antes de desactivarse")]
    public float tiempoDesactivacion = 5f;
    
    [Tooltip("¿Destruir el objeto en lugar de solo desactivarlo?")]
    public bool autodestruible = false;

    void OnEnable()
    {
        // Cancelar invoke anterior si existe
        CancelInvoke();
        
        // Programar desactivación
        Invoke(nameof(Desactivar), tiempoDesactivacion);
    }

    void Desactivar()
    {
        if (autodestruible)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void OnDisable()
    {
        // Cancelar invoke al desactivarse
        CancelInvoke();
    }
}