using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchHUD : MonoBehaviour
{
    [Header("Textos")]
    public TMP_Text textoPuntosP1;
    public TMP_Text textoPuntosP2;
    public TMP_Text textoTurno;
    public TMP_Text textoCronometro;

    [Header("Modo solo")]
    public GameObject panelP2;

    [Header("Indicador de turno")]
    public Image imagenTurnoP1;
    public Image imagenTurnoP2;

    [Header("Racha")]
    public EcoAnim ecoAnimP1;
    public EcoAnim ecoAnimP2;
    public GameObject imagenRachaP1;
    public GameObject imagenRachaP2;

    private float cronometro = 0f;
    private bool cronometroActivo = false;

    void Update()
    {
        if (!cronometroActivo) return;
        cronometro += Time.deltaTime;
        ActualizarCronometroTexto();
    }

    public void ConfigurarModo(int jugadores)
    {
        bool esSolo = jugadores == 1;

        if (panelP2 != null) panelP2.SetActive(!esSolo);
        if (textoPuntosP2 != null) textoPuntosP2.gameObject.SetActive(!esSolo);
        if (textoCronometro != null) textoCronometro.gameObject.SetActive(esSolo);

        cronometro = 0f;
        cronometroActivo = esSolo;
        ActualizarCronometroTexto();
    }

    public void ActualizarPuntos(int jugador, int puntos)
    {
        TMP_Text texto = jugador == 0 ? textoPuntosP1 : textoPuntosP2;
        if (texto != null) texto.text = puntos.ToString();
    }

    public void ActualizarTurno(int jugador)
    {
        if (textoTurno != null) textoTurno.text = "Turno de P" + (jugador + 1);
    }

    public void ActualizarIntentos(int intentos)
    {
        if (textoTurno != null) textoTurno.text = "Intentos: " + intentos;
    }

    public void OcultarTurno()
    {
        if (textoTurno != null) textoTurno.text = "";
    }

    public void MostrarIndicadorTurno(int jugador)
    {
        if (imagenTurnoP1 != null) imagenTurnoP1.gameObject.SetActive(jugador == 0);
        if (imagenTurnoP2 != null) imagenTurnoP2.gameObject.SetActive(jugador == 1);
    }

    public void OcultarIndicadorTurno()
    {
        if (imagenTurnoP1 != null) imagenTurnoP1.gameObject.SetActive(false);
        if (imagenTurnoP2 != null) imagenTurnoP2.gameObject.SetActive(false);
    }

    public void ActualizarRacha(int jugador, bool rachaActiva)
    {
        EcoAnim eco = jugador == 0 ? ecoAnimP1 : ecoAnimP2;
        if (eco != null)
        {
            if (rachaActiva) eco.Activar();
            else eco.Desactivar();
        }

        GameObject imagen = jugador == 0 ? imagenRachaP1 : imagenRachaP2;
        if (imagen != null) imagen.SetActive(rachaActiva);
    }

    public void OcultarRachas()
    {
        ActualizarRacha(0, false);
        ActualizarRacha(1, false);
    }

    public void DetenerCronometro()
    {
        cronometroActivo = false;
    }

    public float TiempoActual() => cronometro;

    void ActualizarCronometroTexto()
    {
        if (textoCronometro == null) return;

        int minutos = Mathf.FloorToInt(cronometro / 60f);
        int segundos = Mathf.FloorToInt(cronometro % 60f);
        textoCronometro.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }
}
