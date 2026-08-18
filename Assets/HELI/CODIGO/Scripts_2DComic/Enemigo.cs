using System.Collections.Generic;
using UnityEngine;

public class Enemigo : MonoBehaviour
{
    [Header("Vida")]
    public float salud = 1f;

    [Header("Particulas")]
    public List<ParticleSystem> particulasGolpe;

    [Header("Muerte")]
    public float fuerzaMuerte = 5f;
    public float duracionEmpuje = 0.15f;
    public AnimationCurve curvaEmpuje = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public GameObject objetoAEliminar;

    [Header("Animaciones")]
    public Animator animator;
    public float tiempoCambioIdle = 2f;

    [Header("Mirar al jugador")]
    public float tiempoMirarJugador = 0.15f;

    private Collider2D col;
    private Transform jugador;
    private static readonly int ParamGolpeado = Animator.StringToHash("golpeado");
    private static readonly int ParamDerrotado = Animator.StringToHash("derrotado");
    private static readonly int ParamIdleVariante = Animator.StringToHash("idleVariante");
    private bool tieneIdleVariante;

    public System.Action onDerrotado;
    public static System.Action onCualquierDerrota;
    public bool muerto;

    void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            for (int i = 0; i < animator.parameterCount; i++)
            {
                if (animator.GetParameter(i).nameHash == ParamIdleVariante)
                {
                    tieneIdleVariante = true;
                    break;
                }
            }
        }
        col = GetComponent<Collider2D>();
    }

    protected void Start()
    {
        if (animator == null)
            Debug.LogWarning("Enemigo sin Animator. Arrástralo en el campo animator.", this);
        else
            StartCoroutine(AlternarIdle());

        MovimientoBEU m = FindFirstObjectByType<MovimientoBEU>();
        if (m != null) jugador = m.transform;
        StartCoroutine(MirarJugador());
    }

    System.Collections.IEnumerator MirarJugador()
    {
        while (!muerto)
        {
            yield return new WaitForSeconds(tiempoMirarJugador);
            if (jugador == null || muerto) continue;

            float dirX = Mathf.Sign(jugador.position.x - transform.position.x);
            SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
                sr.flipX = dirX < 0f;
        }
    }

    System.Collections.IEnumerator AlternarIdle()
    {
        if (!tieneIdleVariante) yield break;
        while (!muerto)
        {
            yield return new WaitForSeconds(Random.Range(tiempoCambioIdle * 0.5f, tiempoCambioIdle * 1.5f));
            if (animator != null && !muerto)
                animator.SetInteger(ParamIdleVariante, Random.Range(0, 2));
        }
    }

    public void RecibirDano(float dano)
    {
        if (muerto) return;
        salud -= dano;

        MirarAlJugador();

        ReproducirParticula(0);

        if (salud <= 0f)
            Derrotar();
        else if (animator != null)
            animator.SetTrigger(ParamGolpeado);
    }

    public void ReproducirParticula(int indice)
    {
        if (particulasGolpe == null || indice < 0 || indice >= particulasGolpe.Count) return;
        particulasGolpe[indice].Play();
    }

    void MirarAlJugador()
    {
        if (jugador == null)
        {
            MovimientoBEU m = FindFirstObjectByType<MovimientoBEU>();
            if (m != null) jugador = m.transform;
        }
        if (jugador == null) return;

        float dirX = Mathf.Sign(jugador.position.x - transform.position.x);
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
            sr.flipX = dirX < 0f;
    }

    void Derrotar()
    {
        if (muerto) return;
        muerto = true;
        if (onDerrotado != null)
            onDerrotado();
        if (onCualquierDerrota != null)
            onCualquierDerrota();

        if (animator != null)
        {
            animator.ResetTrigger(ParamGolpeado);
            animator.ResetTrigger(ParamDerrotado);
            ReproducirParticula(1);
            animator.SetTrigger(ParamDerrotado);
        }
        if (col != null)
            col.enabled = false;
        Destroy(objetoAEliminar != null ? objetoAEliminar : gameObject, 1f);
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
