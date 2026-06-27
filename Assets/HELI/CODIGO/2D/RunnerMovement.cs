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
    public float duracionTransicion = 0.08f;

    [Header("Detección de suelo")]
    [SerializeField] private float distanciaAlSuelo = 1.1f;

    [Header("Coyote Time")]
    [Tooltip("Tiempo en segundos después de salir de un borde donde aún se puede saltar")]
    public float tiempoCoyote = 0.1f;

    private Rigidbody rb;
    private Animator animator;
    private JumpController jumpController;
    private PlayerInputActions actions;
    private bool estaEnSuelo = false;
    private bool estaVivo = true;

    private int carrilActual;
    private bool transicionando = false;
    private float direccionInputAnterior = 0f;
    private float ultimoTiempoEnSuelo;

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

    void OnEnable()
    {
        actions = new PlayerInputActions();
        actions.Player.Enable();

        actions.Player.Move.performed += OnMove;
        actions.Player.Move.canceled += OnMoveCancel;
        actions.Player.Jump.performed += OnJump;
    }

    void OnDisable()
    {
        actions.Player.Move.performed -= OnMove;
        actions.Player.Move.canceled -= OnMoveCancel;
        actions.Player.Jump.performed -= OnJump;
        actions.Player.Disable();
        actions.Dispose();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        if (!estaVivo) return;
        direccionInputAnterior = ctx.ReadValue<Vector2>().x;
    }

    private void OnMoveCancel(InputAction.CallbackContext ctx)
    {
        direccionInputAnterior = 0f;
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (!estaVivo) return;
        if (estaEnSuelo || Time.time - ultimoTiempoEnSuelo <= tiempoCoyote)
        {
            IntentarSaltar();
            ultimoTiempoEnSuelo = -tiempoCoyote;
        }
    }

    void Update()
    {
        if (!estaVivo) return;

        VerificarSuelo();
        ManejarAnimaciones();

        if (!transicionando && estaEnSuelo)
        {
            if (Mathf.Abs(direccionInputAnterior) > 0.5f)
            {
                CambiarCarril((int)Mathf.Sign(direccionInputAnterior));
                direccionInputAnterior = 0f;
            }
        }
    }

    void FixedUpdate()
    {
        if (!estaVivo) return;

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
        if (nuevoIndice == carrilActual) return;
        carrilActual = nuevoIndice;
        StartCoroutine(TransicionarACarril());
    }

    private System.Collections.IEnumerator TransicionarACarril()
    {
        transicionando = true;
        Carril destino = carriles[carrilActual];

        float inicioY = transform.position.y;
        float inicioZ = transform.position.z;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / duracionTransicion;
            float suavizado = t * t * (3f - 2f * t);

            Vector3 pos = transform.position;
            pos.y = Mathf.Lerp(inicioY, destino.altura, suavizado);
            pos.z = Mathf.Lerp(inicioZ, destino.profundidad, suavizado);
            transform.position = pos;

            yield return null;
        }

        Vector3 final = transform.position;
        final.y = destino.altura;
        final.z = destino.profundidad;
        transform.position = final;

        rb.position = transform.position;

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

        if (estaEnSuelo)
            ultimoTiempoEnSuelo = Time.time;

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
