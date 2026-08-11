using UnityEngine;

public class JefeFinal : Enemigo
{
    [Header("Ataque especial: Proyectiles")]
    public GameObject proyectilPrefab;
    public Transform puntoDisparo;
    public float danoProyectil = 1f;
    public float velocidadProyectil = 6f;
    public float cooldownAtaque = 2f;
    public float distanciaAtaque = 6f;
    public float tiempoRevision = 0.2f;
    public string paramDisparo = "disparo";

    [Header("Audio")]
    public AudioClip audioDisparo;

    private Transform jugador;
    private VidaJugador vidaJugador;
    private float tiempoUltimoAtaque;
    private bool atacando;

    void Start()
    {
        base.Start();

        MovimientoBEU m = FindFirstObjectByType<MovimientoBEU>();
        if (m != null)
        {
            jugador = m.transform;
            vidaJugador = m.GetComponent<VidaJugador>();
        }

        if (puntoDisparo == null)
            puntoDisparo = transform;

        StartCoroutine(RevisarAtaque());
    }

    System.Collections.IEnumerator RevisarAtaque()
    {
        while (!muerto)
        {
            if (!atacando && jugador != null &&
                vidaJugador != null && !vidaJugador.muerta &&
                Time.time >= tiempoUltimoAtaque + cooldownAtaque &&
                Vector2.Distance(transform.position, jugador.position) <= distanciaAtaque)
            {
                tiempoUltimoAtaque = Time.time;
                atacando = true;

                Animator anim = animator != null ? animator : GetComponentInChildren<Animator>();
                if (anim != null)
                    anim.SetTrigger(paramDisparo);
                else
                    Disparar();
            }
            yield return new WaitForSeconds(tiempoRevision);
        }
    }

    public void Disparar()
    {
        if (muerto || proyectilPrefab == null) return;

        Vector2 dir = Vector2.left;
        if (jugador != null)
            dir = (jugador.position - puntoDisparo.position).normalized;

        GameObject obj = Instantiate(proyectilPrefab, puntoDisparo.position, Quaternion.identity);
        ProyectilBoss proyectil = obj.GetComponent<ProyectilBoss>();
        if (proyectil == null)
            proyectil = obj.AddComponent<ProyectilBoss>();
        proyectil.Disparar(dir, velocidadProyectil, danoProyectil);

        if (audioDisparo != null && SoundManager.instancia != null)
            SoundManager.instancia.Reproducir(audioDisparo);
    }

    public void FinAtaque()
    {
        atacando = false;
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
