using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProyectilBoss : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 6f;
    public float tiempoVida = 4f;

    [Header("Daño")]
    public float dano = 1f;

    [Header("Audio")]
    public AudioClip audioImpacto;

    private Rigidbody2D rb;
    private Vector2 direccion;
    private bool explotando;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    void OnEnable()
    {
        explotando = false;
        Invoke(nameof(Explotar), tiempoVida);
    }

    void OnDisable()
    {
        CancelInvoke(nameof(Explotar));
    }

    public void Disparar(Vector2 dir, float velocidad, float dano)
    {
        this.velocidad = velocidad;
        this.dano = dano;
        direccion = dir.normalized;
        rb.linearVelocity = direccion * velocidad;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (explotando) return;

        if (other.CompareTag("Player"))
        {
            VidaJugador jugador = other.GetComponent<VidaJugador>();
            if (jugador != null && !jugador.muerta)
            {
                explotando = true;
                jugador.RecibirDano(dano);
                if (audioImpacto != null && SoundManager.instancia != null)
                    SoundManager.instancia.Reproducir(audioImpacto);
                Explotar();
            }
        }
    }

    void Explotar()
    {
        if (gameObject != null)
            Destroy(gameObject);
    }
}
