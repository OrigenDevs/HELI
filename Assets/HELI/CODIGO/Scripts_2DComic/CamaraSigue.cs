using UnityEngine;

public class CamaraSigue : MonoBehaviour
{
    public Transform objetivo;
    public float suavizado = 0.15f;

    [Header("Temblor")]
    public float intensidadTemblor = 0.2f;
    public float duracionTemblor = 0.1f;

    [Header("Zoom por velocidad")]
    public float distanciaZoom = 0f;
    public float velocidadZoomIda = 5f;
    public float velocidadZoomVuelta = 5f;

    private Vector3 velocidad;
    private float temblorTiempo;
    private float temblorIntensidad;
    private float zoomBase;
    private Vector3 ultimaPosicion;

    void Start()
    {
        zoomBase = transform.position.z;
        if (objetivo != null)
            ultimaPosicion = objetivo.position;
    }

    void LateUpdate()
    {
        if (objetivo == null) return;

        Vector3 destino = new Vector3(objetivo.position.x, objetivo.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, destino, ref velocidad, suavizado);

        if (temblorTiempo > 0f)
        {
            temblorTiempo -= Time.deltaTime;
            Vector3 offset = Random.insideUnitCircle * temblorIntensidad;
            transform.position += new Vector3(offset.x, offset.y, 0f);
        }

        if (distanciaZoom != 0f && objetivo != null)
        {
            float rapidez = (objetivo.position - ultimaPosicion).magnitude / Time.deltaTime;
            ultimaPosicion = objetivo.position;
            float factor = Mathf.Clamp01(rapidez / 5f);
            float zMeta = zoomBase + distanciaZoom * factor;

            float distActual = Mathf.Abs(transform.position.z - zoomBase);
            float distMeta = Mathf.Abs(zMeta - zoomBase);
            float vel = distMeta > distActual ? velocidadZoomIda : velocidadZoomVuelta;

            Vector3 pos = transform.position;
            pos.z = Mathf.Lerp(pos.z, zMeta, Time.deltaTime * vel);
            transform.position = pos;

            velocidad.z = 0f;
        }
    }

    public void Sacudir()
    {
        temblorTiempo = duracionTemblor;
        temblorIntensidad = intensidadTemblor;
    }

    public void Sacudir(float intensidad, float duracion)
    {
        temblorTiempo = duracion;
        temblorIntensidad = intensidad;
    }
}
