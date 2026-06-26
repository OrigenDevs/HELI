using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(JumpController))]
public class RunnerMovement : MonoBehaviour
{
    [Header("Movimiento adelante")]
    public float velocidad = 8f;

    [Tooltip("Eje de movimiento: X, Y o Z")]
    public enum EjeMovimiento { X, Y, Z }
    public EjeMovimiento eje = EjeMovimiento.Z;

    [Tooltip("Dirección: 1 = positivo, -1 = negativo")]
    public float direccion = 1f;

    public enum EjeCarril { X, Z }

    [Header("Carriles")]
    [Tooltip("Eje por el que se cambia de carril")]
    public EjeCarril ejeCarril = EjeCarril.Z;

    [Tooltip("Separación entre carriles")]
    public float anchoCarril = 2f;

    [Tooltip("Velocidad de deslizamiento entre carriles")]
    public float velocidadCambioCarril = 30f;

    [Tooltip("Carril inicial (0 = centro)")]
    public int carrilInicial = 0;

    [Tooltip("Carril mínimo (negativo = izquierda)")]
    public int carrilMin = -1;

    [Tooltip("Carril máximo (positivo = derecha)")]
    public int carrilMax = 1;

    [Tooltip("Altura que sube/baja por cada carril. Ej: 2 = cada carril está 2 unidades más arriba")]
    public float alturaPorCarril = 0f;

    [Tooltip("Velocidad de subida/bajada entre carriles")]
    public float velocidadAltura = 20f;

    [Header("Detección de suelo")]
    [SerializeField] private float distanciaAlSuelo = 1.1f;

    private Rigidbody rb;
    private Animator animator;
    private JumpController jumpController;
    private bool estaEnSuelo = false;
    private bool estaVivo = true;

    private int carrilActual;
    private float posicionObjetivoCarril;
    private float posicionBaseCarril;
    private float posicionYBase;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        jumpController = GetComponent<JumpController>();

        rb.constraints = RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationY
                       | RigidbodyConstraints.FreezeRotationZ;

        carrilActual = carrilInicial;
        posicionBaseCarril = (ejeCarril == EjeCarril.X) ? transform.position.x : transform.position.z;
        posicionObjetivoCarril = posicionBaseCarril + carrilActual * anchoCarril;
        posicionYBase = transform.position.y;
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

            if (kb.aKey.wasPressedThisFrame || kb.leftArrowKey.wasPressedThisFrame)
                CambiarCarril(-1);

            if (kb.dKey.wasPressedThisFrame || kb.rightArrowKey.wasPressedThisFrame)
                CambiarCarril(1);
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

        float posActual = (ejeCarril == EjeCarril.X) ? rb.position.x : rb.position.z;
        float diff = posicionObjetivoCarril - posActual;
        float velCarril = (Mathf.Abs(diff) > 0.02f) ? Mathf.Sign(diff) * velocidadCambioCarril : 0f;

        if (ejeCarril == EjeCarril.X) v.x = velCarril;
        else v.z = velCarril;

        float objetivoY = posicionYBase + carrilActual * alturaPorCarril;
        float diffY = objetivoY - rb.position.y;
        if (Mathf.Abs(diffY) > 0.05f)
            v.y = Mathf.Sign(diffY) * velocidadAltura;

        rb.linearVelocity = v;
    }

    private void CambiarCarril(int direccionCarril)
    {
        if (!estaEnSuelo) return;
        carrilActual = Mathf.Clamp(carrilActual + direccionCarril, carrilMin, carrilMax);
        posicionObjetivoCarril = posicionBaseCarril + carrilActual * anchoCarril;
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
    }

    public bool EstaEnSuelo() => estaEnSuelo;
    public bool EstaVivo()    => estaVivo;
}
