using UnityEngine;

[RequireComponent(typeof(Enemigo))]
public class EnemigoAtaque : MonoBehaviour
{
    [Header("Ataque")]
    public float dano = 1f;
    public float distanciaAtaque = 0.8f;
    public float cooldownAtaque = 1.5f;
    public float tiempoRevision = 0.2f;
    public string paramAtacar = "atacar";

    [Header("ZonaAtaque")]
    public Collider2D zonaAtaque;

    [Header("Audio")]
    public AudioClip audioAtaque;

    private Enemigo enemigo;
    private Animator animator;
    private Transform jugador;
    private float tiempoUltimoAtaque;
    private bool atacando;

    void Awake()
    {
        enemigo = GetComponent<Enemigo>();
        animator = GetComponentInChildren<Animator>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Start()
    {
        MovimientoBEU movimiento = FindFirstObjectByType<MovimientoBEU>();
        if (movimiento != null) jugador = movimiento.transform;

        if (zonaAtaque != null)
        {
            zonaAtaque.enabled = false;
            ZonaAtaque za = zonaAtaque.GetComponent<ZonaAtaque>();
            if (za == null) za = zonaAtaque.gameObject.AddComponent<ZonaAtaque>();
            za.enemigo = this;
        }

        StartCoroutine(RevisarAtaque());
    }

    System.Collections.IEnumerator RevisarAtaque()
    {
        while (!enemigo.muerto)
        {
            if (!atacando && jugador != null &&
                Time.time >= tiempoUltimoAtaque + cooldownAtaque &&
                Vector2.Distance(transform.position, jugador.position) <= distanciaAtaque)
            {
                tiempoUltimoAtaque = Time.time;
                atacando = true;
                animator.SetTrigger(paramAtacar);
            }
            yield return new WaitForSeconds(tiempoRevision);
        }
    }

    public void ActivarZonaAtaque()
    {
        if (zonaAtaque != null) zonaAtaque.enabled = true;
    }

    public void DesactivarZonaAtaque()
    {
        if (zonaAtaque != null) zonaAtaque.enabled = false;
        atacando = false;
    }

    public void EventoParticula(int indice)
    {
        if (jugador == null) return;
        VidaJugador vj = jugador.GetComponent<VidaJugador>();
        if (vj != null) vj.ReproducirParticula(indice);
    }

    public void EventoAudio()
    {
        if (audioAtaque != null && SoundManager.instancia != null)
            SoundManager.instancia.Reproducir(audioAtaque);
    }

    public void GolpearJugador(VidaJugador jugador)
    {
        if (jugador != null) jugador.RecibirDano(dano);
    }
}