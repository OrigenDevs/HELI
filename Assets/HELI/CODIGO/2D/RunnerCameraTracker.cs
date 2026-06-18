using UnityEngine;

/// <summary>
/// Adjunta este script a la cámara.
/// Sigue al personaje solo en el eje de movimiento (horizontal),
/// manteniendo la distancia y posición inicial en los otros ejes.
/// </summary>
public class RunnerCameraTracker : MonoBehaviour
{
    [Header("Referencia")]
    [Tooltip("Arrastra aquí el GameObject del personaje")]
    public GameObject personaje;

    public enum EjeSeguimiento { X, Y, Z }

    [Header("Eje de seguimiento")]
    [Tooltip("Debe coincidir con el eje configurado en RunnerMovement")]
    public EjeSeguimiento eje = EjeSeguimiento.Z;

    [Header("Suavizado")]
    [Tooltip("0 = sin suavizado, valores mayores = más suave")]
    public float suavizado = 0f;

    // Offset inicial entre la cámara y el personaje
    private Vector3 offset;

    void Start()
    {
        if (personaje == null)
        {
            Debug.LogWarning("CameraFollow: no hay personaje asignado.");
            return;
        }

        // Guardar la distancia inicial en todos los ejes
        offset = transform.position - personaje.transform.position;
    }

    void LateUpdate()
    {
        if (personaje == null) return;

        // Posición objetivo: mantener offset pero seguir al personaje en el eje elegido
        Vector3 objetivo = transform.position;

        switch (eje)
        {
            case EjeSeguimiento.X:
                objetivo.x = personaje.transform.position.x + offset.x;
                break;
            case EjeSeguimiento.Y:
                objetivo.y = personaje.transform.position.y + offset.y;
                break;
            case EjeSeguimiento.Z:
                objetivo.z = personaje.transform.position.z + offset.z;
                break;
        }

        // Aplicar con o sin suavizado
        if (suavizado <= 0f)
            transform.position = objetivo;
        else
            transform.position = Vector3.Lerp(transform.position, objetivo, Time.deltaTime * (1f / suavizado) * 10f);
    }
}