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

        if (salud <= 0f)
            Derrotar();
        else if (animator != null)
            animator.SetTrigger(ParamGolpeado);
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
