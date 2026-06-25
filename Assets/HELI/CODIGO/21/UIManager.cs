using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Textos de puntuación")]
    public TMP_Text textoPuntosJugador;
    public TMP_Text textoPuntosIA;

    [Header("Velocidad de animación de puntos (segundos entre +1)")]
    public float velocidadAnimacionPuntos = 0.05f;

    [Header("Cajas del Jugador (lista de 21 GameObjects)")]
    public List<GameObject> cajasJugador = new List<GameObject>();

    [Header("Cajas de la IA (lista de 21 GameObjects)")]
    public List<GameObject> cajasIA = new List<GameObject>();

    [Header("Delay antes de empezar a prender cajas (segundos)")]
    public float delayAnimacionCajas = 0.5f;

    [Header("Intervalo entre cada caja prendiéndose (segundos)")]
    public float intervaloCajas = 0.05f;

    [Header("Botones del jugador")]
    public GameObject botonHit;
    public GameObject botonStand;

    // Contadores visuales actuales (independientes de la suma interna)
    private int visualJugador = 0;
    private int visualIA = 0;

    void Start()
    {
        // Apagar todas las cajas al inicio
        foreach (var c in cajasJugador) c.SetActive(false);
        foreach (var c in cajasIA)       c.SetActive(false);

        textoPuntosJugador.text = "0";
        textoPuntosIA.text      = "0";

        HabilitarBotonesJugador(false);
    }

    public void HabilitarBotonesJugador(bool estado)
    {
        botonHit.SetActive(estado);
        botonStand.SetActive(estado);

        if (estado)
            StartCoroutine(SeleccionarHit());
    }

    private IEnumerator SeleccionarHit()
    {
        yield return null;
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(botonHit);
    }

    // ─────────────────────────────────────────
    //  ANIMACIÓN DE PUNTOS Y CAJAS
    // ─────────────────────────────────────────

    /// <summary>
    /// Anima el texto de puntos y enciende las cajas correspondientes.
    /// esJugador=true → afecta al jugador, false → IA
    /// </summary>
    public IEnumerator AnimarPuntos(int valorGanado, bool esJugador)
    {
        int inicio   = esJugador ? visualJugador : visualIA;
        int objetivo = inicio + valorGanado;

        List<GameObject> cajas = esJugador ? cajasJugador : cajasIA;
        TMP_Text texto         = esJugador ? textoPuntosJugador : textoPuntosIA;

        int cajasEncendidasActuales = inicio; // cuántas cajas ya están prendidas

        // Ir sumando de uno en uno
        for (int i = inicio + 1; i <= objetivo; i++)
        {
            // Actualizar texto
            texto.text = i.ToString();

            // Encender la caja correspondiente (índice base 0)
            int indiceCaja = cajasEncendidasActuales;
            if (indiceCaja < cajas.Count)
            {
                cajas[indiceCaja].SetActive(true);
                cajasEncendidasActuales++;
            }

            yield return new WaitForSeconds(intervaloCajas);
        }

        // Guardar visual final
        if (esJugador) visualJugador = objetivo;
        else           visualIA      = objetivo;
    }
}