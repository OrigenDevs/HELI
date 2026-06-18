using UnityEngine;

/// <summary>
/// Salto cartoon: control independiente de subida, bajada y altura máxima.
/// Requiere Rigidbody en el mismo GameObject.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class JumpController : MonoBehaviour
{
    [Header("Parámetros del salto")]
    [Tooltip("Fuerza inicial del salto")]
    public float fuerzaSalto = 12f;

    [Tooltip("Altura máxima que puede alcanzar el personaje")]
    public float alturaMaxima = 4f;

    [Tooltip("Multiplicador de gravedad durante la SUBIDA (menor = sube más lento/flotante)")]
    public float multiplicadorSubida = 1f;

    [Tooltip("Multiplicador de gravedad durante la BAJADA (mayor = cae más rápido/pesado)")]
    public float multiplicadorBajada = 3f;

    [Tooltip("Velocidad máxima de caída")]
    public float velocidadMaximaCaida = 20f;

    private Rigidbody rb;
    private bool saltando = false;
    private float posYInicial;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float vy = rb.linearVelocity.y;

        if (saltando && vy > 0)
        {
            // Subida: gravedad personalizada (solo durante salto activo)
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (multiplicadorSubida - 1) * Time.fixedDeltaTime;

            // Limitar altura máxima
            if (transform.position.y >= posYInicial + alturaMaxima)
            {
                Vector3 v = rb.linearVelocity;
                v.y = 0f;
                rb.linearVelocity = v;
            }
        }
        else if (vy < 0)
        {
            // Bajada: gravedad extra siempre que esté cayendo,
            // ya sea por salto o por caer del borde de una plataforma
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (multiplicadorBajada - 1) * Time.fixedDeltaTime;

            // Limitar velocidad de caída
            Vector3 v = rb.linearVelocity;
            v.y = Mathf.Max(v.y, -velocidadMaximaCaida);
            rb.linearVelocity = v;
        }
    }

    public void EjecutarSalto()
    {
        posYInicial = transform.position.y;
        saltando = true;

        // Resetear velocidad vertical antes de saltar (saltos consistentes)
        Vector3 v = rb.linearVelocity;
        v.y = 0f;
        rb.linearVelocity = v;

        rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
    }

    public void AlTocarSuelo()
    {
        saltando = false;

        // Asegurar que no quede velocidad vertical residual
        Vector3 v = rb.linearVelocity;
        v.y = 0f;
        rb.linearVelocity = v;
    }

    public bool EstaSaltando() => saltando;
}