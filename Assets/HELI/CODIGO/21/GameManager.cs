using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Baraja")]
    public List<Carta> todasLasCartas = new List<Carta>();

    [Header("Turno Inicial (true = Jugador, false = IA)")]
    public bool jugadorEmpieza = true;

    [Header("Transición de Turno - Delay switch de menús (segundos)")]
    public float delaySwitch = 1f;

    [Header("Tiempo que se muestra la transición de turno")]
    public float tiempoTransicion = 2f;

    [Header("Referencia al CardDisplay (objeto 3D de carta)")]
    public CardDisplay cardDisplay;

    [Header("Referencia al UIManager")]
    public UIManager uiManager;

    [Header("Referencia al AIController")]
    public AIController aiController;

    [Header("Menús de turno")]
    public GameObject menuTurnoJugador;
    public GameObject menuTurnoIA;

    [Header("Transiciones de turno (objetos con Animator)")]
    public GameObject transicionJugador;
    public GameObject transicionIA;

    [Header("Menús de resultado")]
    public GameObject menuVictoria;
    public GameObject menuDerrota;

    // Estado interno del juego
    [HideInInspector] public List<Carta> baraja = new List<Carta>();
    [HideInInspector] public Carta cartaActual;

    [HideInInspector] public int sumaJugador = 0;
    [HideInInspector] public int sumaIA = 0;

    [HideInInspector] public bool jugadorPlantado = false;
    [HideInInspector] public bool iaPlantada = false;

    [HideInInspector] public bool esturnoJugador = true;
    [HideInInspector] public bool juegoTerminado = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        IniciarPartida();
    }

    // ─────────────────────────────────────────
    //  INICIO DE PARTIDA
    // ─────────────────────────────────────────
    void IniciarPartida()
    {
        juegoTerminado = false;
        sumaJugador = 0;
        sumaIA = 0;
        jugadorPlantado = false;
        iaPlantada = false;

        menuVictoria.SetActive(false);
        menuDerrota.SetActive(false);
        transicionJugador.SetActive(false);
        transicionIA.SetActive(false);

        // Mezclar baraja
        baraja = new List<Carta>(todasLasCartas);
        MezclarBaraja();

        // Asignar primera carta
        AsignarCartaActual();

        // Iniciar primer turno
        esturnoJugador = jugadorEmpieza;
        StartCoroutine(IniciarTurno());
    }

    void MezclarBaraja()
    {
        for (int i = baraja.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Carta tmp = baraja[i];
            baraja[i] = baraja[j];
            baraja[j] = tmp;
        }
    }

    // ─────────────────────────────────────────
    //  CARTA ACTUAL
    // ─────────────────────────────────────────
    public void AsignarCartaActual()
    {
        if (baraja.Count == 0)
        {
            Debug.LogWarning("¡La baraja está vacía!");
            return;
        }
        cartaActual = baraja[0];
        cardDisplay.ActualizarMaterial(cartaActual.imagen);
    }

    public void EliminarCartaActualYAvanzar()
    {
        if (baraja.Count > 0) baraja.RemoveAt(0);
        AsignarCartaActual();
    }

    // ─────────────────────────────────────────
    //  GESTIÓN DE TURNOS
    // ─────────────────────────────────────────
    public IEnumerator IniciarTurno()
    {
        if (juegoTerminado) yield break;

        yield return new WaitForSeconds(delaySwitch);

        if (esturnoJugador)
        {
            menuTurnoIA.SetActive(false);
            menuTurnoJugador.SetActive(true);
            yield return StartCoroutine(MostrarTransicion(transicionJugador));
            uiManager.HabilitarBotonesJugador(true);
        }
        else
        {
            menuTurnoJugador.SetActive(false);
            menuTurnoIA.SetActive(true);
            yield return StartCoroutine(MostrarTransicion(transicionIA));
            aiController.EjecutarTurnoIA();
        }
    }

    IEnumerator MostrarTransicion(GameObject transicion)
    {
        // Reiniciar animación
        transicion.SetActive(false);
        yield return null;
        transicion.SetActive(true);

        // Reiniciar el Animator
        Animator anim = transicion.GetComponent<Animator>();
        if (anim != null)
        {
            anim.Rebind();
            anim.Update(0f);
        }

        yield return new WaitForSeconds(tiempoTransicion);
        transicion.SetActive(false);
    }

    public void CambiarTurno()
    {
        esturnoJugador = !esturnoJugador;
        StartCoroutine(IniciarTurno());
    }

    // ─────────────────────────────────────────
    //  VERIFICAR VICTORIA
    // ─────────────────────────────────────────
    public void VerificarEstado(bool esJugador)
    {
        int suma = esJugador ? sumaJugador : sumaIA;

        if (suma == 21)
        {
            TerminarJuego(esJugador);
            return;
        }

        if (suma > 21)
        {
            TerminarJuego(!esJugador); // el otro gana
            return;
        }

        // Si ambos están plantados → comparar sumas
        if (jugadorPlantado && iaPlantada)
        {
            TerminarJuego(sumaJugador >= sumaIA);
            return;
        }

        CambiarTurno();
    }

    void TerminarJuego(bool ganoJugador)
    {
        juegoTerminado = true;
        uiManager.HabilitarBotonesJugador(false);
        menuTurnoJugador.SetActive(false);
        menuTurnoIA.SetActive(false);

        if (ganoJugador)
            menuVictoria.SetActive(true);
        else
            menuDerrota.SetActive(true);
    }
}