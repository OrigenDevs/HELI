using UnityEngine;
using UnityEngine.UI;

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
    public Collider2D zonaSuper;
    public ParticleSystem particulaFlash;
    public ParticleSystem particulaSuper;
    public GameObject uiSuper;
    public Slider sliderSuper;
    public float sliderVelocidad = 3f;
    public float duracionCongelamiento = 0.3f;
    public string paramPreSuperAnim = "preSuper";
    public string paramSuperAnim = "super";
    public AudioClip audioPreSuper;
    public AudioClip audioSuper;
    public float factorMusicaSuper = 0.2f;

    private MovimientoBEU movimiento;
    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D zonaGolpe;
    private AudioSource fuenteSuper;
    private Vector3 posicionSuperOriginal;
    private Vector3 posicionFlashOriginal;
    private Vector3 escalaFlashOriginal;
    private float flipXFlashOriginal;
    private Vector3 posicionUiSuperOriginal;
    private Vector3 escalaUiSuperOriginal;
    private Vector3 posicionParticulaSuperOriginal;
    private Vector3 escalaParticulaSuperOriginal;
    private float flipXParticulaSuperOriginal;
    private bool golpeando;
    private bool golpeSuper;
    private float dirXCongelado;
    private float sliderTarget;
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
        {
            zonaSuper.enabled = false;
            posicionSuperOriginal = zonaSuper.transform.localPosition;
        }

        fuenteSuper = gameObject.AddComponent<AudioSource>();
        fuenteSuper.playOnAwake = false;

        if (particulaFlash != null)
        {
            posicionFlashOriginal = particulaFlash.transform.localPosition;
            escalaFlashOriginal = particulaFlash.transform.localScale;
            flipXFlashOriginal = particulaFlash.GetComponent<ParticleSystemRenderer>().flip.x;
        }
        if (particulaSuper != null)
        {
            posicionParticulaSuperOriginal = particulaSuper.transform.localPosition;
            escalaParticulaSuperOriginal = particulaSuper.transform.localScale;
            flipXParticulaSuperOriginal = particulaSuper.GetComponent<ParticleSystemRenderer>().flip.x;
        }
        if (uiSuper != null)
        {
            posicionUiSuperOriginal = uiSuper.transform.localPosition;
            escalaUiSuperOriginal = uiSuper.transform.localScale;
        }

        if (sliderSuper != null)
        {
            sliderSuper.value = 0f;
            sliderTarget = 0f;
        }
    }

    void SumarDerrota()
    {
        if (golpeSuper) return;
        enemigosDerrotados++;
        sliderTarget = (float)enemigosDerrotados / enemigosParaSuper;
        if (enemigosDerrotados >= enemigosParaSuper)
        {
            superActivo = true;
            enemigosDerrotados = 0;
        }
    }

    void Update()
    {
        if (sliderSuper != null)
            sliderSuper.value = Mathf.Lerp(sliderSuper.value, sliderTarget, Time.deltaTime * sliderVelocidad);

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
            if (sliderSuper != null) sliderTarget = 0f;

            foreach (Enemigo e in FindObjectsByType<Enemigo>(FindObjectsSortMode.None))
            {
                Animator a = e.GetComponentInChildren<Animator>();
                if (a != null) a.speed = 0f;
            }

            float dirX = ObtenerDireccionFlipeada();
            dirXCongelado = dirX;

            if (zonaSuper != null)
                zonaSuper.transform.localPosition = new Vector2(
                    Mathf.Abs(posicionSuperOriginal.x) * dirX,
                    posicionSuperOriginal.y
                );

            if (particulaFlash != null)
            {
                Transform pt = particulaFlash.transform;
                pt.localPosition = new Vector2(
                    Mathf.Abs(posicionFlashOriginal.x) * dirX,
                    posicionFlashOriginal.y
                );
                pt.localScale = new Vector3(
                    Mathf.Abs(escalaFlashOriginal.x) * dirX,
                    escalaFlashOriginal.y,
                    escalaFlashOriginal.z
                );
                SetFlipXRecursivo(particulaFlash, dirX > 0f ? flipXFlashOriginal : 0f);
                particulaFlash.Play();
            }

            if (audioPreSuper != null && SoundManager.instancia != null)
                SoundManager.instancia.Reproducir(audioPreSuper);

            if (uiSuper != null)
            {
                Transform ut = uiSuper.transform;
                ut.localPosition = new Vector2(
                    Mathf.Abs(posicionUiSuperOriginal.x) * -dirX,
                    posicionUiSuperOriginal.y
                );
                ut.localScale = new Vector3(
                    Mathf.Abs(ut.localScale.x) * dirX,
                    ut.localScale.y,
                    ut.localScale.z
                );
                uiSuper.SetActive(true);
            }

            if (SoundManager.instancia != null)
                SoundManager.instancia.BajarMusica(factorMusicaSuper);

            animator.SetTrigger(paramPreSuperAnim);
            StartCoroutine(DescongelarEnemigos());
            return;
        }

        animator.SetInteger(ParamVariante, contadorGolpes);
        animator.SetTrigger(ParamGolpe);

        contadorGolpes = (contadorGolpes + 1) % golpesDisponibles;

        Invoke(nameof(AplicarGolpe), duracionGolpe * 0.5f);
        Invoke(nameof(FinGolpe), duracionGolpe);
    }

    float ObtenerDireccionFlipeada()
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        return sr != null && sr.flipX ? -1f : 1f;
    }

    System.Collections.IEnumerator DescongelarEnemigos()
    {
        yield return new WaitForSecondsRealtime(duracionCongelamiento);
        foreach (Enemigo e in FindObjectsByType<Enemigo>(FindObjectsSortMode.None))
        {
            Animator a = e.GetComponentInChildren<Animator>();
            if (a != null) a.speed = 1f;
        }
    }

    void SetFlipXRecursivo(ParticleSystem ps, float flipX)
    {
        foreach (ParticleSystemRenderer r in ps.GetComponentsInChildren<ParticleSystemRenderer>())
            r.flip = new Vector3(flipX, r.flip.y, r.flip.z);
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

        float dirX = ObtenerDireccionFlipeada();
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

    public void FinPreSuper()
    {
        if (uiSuper != null)
            uiSuper.SetActive(false);

        if (audioSuper != null)
        {
            fuenteSuper.clip = audioSuper;
            fuenteSuper.Play();
        }

        if (particulaSuper != null)
        {
            Transform pt = particulaSuper.transform;
            pt.localPosition = new Vector3(
                Mathf.Abs(posicionParticulaSuperOriginal.x) * dirXCongelado,
                posicionParticulaSuperOriginal.y,
                posicionParticulaSuperOriginal.z
            );
            pt.localScale = new Vector3(
                Mathf.Abs(escalaParticulaSuperOriginal.x) * dirXCongelado,
                escalaParticulaSuperOriginal.y,
                escalaParticulaSuperOriginal.z
            );
            SetFlipXRecursivo(particulaSuper, dirXCongelado > 0f ? flipXParticulaSuperOriginal : 0f);
        }

        animator.SetTrigger(paramSuperAnim);
    }

    public void EventoSuperGolpe()
    {
        if (zonaSuper != null)
            zonaSuper.enabled = true;
        if (particulaSuper != null)
            particulaSuper.Play();
        AplicarGolpe();
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
        fuenteSuper.Stop();
        fuenteSuper.clip = null;

        if (zonaSuper != null)
            zonaSuper.enabled = false;

        if (uiSuper != null)
        {
            uiSuper.transform.localPosition = posicionUiSuperOriginal;
            uiSuper.transform.localScale = escalaUiSuperOriginal;
        }

        if (SoundManager.instancia != null)
            SoundManager.instancia.RestaurarMusica();

        animator.ResetTrigger(ParamGolpe);
        animator.ResetTrigger(paramPreSuperAnim);
        animator.ResetTrigger(paramSuperAnim);
        golpeando = false;
        movimiento.atacando = false;

        StartCoroutine(RestaurarParticulasAlMorir());
    }

    System.Collections.IEnumerator RestaurarParticulasAlMorir()
    {
        if (particulaFlash != null)
            while (particulaFlash.IsAlive()) yield return null;
        if (particulaSuper != null)
            while (particulaSuper.IsAlive()) yield return null;

        if (zonaSuper != null)
            zonaSuper.transform.localPosition = posicionSuperOriginal;
        if (particulaFlash != null)
        {
            particulaFlash.transform.localPosition = posicionFlashOriginal;
            particulaFlash.transform.localScale = escalaFlashOriginal;
            SetFlipXRecursivo(particulaFlash, flipXFlashOriginal);
        }
        if (particulaSuper != null)
        {
            particulaSuper.transform.localPosition = posicionParticulaSuperOriginal;
            particulaSuper.transform.localScale = escalaParticulaSuperOriginal;
            SetFlipXRecursivo(particulaSuper, flipXParticulaSuperOriginal);
        }
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
