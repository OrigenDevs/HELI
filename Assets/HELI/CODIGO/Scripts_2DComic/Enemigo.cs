using UnityEngine;

public class Enemigo : MonoBehaviour
{
    [Header("Vida")]
    public float salud = 1f;

    [Header("Muerte")]
    public float fuerzaMuerte = 5f;
    public float duracionEmpuje = 0.15f;
    public AnimationCurve curvaEmpuje = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

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

    public void EventoMuerte()
    {
        MovimientoBEU jugador = FindFirstObjectByType<MovimientoBEU>();
        if (jugador == null) return;

        float dirX = Mathf.Sign(jugador.transform.position.x - transform.position.x);
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * dirX, transform.localScale.y, transform.localScale.z);

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.flipX = false;

        StartCoroutine(EmpujarMuerte(-dirX));
    }

    System.Collections.IEnumerator EmpujarMuerte(float direccion)
    {
        float t = 0f;
        float prev = 0f;
        while (t < duracionEmpuje)
        {
            t += Time.deltaTime;
            float factor = curvaEmpuje.Evaluate(t / duracionEmpuje);
            transform.Translate(new Vector3(direccion * fuerzaMuerte * (factor - prev), 0f, 0f));
            prev = factor;
            yield return null;
        }
    }

}
