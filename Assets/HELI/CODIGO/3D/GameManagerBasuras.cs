using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManagerBasuras : MonoBehaviour
{
    [Header("Objetos Basura")]
    [Tooltip("Lista de GameObjects con trigger que el jugador debe recoger.")]
    public List<GameObject> objetosBasura = new List<GameObject>();

    [Header("Puntuación")]
    [Tooltip("Puntos que se suman por cada basura recogida.")]
    public int puntosPorBasura = 10;

    [Tooltip("Texto del canvas donde se muestra la puntuación.")]
    public TextMeshProUGUI textoPuntuacion;

    [Tooltip("Velocidad a la que sube el contador de puntos en pantalla.")]
    public float velocidadAnimacionPuntos = 200f;

    [Header("Barra de Limpieza")]
    [Tooltip("Transform del objeto que representa la barra de limpieza.")]
    public Transform barraLimpieza;

    [Tooltip("Eje sobre el que se escala la barra (X, Y o Z).")]
    public EjeBarra ejeBarra = EjeBarra.X;

    [Tooltip("Cantidad de triggers necesarios para llenar la barra (escala 1).")]
    public int triggersParaCompletar = 10;

    [Tooltip("Velocidad a la que crece la barra de limpieza (escala por segundo).")]
    public float velocidadBarra = 1f;

    [Header("Victoria")]
    [Tooltip("Objeto que se activa cuando se completan todos los triggers necesarios.")]
    public GameObject pantallaVictoria;

    [Tooltip("Tag del jugador.")]
    public string tagJugador = "Player";

    // Estado interno
    private int puntuacionActual = 0;
    private float puntuacionMostrada = 0f;
    private int triggersCompletados = 0;
    private bool victoriaAlcanzada = false;
    private float escalaActual = 0f;
    private float escalaObjetivo = 0f;

    void Start()
    {
        if (pantallaVictoria != null)
            pantallaVictoria.SetActive(false);

        escalaActual = 0f;
        escalaObjetivo = 0f;
        ActualizarEscalaBarra(0f);

        if (textoPuntuacion != null)
            textoPuntuacion.text = "0";

        foreach (GameObject obj in objetosBasura)
        {
            if (obj == null) continue;

            Collider col = obj.GetComponent<Collider>();
            if (col == null) { Debug.LogWarning($"⚠️ '{obj.name}' no tiene Collider."); continue; }
            if (!col.isTrigger) Debug.LogWarning($"⚠️ '{obj.name}' no tiene Is Trigger activado.");

            DetectorBasura detector = obj.GetComponent<DetectorBasura>();
            if (detector == null) detector = obj.AddComponent<DetectorBasura>();
            detector.Inicializar(this, tagJugador);
        }
    }

    void Update()
    {
        // Animar puntuación
        if (puntuacionMostrada < puntuacionActual)
        {
            puntuacionMostrada = Mathf.MoveTowards(puntuacionMostrada, puntuacionActual, velocidadAnimacionPuntos * Time.deltaTime);
            if (textoPuntuacion != null)
                textoPuntuacion.text = Mathf.FloorToInt(puntuacionMostrada).ToString();
        }

        // Animar barra suavemente hacia el objetivo
        if (!Mathf.Approximately(escalaActual, escalaObjetivo))
        {
            escalaActual = Mathf.MoveTowards(escalaActual, escalaObjetivo, velocidadBarra * Time.deltaTime);
            ActualizarEscalaBarra(escalaActual);
        }
    }

    public void RegistrarBasuraRecogida()
    {
        if (victoriaAlcanzada) return;

        triggersCompletados++;
        puntuacionActual += puntosPorBasura;

        escalaObjetivo = Mathf.Clamp01((float)triggersCompletados / triggersParaCompletar);

        Debug.Log($"🗑️ {triggersCompletados}/{triggersParaCompletar} | Puntos: {puntuacionActual}");

        if (triggersCompletados >= triggersParaCompletar)
            ActivarVictoria();
    }

    void ActualizarEscalaBarra(float valor)
    {
        if (barraLimpieza == null) return;

        Vector3 escala = barraLimpieza.localScale;
        switch (ejeBarra)
        {
            case EjeBarra.X: escala.x = valor; break;
            case EjeBarra.Y: escala.y = valor; break;
            case EjeBarra.Z: escala.z = valor; break;
        }
        barraLimpieza.localScale = escala;
    }

    void ActivarVictoria()
    {
        victoriaAlcanzada = true;
        if (pantallaVictoria != null)
            pantallaVictoria.SetActive(true);
        Debug.Log("🏆 ¡NIVEL COMPLETADO!");
    }
}

public enum EjeBarra { X, Y, Z }

public class DetectorBasura : MonoBehaviour
{
    private GameManagerBasuras manager;
    private string tagJugador;
    private bool recogido = false;

    public void Inicializar(GameManagerBasuras manager, string tagJugador)
    {
        this.manager = manager;
        this.tagJugador = tagJugador;
    }

    void OnTriggerEnter(Collider other)
    {
        if (recogido) return;
        if (!other.CompareTag(tagJugador)) return;

        recogido = true;
        manager.RegistrarBasuraRecogida();

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }
}