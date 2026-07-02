using UnityEngine;

public class Teletransporte : MonoBehaviour
{
    public Transform[] ruta;
    public bool instantaneo = true;
    public float velocidadTraslado = 5f;
    public float zoomCamara;

    private Coroutine trasladoActivo;
    private Coroutine restauracionZoom;
    private CamaraSigue camara;
    private float zoomOriginal;

    private static readonly int ParamVelocidad = Animator.StringToHash("velocidad");

    void Start()
    {
        camara = FindFirstObjectByType<CamaraSigue>();
        if (camara != null)
            zoomOriginal = camara.transform.position.z;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        MovimientoBEU jugador = other.GetComponent<MovimientoBEU>();
        if (jugador == null || ruta == null || ruta.Length == 0) return;

        if (trasladoActivo != null)
            StopCoroutine(trasladoActivo);
        if (restauracionZoom != null)
            StopCoroutine(restauracionZoom);

        if (instantaneo)
        {
            other.transform.position = ruta[ruta.Length - 1].position;
            RestaurarZoomSuave();
        }
        else
        {
            jugador.controlBloqueado = true;
            Rigidbody2D rb = other.attachedRigidbody;
            if (rb != null) rb.linearVelocity = Vector2.zero;
            trasladoActivo = StartCoroutine(Trasladar(jugador, other.transform));
        }
    }

    void RestaurarZoomSuave()
    {
        if (camara != null)
            restauracionZoom = StartCoroutine(SuavizarZoom(zoomOriginal));
    }

    System.Collections.IEnumerator SuavizarZoom(float destinoZ)
    {
        Transform ct = camara.transform;
        while (Mathf.Abs(ct.position.z - destinoZ) > 0.01f)
        {
            Vector3 pos = ct.position;
            ct.position = new Vector3(pos.x, pos.y, Mathf.Lerp(pos.z, destinoZ, Time.deltaTime * 5f));
            yield return null;
        }
        Vector3 p = ct.position;
        ct.position = new Vector3(p.x, p.y, destinoZ);
        restauracionZoom = null;
    }

    System.Collections.IEnumerator Trasladar(MovimientoBEU jugador, Transform jugadorTransform)
    {
        Animator animator = jugadorTransform.GetComponent<Animator>();
        SpriteRenderer sr = jugadorTransform.GetComponentInChildren<SpriteRenderer>();
        Transform camTransform = camara != null ? camara.transform : null;
        bool usarZoom = camTransform != null && zoomCamara != 0f;

        for (int i = 0; i < ruta.Length; i++)
        {
            Transform punto = ruta[i];
            if (punto == null) continue;

            while (Vector2.Distance(jugadorTransform.position, punto.position) > 0.05f)
            {
                float dirX = Mathf.Sign(punto.position.x - jugadorTransform.position.x);
                jugadorTransform.position = Vector2.MoveTowards(jugadorTransform.position, punto.position, velocidadTraslado * Time.deltaTime);

                if (sr != null)
                    sr.flipX = dirX < 0f;

                if (animator != null)
                    animator.SetFloat(ParamVelocidad, velocidadTraslado);

                if (usarZoom)
                {
                    Vector3 pos = camTransform.position;
                    camTransform.position = new Vector3(pos.x, pos.y, Mathf.Lerp(pos.z, zoomCamara, Time.deltaTime * 5f));
                }

                yield return null;
            }

            jugadorTransform.position = punto.position;
        }

        if (animator != null)
            animator.SetFloat(ParamVelocidad, 0f);

        RestaurarZoomSuave();

        jugador.controlBloqueado = false;
        trasladoActivo = null;
    }
}
