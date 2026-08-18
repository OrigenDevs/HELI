using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class CartaInteractuable : MonoBehaviour
{
    [Tooltip("Si esta activado, la carta responde a click/touch.")]
    public bool interactuable = true;

    public UnityEvent onClick;

    private MatchCards matchCards;

    void Start()
    {
        matchCards = FindFirstObjectByType<MatchCards>();

        if (interactuable)
        {
            MatchNavegacion nav = FindFirstObjectByType<MatchNavegacion>();
            if (nav != null) nav.gameObject.SetActive(false);
        }
    }

    void OnMouseDown()
    {
        if (!interactuable) return;
        if (matchCards == null || matchCards.inputBloqueado) return;
        if (matchCards.cartas == null) return;

        Vector2Int? coord = EncontrarCoordenada();
        if (!coord.HasValue) return;

        matchCards.Seleccionar(coord.Value);
        if (onClick != null) onClick.Invoke();
    }

    Vector2Int? EncontrarCoordenada()
    {
        for (int f = 0; f < matchCards.filas; f++)
            for (int c = 0; c < matchCards.columnas; c++)
                if (matchCards.cartas[f, c] == transform)
                    return new Vector2Int(f, c);

        return null;
    }
}