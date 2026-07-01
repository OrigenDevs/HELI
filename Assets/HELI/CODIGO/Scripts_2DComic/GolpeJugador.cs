using UnityEngine;

[RequireComponent(typeof(MovimientoBEU))]
[RequireComponent(typeof(Animator))]
public class GolpeJugador : MonoBehaviour
{
    [Header("Golpe")]
    public float dano = 1f;
    public float duracionGolpe = 0.5f;
    public int golpesDisponibles = 3;
    public Vector2 tamanoZona = new Vector2(0.5f, 0.5f);
    public Vector2 offsetZona = new Vector2(1f, 0f);

    private MovimientoBEU movimiento;
    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D zonaGolpe;
    private bool golpeando;
    private int contadorGolpes;
    private Enemigo enemigoEnRango;
    private CamaraSigue camara;
    private static readonly int ParamGolpe = Animator.StringToHash("golpe");
    private static readonly int ParamVariante = Animator.StringToHash("golpeVariante");

    void Awake()
    {
        movimiento = GetComponent<MovimientoBEU>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        camara = FindFirstObjectByType<CamaraSigue>();

        GameObject zona = new GameObject("ZonaGolpe");
        zona.transform.SetParent(transform);
        zona.transform.localPosition = offsetZona;
        zona.layer = gameObject.layer;

        zonaGolpe = zona.AddComponent<BoxCollider2D>();
        ((BoxCollider2D)zonaGolpe).size = tamanoZona;
        zonaGolpe.isTrigger = true;
    }

    void Update()
    {
        if (golpeando) return;

        if (enemigoEnRango != null)
            IniciarGolpe();
    }

    void IniciarGolpe()
    {
        golpeando = true;
        movimiento.atacando = true;
        rb.linearVelocity = Vector2.zero;

        animator.SetInteger(ParamVariante, contadorGolpes);
        animator.SetTrigger(ParamGolpe);

        contadorGolpes = (contadorGolpes + 1) % golpesDisponibles;

        Invoke(nameof(AplicarGolpe), duracionGolpe * 0.5f);
        Invoke(nameof(FinGolpe), duracionGolpe);
    }

    public void EventoGolpe()
    {
        if (camara != null) camara.Sacudir();
    }

    public void AplicarGolpe()
    {
        if (enemigoEnRango != null)
            enemigoEnRango.RecibirDano(dano);
    }

    public void FinGolpe()
    {
        golpeando = false;
        movimiento.atacando = false;
        animator.ResetTrigger(ParamGolpe);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Enemigo enemigo = other.GetComponent<Enemigo>();
        if (enemigo != null)
            enemigoEnRango = enemigo;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Enemigo>() == enemigoEnRango)
            enemigoEnRango = null;
    }
}
