using UnityEngine;

public class Parallax2D : MonoBehaviour
{
    [System.Serializable]
    public class Capa
    {
        public Transform transform;
        [Tooltip("0 = no se mueve, 1 = igual que el personaje, >1 = más rápido que el personaje")]
        public float factor = 0.5f;
    }

    [Header("Referencia al personaje")]
    public Transform personaje;

    [Header("Intensidad global")]
    [Tooltip("Multiplica el efecto parallax. 2 = el doble de movimiento en todas las capas")]
    public float intensidad = 2f;

    [Header("Eje de movimiento")]
    public bool ejeX = true;
    public bool ejeY;
    public bool ejeZ = true;

    [Header("Capas de fondo (de más lejana a más cercana)")]
    public Capa[] capas;

    private Vector3 posicionPersonajeInicial;
    private Vector3[] posicionesIniciales;

    void Start()
    {
        if (personaje == null)
        {
            var tracker = FindAnyObjectByType<RunnerCameraTracker>();
            if (tracker != null) personaje = tracker.personaje?.transform;
        }

        if (personaje == null) return;

        posicionPersonajeInicial = personaje.position;

        posicionesIniciales = new Vector3[capas.Length];
        for (int i = 0; i < capas.Length; i++)
        {
            if (capas[i].transform != null)
                posicionesIniciales[i] = capas[i].transform.position;
        }
    }

    void LateUpdate()
    {
        if (personaje == null) return;

        Vector3 delta = personaje.position - posicionPersonajeInicial;
        if (!ejeX) delta.x = 0f;
        if (!ejeY) delta.y = 0f;
        if (!ejeZ) delta.z = 0f;

        for (int i = 0; i < capas.Length; i++)
        {
            if (capas[i].transform == null) continue;
            capas[i].transform.position = posicionesIniciales[i] + delta * capas[i].factor * intensidad;
        }
    }
}
