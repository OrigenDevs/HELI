using UnityEngine;

public class CamaraSigue : MonoBehaviour
{
    public Transform objetivo;
    public float suavizado = 0.15f;

    [Header("Temblor")]
    public float intensidadTemblor = 0.2f;
    public float duracionTemblor = 0.1f;

    private Vector3 velocidad;
    private float temblorTiempo;
    private float temblorIntensidad;

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
