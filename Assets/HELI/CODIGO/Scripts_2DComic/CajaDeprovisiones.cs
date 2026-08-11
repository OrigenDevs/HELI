using System.Collections;
using UnityEngine;

public class CajaDeprovisiones : MonoBehaviour
{
    [Header("Golpes")]
    [Tooltip("Golpes necesarios para activar el boleano y pasar a la versión dañada.")]
    public int golpesParaDanada = 2;
    [Tooltip("Golpes totales necesarios para destrozarla.")]
    public int golpesParaDestruir = 4;

    [Header("Daño")]
    [Tooltip("Se activa cuando la caja pasa a su versión dañada.")]
    public bool danada;
    public float fuerzaEmpujeX = 2f;
    public float duracionEmpuje = 0.15f;
    public AnimationCurve curvaEmpuje = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Apariencia")]
    public SpriteRenderer spriteRenderer;
    public Sprite spriteNormal;
    public Sprite spriteDanada;
    public Sprite spriteDestruida;
    public float duracionFade = 0.5f;

    [Header("Drop")]
    public GameObject prefabItemVida;

    [Header("Evento")]
    public UnityEngine.Events.UnityEvent onDestruida;

    private int golpes;
    private bool destruida;
    private Collider2D col;
    private Rigidbody2D rb;

    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (spriteRenderer != null && spriteNormal != null)
            spriteRenderer.sprite = spriteNormal;
    }

    public void RecibirGolpe(float dano)
    {
        if (destruida) return;
        golpes++;
        EmpujarEnX();

        if (golpes >= golpesParaDestruir)
        {
            Destruir();
            return;
        }

        if (!danada && golpes >= golpesParaDanada)
        {
            danada = true;
            if (spriteRenderer != null && spriteDanada != null)
                spriteRenderer.sprite = spriteDanada;
        }
    }

    void EmpujarEnX()
    {
        float dir = 1f;
        MovimientoBEU jugador = FindFirstObjectByType<MovimientoBEU>();
        if (jugador != null)
            dir = Mathf.Sign(transform.position.x - jugador.transform.position.x);
        if (dir == 0f) dir = 1f;

        if (rb != null && !rb.isKinematic)
        {
            rb.linearVelocity = new Vector2(dir * fuerzaEmpujeX, rb.linearVelocity.y);
        }
        else
        {
            StartCoroutine(DesplazarX(dir));
        }
    }

    System.Collections.IEnumerator DesplazarX(float direccion)
    {
        float t = 0f;
        float prev = 0f;
        while (t < duracionEmpuje)
        {
            t += Time.deltaTime;
            float factor = curvaEmpuje.Evaluate(t / duracionEmpuje);
            transform.Translate(new Vector3(direccion * fuerzaEmpujeX * (factor - prev), 0f, 0f));
            prev = factor;
            yield return null;
        }
    }

    void Destruir()
    {
        destruida = true;
        if (spriteRenderer != null && spriteDestruida != null)
            spriteRenderer.sprite = spriteDestruida;

        if (col != null)
            col.enabled = false;

        if (onDestruida != null)
            onDestruida.Invoke();

        if (prefabItemVida != null)
            Instantiate(prefabItemVida, transform.position, Quaternion.identity);

        if (spriteRenderer != null)
            StartCoroutine(FadeYDestruir());
        else
            Destroy(gameObject);
    }

    System.Collections.IEnumerator FadeYDestruir()
    {
        float t = 0f;
        Color color = spriteRenderer.color;
        while (t < duracionFade)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, t / duracionFade);
            spriteRenderer.color = color;
            yield return null;
        }
        Destroy(gameObject);
    }
}
