using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Sistema de diálogos con audio sincronizado
/// Soporta diálogos automáticos y diálogos con botón de continuar
/// </summary>
public class DialogueSystem : MonoBehaviour
{
    [Header("Referencias de UI")]
    [Tooltip("TextMeshPro donde se muestra el diálogo")]
    public TextMeshProUGUI textoDialogo;
    
    [Tooltip("GameObject del panel completo de diálogo (se desactiva al terminar)")]
    public GameObject panelDialogo;
    
    [Tooltip("Botón para continuar al siguiente diálogo")]
    public Button botonContinuar;

    [Header("Lista de Diálogos")]
    [Tooltip("Lista de todos los diálogos a reproducir en orden")]
    public List<Dialogo> dialogos = new List<Dialogo>();

    [Header("Configuración")]
    [Tooltip("Pausa en segundos después de que termina cada diálogo antes de pasar al siguiente")]
    [Range(0f, 5f)]
    public float pausaEntreDialogos = 1f;

    [Tooltip("Ajuste global de tiempo (en segundos) que se suma a la duración del audio para calcular la velocidad de escritura. " +
             "Negativo = el texto termina ANTES que el audio. Positivo = el texto termina DESPUÉS. " +
             "Se aplica a todos los diálogos; úsalo como corrección general de desfase.")]
    [Range(-5f, 5f)]
    public float ajusteGlobalTiempo = 0f;

    // Variables privadas
    private int indiceDialogoActual = 0;
    private bool esperandoBotonContinuar = false;
    private Coroutine coroutinaEscritura;
    private AudioSource audioSource;

    void Start()
    {
        // Obtener o crear AudioSource automáticamente
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Verificar referencias
        VerificarReferencias();

        // Configurar botón de continuar
        if (botonContinuar != null)
        {
            botonContinuar.onClick.AddListener(AlPresionarContinuar);
            botonContinuar.gameObject.SetActive(false);
        }

        // Iniciar sistema de diálogos
        IniciarDialogos();
    }

    /// <summary>
    /// Verifica que todas las referencias estén asignadas
    /// </summary>
    void VerificarReferencias()
    {
        if (textoDialogo == null)
            Debug.LogError("⚠️ DIALOGUESYSTEM: No se ha asignado el TextMeshPro!");

        if (panelDialogo == null)
            Debug.LogError("⚠️ DIALOGUESYSTEM: No se ha asignado el Panel de Diálogo!");

        if (botonContinuar == null)
            Debug.LogWarning("⚠️ DIALOGUESYSTEM: No se ha asignado el Botón Continuar!");

        if (dialogos.Count == 0)
            Debug.LogWarning("⚠️ DIALOGUESYSTEM: No hay diálogos en la lista!");
    }

    /// <summary>
    /// Inicia el sistema de diálogos desde el principio
    /// </summary>
    public void IniciarDialogos()
    {
        if (dialogos.Count == 0)
        {
            Debug.LogWarning("No hay diálogos para mostrar");
            return;
        }

        indiceDialogoActual = 0;
        
        if (panelDialogo != null)
            panelDialogo.SetActive(true);

        MostrarDialogoActual();
    }

    /// <summary>
    /// Muestra el diálogo en el índice actual
    /// </summary>
    void MostrarDialogoActual()
    {
        if (indiceDialogoActual >= dialogos.Count)
        {
            // Ya no hay más diálogos
            FinalizarDialogos();
            return;
        }

        Dialogo dialogoActual = dialogos[indiceDialogoActual];

        // Detener cualquier escritura anterior
        if (coroutinaEscritura != null)
        {
            StopCoroutine(coroutinaEscritura);
        }

        // Iniciar escritura del diálogo actual
        coroutinaEscritura = StartCoroutine(EscribirDialogo(dialogoActual));
    }

    /// <summary>
    /// Escribe el diálogo letra por letra sincronizado con el audio.
    /// La duración efectiva se calcula como:
    ///   duracionAudio + ajusteGlobalTiempo + dialogo.ajusteIndividualTiempo
    /// </summary>
    IEnumerator EscribirDialogo(Dialogo dialogo)
    {
        esperandoBotonContinuar = false;

        // Limpiar texto
        textoDialogo.text = "";

        // Reproducir audio y obtener duración
        float duracionAudio = 0f;
        if (dialogo.audioClip != null && audioSource != null)
        {
            audioSource.clip = dialogo.audioClip;
            audioSource.Play();
            duracionAudio = dialogo.audioClip.length;
        }
        else
        {
            Debug.LogWarning($"⚠️ No hay audio asignado en el diálogo: {dialogo.texto.Substring(0, Mathf.Min(30, dialogo.texto.Length))}...");
            yield break;
        }

        // Duración efectiva: audio real + ajuste global + ajuste individual del diálogo
        // Un valor negativo adelanta el texto; uno positivo lo retrasa.
        float duracionEfectiva = duracionAudio + ajusteGlobalTiempo + dialogo.ajusteIndividualTiempo;

        // Protección: garantizar un mínimo para evitar velocidades absurdas
        duracionEfectiva = Mathf.Max(duracionEfectiva, 0.1f);

        // Calcular tiempo entre caracteres para que la escritura termine en duracionEfectiva
        float tiempoEntreCar = duracionEfectiva / dialogo.texto.Length;

        // Escribir letra por letra
        foreach (char letra in dialogo.texto)
        {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(tiempoEntreCar);
        }

        // Esperar a que el audio termine completamente antes de avanzar,
        // independientemente de si el texto terminó antes o después.
        while (audioSource.isPlaying)
        {
            yield return null;
        }

        // Verificar si requiere botón de continuar
        if (dialogo.requiereBotonContinuar)
        {
            MostrarBotonContinuar();
        }
        else
        {
            // Pausa antes de continuar automáticamente
            yield return new WaitForSeconds(pausaEntreDialogos);
            SiguienteDialogo();
        }
    }

    /// <summary>
    /// Muestra el botón de continuar y espera interacción
    /// </summary>
    void MostrarBotonContinuar()
    {
        if (botonContinuar != null)
        {
            botonContinuar.gameObject.SetActive(true);
            esperandoBotonContinuar = true;
        }
        else
        {
            Debug.LogWarning("⚠️ Se requiere botón continuar pero no está asignado. Continuando automáticamente.");
            SiguienteDialogo();
        }
    }

    /// <summary>
    /// Se ejecuta cuando el usuario presiona el botón Continuar
    /// </summary>
    void AlPresionarContinuar()
    {
        if (!esperandoBotonContinuar)
            return;

        // Ocultar botón
        botonContinuar.gameObject.SetActive(false);
        esperandoBotonContinuar = false;

        // Pasar al siguiente diálogo
        SiguienteDialogo();
    }

    /// <summary>
    /// Avanza al siguiente diálogo
    /// </summary>
    void SiguienteDialogo()
    {
        indiceDialogoActual++;
        MostrarDialogoActual();
    }

    /// <summary>
    /// Se ejecuta cuando todos los diálogos han terminado
    /// </summary>
    void FinalizarDialogos()
    {
        // Desactivar panel de diálogo
        if (panelDialogo != null)
        {
            panelDialogo.SetActive(false);
        }

        // Detener audio si está reproduciendo
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    /// <summary>
    /// Método público para saltar al diálogo en un índice específico
    /// </summary>
    public void IrADialogo(int indice)
    {
        if (indice >= 0 && indice < dialogos.Count)
        {
            indiceDialogoActual = indice;
            MostrarDialogoActual();
        }
        else
        {
            Debug.LogWarning($"⚠️ Índice de diálogo fuera de rango: {indice}");
        }
    }

    /// <summary>
    /// Método público para reiniciar los diálogos
    /// </summary>
    public void ReiniciarDialogos()
    {
        // Detener audio y coroutines
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        if (coroutinaEscritura != null)
            StopCoroutine(coroutinaEscritura);

        // Ocultar botón
        if (botonContinuar != null)
            botonContinuar.gameObject.SetActive(false);

        // Reiniciar
        IniciarDialogos();
    }

    /// <summary>
    /// Método público para detener los diálogos inmediatamente
    /// </summary>
    public void DetenerDialogos()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        if (coroutinaEscritura != null)
            StopCoroutine(coroutinaEscritura);

        FinalizarDialogos();
    }

    void OnDestroy()
    {
        // Limpiar evento del botón
        if (botonContinuar != null)
        {
            botonContinuar.onClick.RemoveListener(AlPresionarContinuar);
        }
    }
}

/// <summary>
/// Clase que representa un diálogo individual
/// </summary>
[System.Serializable]
public class Dialogo
{
    [Tooltip("Texto completo del diálogo")]
    [TextArea(3, 10)]
    public string texto;

    [Tooltip("Audio que se reproduce con este diálogo")]
    public AudioClip audioClip;

    [Tooltip("¿Requiere que el usuario presione 'Continuar' para avanzar?")]
    public bool requiereBotonContinuar = false;

    [Tooltip("Ajuste individual de tiempo (en segundos) que se suma a la duración del audio SOLO para este diálogo. " +
             "Negativo = el texto termina ANTES que el audio (útil si el audio tiene silencio al final). " +
             "Positivo = el texto termina DESPUÉS. Se acumula con el ajuste global del sistema.")]
    [Range(-5f, 5f)]
    public float ajusteIndividualTiempo = 0f;
}