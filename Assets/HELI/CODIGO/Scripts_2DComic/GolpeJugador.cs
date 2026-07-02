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
    public ParticleSystem particulaGolpe;
    public AudioClip audioGolpe;

    [Header("Aproximacion")]
    public float distanciaAtaque = 0.3f;
    public float velocidadAproximacion = 8f;

    [Header("Super")]
    public int enemigosParaSuper = 3;
    public float danoSuper = 9999f;
    public ParticleSystem particulaSuper;
    public AudioClip audioSuper;
    public Collider2D zonaSuper;

    private MovimientoBEU movimiento;
    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D zonaGolpe;
    private bool golpeando;
    private bool golpeSuper;
    private int contadorGolpes;
    private int enemigosDerrotados;
    private bool superActivo;
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
        Enemigo.onCualquierDerrota += SumarDerrota;

        GameObject zona = new GameObject("ZonaGolpe");
        zona.transform.SetParent(transform);
        zona.transform.localPosition = offsetZona;
        zona.layer = gameObject.layer;
        zonaGolpe = zona.AddComponent<BoxCollider2D>();
        ((BoxCollider2D)zonaGolpe).size = tamanoZona;
        zonaGolpe.isTrigger = true;

        if (zonaSuper != null)
            zonaSuper.enabled = false;
    }

    void SumarDerrota()
    {
        if (golpeSuper) return;
        enemigosDerrotados++;
        if (enemigosDerrotados >= enemigosParaSuper)
        {
            superActivo = true;
            enemigosDerrotados = 0;
        }
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

        if (enemigoEnRango != null)
        {
            MirarAlEnemigo();
            StartCoroutine(AproximarseAlEnemigo());
        }

        if (superActivo)
        {
            superActivo = false;
            golpeSuper = true;

            if (zonaSuper != null)
                zonaSuper.enabled = true;

            if (particulaSuper != null) particulaSuper.Play();
            if (audioSuper != null && SoundManager.instancia != null)
                SoundManager.instancia.Reproducir(audioSuper);
        }

        animator.SetInteger(ParamVariante, contadorGolpes);
        animator.SetTrigger(ParamGolpe);

        contadorGolpes = (contadorGolpes + 1) % golpesDisponibles;

        Invoke(nameof(AplicarGolpe), duracionGolpe * 0.5f);
        Invoke(nameof(FinGolpe), duracionGolpe);
    }

    void MirarAlEnemigo()
    {
        float dirX = Mathf.Sign(enemigoEnRango.transform.position.x - transform.position.x);

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
            sr.flipX = dirX < 0f;

        float absX = Mathf.Abs(offsetZona.x);
        zonaGolpe.transform.localPosition = new Vector2(absX * dirX, offsetZona.y);
    }

    System.Collections.IEnumerator AproximarseAlEnemigo()
    {
        Vector2 objetivo = enemigoEnRango.transform.position;

        float dirX = Mathf.Sign(transform.position.x - objetivo.x);
        objetivo.x += dirX * distanciaAtaque;

        while (Vector2.Distance(transform.position, objetivo) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, objetivo, velocidadAproximacion * Time.deltaTime);
            yield return null;
        }
    }

    public void EventoGolpe()
    {
        if (camara != null) camara.Sacudir();
    }

    public void EventoParticula()
    {
        if (particulaGolpe == null) return;

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        float dirX = sr != null && sr.flipX ? -1f : 1f;

        Transform pt = particulaGolpe.transform;
        pt.localPosition = new Vector3(
            Mathf.Abs(pt.localPosition.x) * dirX,
            pt.localPosition.y,
            pt.localPosition.z
        );
        pt.localScale = new Vector3(
            Mathf.Abs(pt.localScale.x) * dirX,
            pt.localScale.y,
            pt.localScale.z
        );
        particulaGolpe.Play();
    }

    public void EventoAudio()
    {
        if (audioGolpe != null && SoundManager.instancia != null)
            SoundManager.instancia.Reproducir(audioGolpe);
    }

    public void AplicarGolpe()
    {
        if (golpeSuper)
        {
            Collider2D zona = zonaSuper;
            if (zona != null)
            {
                Vector2 origen = zona.transform.position;
                Vector2 tamano = ((BoxCollider2D)zona).size;
                Collider2D[] hits = Physics2D.OverlapBoxAll(origen, tamano, 0f);
                foreach (var hit in hits)
                {
                    Enemigo e = hit.GetComponent<Enemigo>();
                    if (e != null) e.RecibirDano(danoSuper);
                }
            }
            golpeSuper = false;
        }
        else if (enemigoEnRango != null)
        {
            enemigoEnRango.RecibirDano(dano);
        }
    }

    public void FinGolpe()
    {
        if (zonaSuper != null)
            zonaSuper.enabled = false;
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
