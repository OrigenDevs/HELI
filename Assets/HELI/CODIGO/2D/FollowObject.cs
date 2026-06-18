using UnityEngine;

/// <summary>
/// Este objeto seguirá la posición de otro sin necesidad de ser su hijo.
/// Puedes elegir qué ejes seguir y si quieres mantener un offset.
/// </summary>
public class FollowObject : MonoBehaviour
{
    [Header("Referencia")]
    [Tooltip("El objeto al que seguir")]
    public Transform objetivo;

    [Header("Ejes a seguir")]
    public bool seguirX = true;
    public bool seguirY = true;
    public bool seguirZ = true;

    [Header("Offset")]
    [Tooltip("Si es true, mantiene la distancia inicial. Si es false, se pega exactamente al objetivo.")]
    public bool mantenerOffsetInicial = true;
    private Vector3 offset;

    [Header("Suavizado")]
    [Tooltip("0 = instantáneo, valores mayores = más suave")]
    public float suavizado = 0f;

    void Start()
    {
        if (objetivo == null)
        {
            Debug.LogWarning("FollowObject: no hay objetivo asignado.");
            return;
        }

        offset = transform.position - objetivo.position;
    }

    void LateUpdate()
    {
        if (objetivo == null) return;

        Vector3 posObjetivo = objetivo.position + (mantenerOffsetInicial ? offset : Vector3.zero);
        Vector3 nuevaPos = transform.position;

        if (seguirX) nuevaPos.x = posObjetivo.x;
        if (seguirY) nuevaPos.y = posObjetivo.y;
        if (seguirZ) nuevaPos.z = posObjetivo.z;

        if (suavizado <= 0f)
            transform.position = nuevaPos;
        else
            transform.position = Vector3.Lerp(transform.position, nuevaPos, Time.deltaTime * (1f / suavizado) * 10f);
    }
}