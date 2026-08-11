using UnityEngine;

public class ItemPoder : MonoBehaviour
{
    [Header("Recoger")]
    public AudioClip sonidoRecoger;
    public float duracionEncoger = 0.2f;

    [Header("Opcional")]
    public float tiempoVida = 15f;

    private Collider2D col;
    private bool recogido;

    void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    void Start()
    {
        if (tiempoVida > 0f)
            Destroy(gameObject, tiempoVida);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (recogido) return;

        GolpeJugador gj = other.GetComponent<GolpeJugador>();
        if (gj == null) gj = other.GetComponentInParent<GolpeJugador>();
        if (gj == null) return;

        recogido = true;
        gj.CargarSuperAlMaximo();

        if (col != null) col.enabled = false;
        if (sonidoRecoger != null && SoundManager.instancia != null)
            SoundManager.instancia.Reproducir(sonidoRecoger);

        StartCoroutine(EncogerYDestruir());
    }

    System.Collections.IEnumerator EncogerYDestruir()
    {
        Vector3 original = transform.localScale;
        float t = 0f;
        while (t < duracionEncoger)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(original, Vector3.zero, t / duracionEncoger);
            yield return null;
        }
        Destroy(gameObject);
    }
}
