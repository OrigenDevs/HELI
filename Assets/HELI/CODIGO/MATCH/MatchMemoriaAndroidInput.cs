using UnityEngine;

public class MatchMemoriaAndroidInput : MonoBehaviour
{
    [Header("Referencias")]
    public MatchCards matchCards;
    public MatchNavegacion navegacion;
    public Camera camara;

    [Header("Raycast")]
    public float distanciaRaycast = 100f;
    public LayerMask capaCartas;

    void Start()
    {
        if (navegacion != null)
            navegacion.gameObject.SetActive(false);
    }

    void Update()
    {
        if (matchCards == null || matchCards.inputBloqueado) return;
        if (matchCards.cartas == null) return;

        bool toco = false;
        Vector2 posicionPantalla = Vector2.zero;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                toco = true;
                posicionPantalla = touch.position;
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            toco = true;
            posicionPantalla = Input.mousePosition;
        }

        if (!toco) return;

        Camera cam = camara != null ? camara : Camera.main;
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(posicionPantalla);
        if (!Physics.Raycast(ray, out RaycastHit hit, distanciaRaycast, capaCartas)) return;

        Vector2Int? coord = EncontrarCoordenada(hit.transform);
        if (!coord.HasValue) return;

        matchCards.Seleccionar(coord.Value);
    }

    Vector2Int? EncontrarCoordenada(Transform carta)
    {
        for (int f = 0; f < matchCards.filas; f++)
            for (int c = 0; c < matchCards.columnas; c++)
                if (matchCards.cartas[f, c] == carta)
                    return new Vector2Int(f, c);

        return null;
    }
}