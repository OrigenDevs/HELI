using UnityEngine;

public class CamaraSigue : MonoBehaviour
{
    public Transform objetivo;
    public float suavizado = 0.15f;

    private Vector3 velocidad;

    void LateUpdate()
    {
        if (objetivo == null) return;

        Vector3 destino = new Vector3(objetivo.position.x, objetivo.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, destino, ref velocidad, suavizado);
    }
}
