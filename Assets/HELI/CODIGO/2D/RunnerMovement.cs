using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(JumpController))]
public class RunnerMovement : MonoBehaviour
{
    [System.Serializable]
    public class Carril
    {
        [Tooltip("Altura (Y) de este carril")]
        public float altura;
        [Tooltip("Profundidad (Z) de este carril")]
        public float profundidad;
    }

    [Header("Movimiento adelante")]
    public float velocidad = 8f;

    [Tooltip("Eje de movimiento: X, Y o Z")]
    public enum EjeMovimiento { X, Y, Z }
    public EjeMovimiento eje = EjeMovimiento.Z;

    [Tooltip("Dirección: 1 = positivo, -1 = negativo")]
    public float direccion = 1f;

    [Header("Carriles")]
    public Carril[] carriles;

    [Tooltip("Índice del carril inicial")]
    public int carrilInicial = 0;

    [Tooltip("Duración de la transición entre carriles (segundos)")]
    public float duracionTransicion = 0.15f;

    [Header("Detección de suelo")]
    [SerializeField] private float distanciaAlSuelo = 1.1f;

    private Rigidbody rb;
    private Animator animator;
    private JumpController jumpController;
    private bool estaEnSuelo = false;
    private bool estaVivo = true;

    private int carrilActual;
    private bool transicionando = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        jumpController = GetComponent<JumpController>();

        rb.constraints = RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationY
                       | RigidbodyConstraints.FreezeRotationZ;

        carrilActual = Mathf.Clamp(carrilInicial, 0, carriles.Length - 1);
    }

    void Update()
    {
        if (!estaVivo) return;

        VerificarSuelo();
        ManejarAnimaciones();

        var kb = InputSystem.GetDevice<Keyboard>();
        if (kb != null)
        {
            if (kb.spaceKey.wasPressedThisFrame)
                IntentarSaltar();

            if (!transicionando && estaEnSuelo)
            {
                if (kb.aKey.wasPressedThisFrame || kb.leftArrowKey.wasPressedThisFrame)
                    CambiarCarril(-1);

                if (kb.dKey.wasPressedThisFrame || kb.rightArrowKey.wasPressedThisFrame)
                    CambiarCarril(1);
            }
        }
    }

    void FixedUpdate()
    {
        if (!estaVivo || transicionando) return;

        Vector3 v = Vector3.zero;
        float dir = Mathf.Sign(direccion);

        switch (eje)
        {
            case EjeMovimiento.X: v.x = velocidad * dir; break;
            case EjeMovimiento.Y: v.y = velocidad * dir; break;
            case EjeMovimiento.Z: v.z = velocidad * dir; break;
        }

        if (eje != EjeMovimiento.Y)
            v.y = rb.linearVelocity.y;

        rb.linearVelocity = v;
    }

    private void CambiarCarril(int direccionCarril)
    {
        int nuevoIndice = Mathf.Clamp(carrilActual + direccionCarril, 0, carriles.Length - 1);
        if (nuevoIndice == carrilActual || transicionando) return;
        carrilActual = nuevoIndice;
        StartCoroutine(Transicionar());
    }

    private System.Collections.IEnumerator Transicionar()
    {
        transicionando = true;

        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;

        Carril destino = carriles[carrilActual];
        Vector3 inicio = transform.position;
        float dir = Mathf.Sign(direccion);
        float tiempo = 0;

        while (tiempo < duracionTransicion)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionTransicion;
            float suavizado = t * t * (3f - 2f * t);

            transform.position = new Vector3(
                inicio.x + velocidad * dir * tiempo,
                Mathf.Lerp(inicio.y, destino.altura, suavizado),
                Mathf.Lerp(inicio.z, destino.profundidad, suavizado)
            );

            yield return null;
        }

        Vector3 pos = transform.position;
        pos.y = destino.altura;
        pos.z = destino.profundidad;
        transform.position = pos;

        rb.position = pos;
        rb.isKinematic = false;

        transicionando = false;
    }

    public void IntentarSaltar()
    {
        if (estaEnSuelo && estaVivo)
            jumpController.EjecutarSalto();
    }

    private void VerificarSuelo()
    {
        bool anterior = estaEnSuelo;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, distanciaAlSuelo))
            estaEnSuelo = hit.collider.CompareTag("suelo");
        else
            estaEnSuelo = false;

        if (!anterior && estaEnSuelo)
            jumpController.AlTocarSuelo();
    }

    private void ManejarAnimaciones()
    {
        animator.SetBool("volar", !estaEnSuelo);
        animator.SetBool("correr", estaEnSuelo);
    }

    public void Detener()
    {
        estaVivo = false;
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;
        animator.SetBool("correr", false);
        animator.SetBool("volar", false);
        StopAllCoroutines();
        transicionando = false;
    }

    public bool EstaEnSuelo() => estaEnSuelo;
    public bool EstaVivo() => estaVivo;
}
