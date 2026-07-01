using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Enemigo : MonoBehaviour
{
    [Header("Vida")]
    public float salud = 1f;

    private Animator animator;
    private Collider2D col;
    private static readonly int ParamGolpeado = Animator.StringToHash("golpeado");
    private static readonly int ParamDerrotado = Animator.StringToHash("derrotado");

    void Awake()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    public void RecibirDano(float dano)
    {
        salud -= dano;

        if (salud <= 0f)
            Derrotar();
        else
            animator.SetTrigger(ParamGolpeado);
    }

    void Derrotar()
    {
        animator.SetTrigger(ParamDerrotado);
        col.enabled = false;
        Destroy(gameObject, 1f);
    }
}
