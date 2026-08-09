using UnityEngine;
using UnityEngine.Events;

public class MenuInicioMatch : MonoBehaviour
{
    [Header("UI")]
    public GameObject panelInicio;

    [Header("Juego")]
    public MatchCards matchCards;
    public MatchNavegacion navegacion;

    public UnityEvent onJuegoIniciado;

    public static int cantidadJugadores = 1;

    void Awake()
    {
        if (matchCards != null) matchCards.enabled = false;
        if (navegacion != null) navegacion.enabled = false;
    }

    public void Elegir1Jugador()
    {
        cantidadJugadores = 1;
        Iniciar();
    }

    public void Elegir2Jugadores()
    {
        cantidadJugadores = 2;
        Iniciar();
    }

    void Iniciar()
    {
        if (matchCards != null) matchCards.enabled = true;
        if (navegacion != null) navegacion.enabled = true;

        if (panelInicio != null) panelInicio.SetActive(false);

        onJuegoIniciado.Invoke();
    }
}
