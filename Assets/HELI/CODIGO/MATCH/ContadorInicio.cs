using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ContadorInicio : MonoBehaviour
{
    [Header("Textos")]
    public GameObject textoReady;
    public GameObject textoGo;

    [Header("Frases")]
    public float duracionFrase = 1f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip audioReady;
    public AudioClip audioGo;

    [Header("Referencias")]
    public MatchCards matchCards;
    public MatchNavegacion navegacion;

    public UnityEvent onInicio;

    private bool iniciado = false;

    void Awake()
    {
        OcultarTextos();
    }

    public void Iniciar()
    {
        if (iniciado) return;
        iniciado = true;
        StartCoroutine(Secuencia());
    }

    IEnumerator Secuencia()
    {
        yield return MostrarFrase(textoReady, audioReady);
        yield return MostrarFrase(textoGo, audioGo);

        OcultarTextos();

        if (navegacion != null) navegacion.enabled = true;

        if (matchCards != null && matchCards.jugadores > 1)
            yield return matchCards.AnimarTransicionTurno();

        onInicio.Invoke();
        iniciado = false;
    }

    IEnumerator MostrarFrase(GameObject texto, AudioClip clip)
    {
        if (texto != null) texto.SetActive(true);
        if (audioSource != null && clip != null) audioSource.PlayOneShot(clip);
        yield return new WaitForSeconds(duracionFrase);
        if (texto != null) texto.SetActive(false);
    }

    void OcultarTextos()
    {
        if (textoReady != null) textoReady.SetActive(false);
        if (textoGo != null) textoGo.SetActive(false);
    }
}
