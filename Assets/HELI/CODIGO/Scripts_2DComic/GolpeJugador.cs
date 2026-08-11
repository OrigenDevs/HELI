using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(MovimientoBEU))]
[RequireComponent(typeof(Animator))]
public class GolpeJugador : MonoBehaviour
{
    [Header("Controles")]
    public InputActionReference accionGolpe;
    public InputActionReference accionSuper;

    [Header("Golpe")]
    public float dano = 1f;
    public float duracionGolpe = 0.5f;
    public Collider2D zonaGolpe;
    public AudioClip audioGolpe;

    [Header("Combo")]
    public float tiempoVentanaCombo = 0.7f;
    public string paramGolpeSuave = "golpeSuave";
    public string paramGolpeFuerte = "golpeFuerte";

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
    public float tiempoUISuper = 1f;

    private MovimientoBEU movimiento;
    private Animator animator;
    private Rigidbody2D rb;
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
    private int comboGolpes;
    private float tiempoUltimoGolpe;
    private int golpesBuffer;
    private int enemigosDerrotados;
    private bool superActivo;
    private bool zonaGolpeActivada;
    private readonly HashSet<Enemigo> enemigosGolpeados = new HashSet<Enemigo>();
    private CamaraSigue camara;
    private Coroutine corutinaOcultarUI;

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

        if (zonaGolpe != null)
        {
            zonaGolpe.enabled = false;
            ZonaGolpe zg = zonaGolpe.GetComponent<ZonaGolpe>();
            if (zg == null)
                zg = zonaGolpe.gameObject.AddComponent<ZonaGolpe>();
            zg.jugador = this;
        }

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
        if (golpeSuper || superActivo) return;
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

        if (!golpeando && accionSuper != null && accionSuper.action.WasPressedThisFrame() && superActivo)
        {
            IniciarSuper();
            return;
        }

        if (accionGolpe != null && accionGolpe.action.WasPressedThisFrame())
        {
            if (golpeando)
            {
                if (golpesBuffer < 3) golpesBuffer++;
            }
            else
            {
                IniciarGolpe();
            }
        }
    }

    void OnEnable()
    {
        if (accionGolpe != null) accionGolpe.action.Enable();
        if (accionSuper != null) accionSuper.action.Enable();
    }

    void OnDisable()
    {
        if (accionGolpe != null) accionGolpe.action.Disable();
        if (accionSuper != null) accionSuper.action.Disable();
    }

    void IniciarGolpe()
    {
        golpeando = true;
        movimiento.atacando = true;
        rb.linearVelocity = Vector2.zero;

        zonaGolpeActivada = false;
        enemigosGolpeados.Clear();

        if (Time.time - tiempoUltimoGolpe <= tiempoVentanaCombo)
        {
            comboGolpes++;
            if (comboGolpes > 2) comboGolpes = 0;
        }
        else
        {
            comboGolpes = 0;
        }

        if (comboGolpes >= 2)
            animator.SetTrigger(paramGolpeFuerte);
        else
            animator.SetTrigger(paramGolpeSuave);

        Invoke(nameof(FinGolpe), duracionGolpe);
    }

    public void ActivarZonaGolpe()
    {
        if (zonaGolpe == null || zonaGolpeActivada) return;
        zonaGolpeActivada = true;
        enemigosGolpeados.Clear();
        zonaGolpe.enabled = true;
    }

    public void DesactivarZonaGolpe()
    {
        if (zonaGolpe != null)
            zonaGolpe.enabled = false;
    }

    public void HitboxGolpear(Enemigo enemigo)
    {
        if (enemigo == null || enemigo.muerto || enemigosGolpeados.Contains(enemigo)) return;
        enemigosGolpeados.Add(enemigo);
        enemigo.RecibirDano(dano);
    }

    void IniciarSuper()
    {
        golpeando = true;
        movimiento.atacando = true;
        rb.linearVelocity = Vector2.zero;

        if (corutinaOcultarUI != null)
        {
            StopCoroutine(corutinaOcultarUI);
            corutinaOcultarUI = null;
        }

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

    public void EventoGolpe()
    {
        if (camara != null) camara.Sacudir();
    }

    public void EventoParticula(int indice)
    {
        List<Enemigo> objetivo = new List<Enemigo>(enemigosGolpeados);

        if (objetivo.Count == 0 && zonaGolpe != null)
        {
            Vector2 origen = zonaGolpe.transform.position;
            Vector2 tamano = ((BoxCollider2D)zonaGolpe).size;
            foreach (Collider2D hit in Physics2D.OverlapBoxAll(origen, tamano, 0f))
            {
                Enemigo e = hit.GetComponent<Enemigo>();
                if (e != null) objetivo.Add(e);
            }
        }

        foreach (Enemigo e in objetivo)
            e.ReproducirParticula(indice);
    }

    public void EventoAudio()
    {
        if (audioGolpe != null && SoundManager.instancia != null)
            SoundManager.instancia.Reproducir(audioGolpe);
    }

    public void FinPreSuper()
    {
        if (uiSuper != null)
        {
            if (corutinaOcultarUI != null)
                StopCoroutine(corutinaOcultarUI);
            corutinaOcultarUI = StartCoroutine(OcultarUISuper());
        }

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

    System.Collections.IEnumerator OcultarUISuper()
    {
        yield return new WaitForSeconds(tiempoUISuper);
        if (uiSuper != null)
        {
            uiSuper.transform.localPosition = posicionUiSuperOriginal;
            uiSuper.transform.localScale = escalaUiSuperOriginal;
            uiSuper.SetActive(false);
        }
        corutinaOcultarUI = null;
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
        if (!golpeSuper) return;

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

    public void FinGolpe()
    {
        DesactivarZonaGolpe();
        zonaGolpeActivada = false;
        enemigosGolpeados.Clear();
        tiempoUltimoGolpe = Time.time;

        fuenteSuper.Stop();
        fuenteSuper.clip = null;

        if (zonaSuper != null)
            zonaSuper.enabled = false;

        if (uiSuper != null && corutinaOcultarUI == null)
        {
            uiSuper.transform.localPosition = posicionUiSuperOriginal;
            uiSuper.transform.localScale = escalaUiSuperOriginal;
        }

        if (SoundManager.instancia != null)
            SoundManager.instancia.RestaurarMusica();

        animator.ResetTrigger(paramGolpeSuave);
        animator.ResetTrigger(paramGolpeFuerte);
        animator.ResetTrigger(paramPreSuperAnim);
        animator.ResetTrigger(paramSuperAnim);
        golpeando = false;
        movimiento.atacando = false;

        StartCoroutine(RestaurarParticulasAlMorir());

        if (golpesBuffer > 0)
        {
            golpesBuffer--;
            IniciarGolpe();
        }
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
}
