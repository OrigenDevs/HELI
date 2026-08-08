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

    public UnityEvent onSeleccion;
    public UnityEvent onMover;

    private Vector2Int indice;
    private bool bloqueado;
    private bool resaltadoInicializado;

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
        if (matchCards == null) return;

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

        Vector2Int nuevo = indice + dir;
        Vector2Int tamano = matchCards.TamanoGrid;
        if (nuevo.x >= 0 && nuevo.x < tamano.x && nuevo.y >= 0 && nuevo.y < tamano.y)
        {
            matchCards.Resaltar(indice, false);
            indice = nuevo;
            matchCards.Resaltar(indice, true);
            onMover.Invoke();
            bloqueado = true;
        }
    }
}
