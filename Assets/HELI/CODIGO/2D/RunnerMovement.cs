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
        if (!estaVivo) return;

        float dir = Mathf.Sign(direccion);
        Vector3 v = rb.linearVelocity;

        switch (eje)
        {
            case EjeMovimiento.X: v.x = velocidad * dir; break;
            case EjeMovimiento.Y: v.y = velocidad * dir; break;
            case EjeMovimiento.Z: v.z = velocidad * dir; break;
        }

        rb.linearVelocity = v;
    }

    private void CambiarCarril(int direccionCarril)
    {
        int nuevoIndice = Mathf.Clamp(carrilActual + direccionCarril, 0, carriles.Length - 1);
        if (nuevoIndice == carrilActual) return;
        carrilActual = nuevoIndice;
        StartCoroutine(TransicionarACarril());
    }

    private System.Collections.IEnumerator TransicionarACarril()
    {
        transicionando = true;
        Carril destino = carriles[carrilActual];

        float inicioY = rb.position.y;
        float inicioZ = rb.position.z;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / duracionTransicion;
            float suavizado = t * t * (3f - 2f * t);

            Vector3 pos = rb.position;
            pos.y = Mathf.Lerp(inicioY, destino.altura, suavizado);
            pos.z = Mathf.Lerp(inicioZ, destino.profundidad, suavizado);

            rb.MovePosition(pos);
            yield return null;
        }

        Vector3 final = rb.position;
        final.y = destino.altura;
        final.z = destino.profundidad;
        rb.MovePosition(final);

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
