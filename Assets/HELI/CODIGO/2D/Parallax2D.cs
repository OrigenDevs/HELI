using UnityEngine;

public class Parallax2D : MonoBehaviour
{
    [System.Serializable]
    public class Capa
    {
        public Transform transform;
        [Range(0f, 1f)]
        [Tooltip("0 = no se mueve (fondo fijo), 1 = se mueve igual que la cámara")]
        public float factor = 0.5f;
    }

    [Header("Cámaras")]
    public Camera camara;

    [Header("Capas de fondo (de más lejana a más cercana)")]
    public Capa[] capas;

    private Vector3 posicionCamaraInicial;
    private Vector3[] posicionesIniciales;

    void Start()
    {
        if (camara == null)
            camara = Camera.main;

        posicionCamaraInicial = camara.transform.position;

        posicionesIniciales = new Vector3[capas.Length];
        for (int i = 0; i < capas.Length; i++)
        {
            if (capas[i].transform != null)
                posicionesIniciales[i] = capas[i].transform.position;
        }
    }

    void LateUpdate()
    {
        Vector3 deltaCamara = camara.transform.position - posicionCamaraInicial;

        for (int i = 0; i < capas.Length; i++)
        {
            if (capas[i].transform == null) continue;
            capas[i].transform.position = posicionesIniciales[i] + deltaCamara * capas[i].factor;
        }
    }
}
