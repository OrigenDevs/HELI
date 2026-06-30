using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class MovimientoBEU : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;

    private Rigidbody2D rb;
    private PlayerInputActions actions;
    private Vector2 direccion;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
        rb.linearVelocity = direccion * velocidad;
    }
}
