using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class MovimientoBEU : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;
    public float umbralMovimiento = 0.1f;

    [HideInInspector] public bool atacando;
    [HideInInspector] public bool controlBloqueado;

    private Rigidbody2D rb;
    private Animator animator;
    private PlayerInputActions actions;
    private Vector2 direccion;
    private SpriteRenderer sr;
    private static readonly int ParamVelocidad = Animator.StringToHash("velocidad");

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void OnEnable()
    {
        actions = new PlayerInputActions();
        actions.Player.Enable();
        actions.Player.Move.performed += OnMove;
        actions.Player.Move.canceled += OnMoveCancel;
    }

    void OnDisable()
    {
        actions.Player.Move.performed -= OnMove;
        actions.Player.Move.canceled -= OnMoveCancel;
        actions.Player.Disable();
        actions.Dispose();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        direccion = ctx.ReadValue<Vector2>();
    }

    private void OnMoveCancel(InputAction.CallbackContext ctx)
    {
        direccion = Vector2.zero;
    }

    void FixedUpdate()
    {
        if (!atacando && !controlBloqueado)
            rb.linearVelocity = direccion * velocidad;

        if (sr != null && !atacando && !controlBloqueado && Mathf.Abs(direccion.x) > umbralMovimiento)
            sr.flipX = direccion.x < 0f;

        animator.SetFloat(ParamVelocidad, atacando || controlBloqueado ? 0f : rb.linearVelocity.magnitude);
    }
}
