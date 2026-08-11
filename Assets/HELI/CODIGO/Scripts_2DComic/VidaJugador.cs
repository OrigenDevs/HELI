using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VidaJugador : MonoBehaviour
{
    [Header("Vida")]
    public float vida = 3f;
    public float invulnerabilidad = 0.5f;

    [Header("Barra de vida")]
    public Slider barraVida;
    public float velocidadBarra = 3f;

    [Header("Derrota")]
    public GameObject panelDerrota;
    public float retrasoPanelDerrota = 0.5f;

    [Header("Particulas de daño")]
    public List<ParticleSystem> particulasDano;

    public System.Action onMuerte;
    public bool muerta;

    private float vidaMaxima;
    private float barraTarget;
    private float tiempoUltimoDaño;

    void Awake()
    {
        vidaMaxima = vida;
    }

    void Start()
    {
        if (barraVida != null)
        {
            barraVida.maxValue = vidaMaxima;
            barraVida.value = vida;
            barraTarget = vida;
        }
    }

    void Update()
    {
        if (barraVida != null)
            barraVida.value = Mathf.Lerp(barraVida.value, barraTarget, Time.deltaTime * velocidadBarra);
    }

    public void RecibirDano(float dano)
    {
        if (muerta || dano <= 0f) return;
        if (Time.time < tiempoUltimoDaño + invulnerabilidad) return;
        tiempoUltimoDaño = Time.time;

        vida -= dano;
        barraTarget = vida;
        ReproducirParticula(0);

        Animator animator = GetComponentInChildren<Animator>();
        if (animator != null)
            animator.SetTrigger("golpeado");

        if (vida <= 0f)
            Morir();
    }

    public void ReproducirParticula(int indice)
    {
        if (particulasDano == null || indice < 0 || indice >= particulasDano.Count) return;
        particulasDano[indice].Play();
    }

    public void Curar(float cantidad)
    {
        if (muerta || cantidad <= 0f) return;
        vida = Mathf.Min(vida + cantidad, vidaMaxima);
        barraTarget = vida;
    }

    void Morir()
    {
        muerta = true;
        if (onMuerte != null) onMuerte();

        MovimientoBEU m = GetComponent<MovimientoBEU>();
        if (m != null) m.controlBloqueado = true;

        GolpeJugador gj = GetComponent<GolpeJugador>();
        if (gj != null) gj.BloquearControles();

        foreach (EnemigoAtaque a in FindObjectsByType<EnemigoAtaque>(FindObjectsSortMode.None))
        {
            if (a != null) a.DetenerAtaque();
        }

        if (panelDerrota != null)
            StartCoroutine(MostrarPanelDerrota());
    }

    System.Collections.IEnumerator MostrarPanelDerrota()
    {
        yield return new WaitForSeconds(retrasoPanelDerrota);
        if (panelDerrota != null)
            panelDerrota.SetActive(true);
    }
}