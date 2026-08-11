using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MatchNavegacion : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference accionMover;
    public InputActionReference accionSeleccion;

    [Header("Cursor")]
    public Transform cursor;
    public float velocidadMovimiento = 12f;
    public float umbralDireccion = 0.5f;

    [Header("Referencias")]
    public MatchCards matchCards;

    [Header("Camara")]
    public Transform camara;
    public float desplazamientoCamara = 0.15f;
    public float velocidadRetornoCamara = 5f;

    public UnityEvent onSeleccion;
    public UnityEvent onMover;

    private Vector2Int indice;
    private bool bloqueado;
    private bool resaltadoInicializado;
    private Vector3 posicionOriginalCamara;
    private Vector3 offsetCamara = Vector3.zero;
    private bool camaraGuardada;

    void OnEnable()
    {
        if (accionMover != null) accionMover.action.Enable();
        if (accionSeleccion != null) accionSeleccion.action.Enable();
    }

    void OnDisable()
    {
        if (accionMover != null) accionMover.action.Disable();
        if (accionSeleccion != null) accionSeleccion.action.Disable();
    }

    void Update()
    {
        if (matchCards == null || matchCards.inputBloqueado) return;

        if (!resaltadoInicializado && matchCards.cartas != null)
        {
            matchCards.Resaltar(indice, true);
            resaltadoInicializado = true;
        }

        if (accionSeleccion != null && accionSeleccion.action.WasPressedThisFrame())
        {
            matchCards.Seleccionar(indice);
            onSeleccion.Invoke();
        }

        if (accionMover != null)
        {
            Vector2 input = accionMover.action.ReadValue<Vector2>();
            ProcesarDireccion(input);
        }

        if (cursor != null)
            cursor.position = Vector3.MoveTowards(cursor.position, matchCards.PosicionCarta(indice), velocidadMovimiento * Time.deltaTime);

        ActualizarCamara();
    }

    void ActualizarCamara()
    {
        if (camara == null) return;

        if (!camaraGuardada)
        {
            posicionOriginalCamara = camara.position;
            camaraGuardada = true;
        }

        offsetCamara = Vector3.Lerp(offsetCamara, Vector3.zero, Time.deltaTime * velocidadRetornoCamara);
        camara.position = posicionOriginalCamara + offsetCamara;
    }

    void ProcesarDireccion(Vector2 input)
    {
        if (input.magnitude < umbralDireccion)
        {
            bloqueado = false;
            return;
        }

        if (bloqueado) return;

        Vector2Int dir = Vector2Int.zero;
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            dir = new Vector2Int(0, input.x > 0f ? 1 : -1);
        else
            dir = new Vector2Int(input.y > 0f ? 1 : -1, 0);

        Vector2Int adyacente = Envolver(indice + dir, matchCards.TamanoGrid);
        if (adyacente != indice && matchCards.CartaEmparejada(adyacente) && !matchCards.pasarSobreEmparejadas)
            matchCards.ReproducirSonidoBloqueada();

        Vector2Int? destino = BuscarCartaMasCercana(dir);
        if (destino.HasValue)
        {
            matchCards.Resaltar(indice, false);
            indice = destino.Value;
            matchCards.Resaltar(indice, true);
            onMover.Invoke();
            bloqueado = true;

            Vector3 direccion = new Vector3(dir.y, 0f, dir.x);
            offsetCamara = direccion * desplazamientoCamara;
        }
    }

    Vector2Int? BuscarCartaMasCercana(Vector2Int dir)
    {
        Vector2Int tamano = matchCards.TamanoGrid;

        Vector2Int pos = indice;
        int pasos = dir.x != 0 ? tamano.x : tamano.y;
        for (int i = 0; i < pasos; i++)
        {
            pos = Envolver(pos + dir, tamano);
            if (pos != indice && (matchCards.pasarSobreEmparejadas || !matchCards.CartaEmparejada(pos)))
                return pos;
        }

        Vector2Int perpendicular = new Vector2Int(dir.y, dir.x);
        Vector2Int? mejor = null;
        int mejorCoste = int.MaxValue;

        for (int f = 0; f < tamano.x; f++)
        {
            for (int c = 0; c < tamano.y; c++)
            {
                Vector2Int celda = new Vector2Int(f, c);
                if (celda == indice) continue;
                if (!matchCards.pasarSobreEmparejadas && matchCards.CartaEmparejada(celda)) continue;

                Vector2Int delta = DeltaEnvolvente(celda, indice, tamano);
                int avance = delta.x * dir.x + delta.y * dir.y;
                if (avance <= 0) continue;

                int coste = avance * 10 + Mathf.Abs(delta.x * perpendicular.x + delta.y * perpendicular.y);
                if (coste < mejorCoste)
                {
                    mejorCoste = coste;
                    mejor = celda;
                }
            }
        }

        return mejor;
    }

    Vector2Int DeltaEnvolvente(Vector2Int a, Vector2Int b, Vector2Int tamano)
    {
        int dx = a.x - b.x;
        int dy = a.y - b.y;
        if (dx > tamano.x / 2) dx -= tamano.x;
        if (dx < -tamano.x / 2) dx += tamano.x;
        if (dy > tamano.y / 2) dy -= tamano.y;
        if (dy < -tamano.y / 2) dy += tamano.y;
        return new Vector2Int(dx, dy);
    }

    Vector2Int Envolver(Vector2Int coord, Vector2Int tamano)
    {
        int x = coord.x % tamano.x;
        int y = coord.y % tamano.y;
        if (x < 0) x += tamano.x;
        if (y < 0) y += tamano.y;
        return new Vector2Int(x, y);
    }
}
