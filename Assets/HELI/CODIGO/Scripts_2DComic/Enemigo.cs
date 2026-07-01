using UnityEngine;

public class Enemigo : MonoBehaviour
{
    [Header("Vida")]
    public float salud = 1f;

    [Header("Animaciones")]
    public Animator animator;

    private Collider2D col;
    private static readonly int ParamGolpeado = Animator.StringToHash("golpeado");
    private static readonly int ParamDerrotado = Animator.StringToHash("derrotado");

    void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        col = GetComponent<Collider2D>();
    }

    void Start()
    {
        if (animator == null)
            Debug.LogWarning("Enemigo sin Animator. Arrástralo en el campo animator.", this);
    }

    public void RecibirDano(float dano)
    {
        salud -= dano;

        MirarAlJugador();

        if (salud <= 0f)
            Derrotar();
        else if (animator != null)
            animator.SetTrigger(ParamGolpeado);
    }

    void MirarAlJugador()
    {
        MovimientoBEU jugador = FindFirstObjectByType<MovimientoBEU>();
        if (jugador == null) return;

        float dirX = Mathf.Sign(jugador.transform.position.x - transform.position.x);
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
            sr.flipX = dirX < 0f;
    }

    void Derrotar()
    {
        if (animator != null)
            animator.SetTrigger(ParamDerrotado);
        if (col != null)
            col.enabled = false;
        Destroy(gameObject, 1f);
    }
}
