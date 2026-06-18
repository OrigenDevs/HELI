using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [Header("UI")]
    [SerializeField] private GameObject touchUI;

    public InputMode CurrentInputMode { get; private set; }

    public PlayerInputData CurrentInput { get; private set; }

    // Cuando está en true, CurrentInput devuelve valores vacíos (sin movimiento ni salto)
    public bool InputBloqueado { get; set; }

    private PlayerInputActions actions;
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool jumpHeld;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);

        actions = new PlayerInputActions();
    }

    void OnEnable()
    {
        actions.Enable();

        actions.Player.Move.performed += ctx => OnMove(ctx);
        actions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        actions.Player.Jump.performed += _ => jumpPressed = true;
        actions.Player.Jump.canceled += _ => jumpHeld = false;
    }

    void OnDisable()
    {
        actions.Disable();
    }

    void Update()
    {
        DetectActiveInputMode();
        UpdateTouchUI();
        BuildInputData();
    }

    // ---------------- INPUT CALLBACKS ----------------

    void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
        UpdateInputModeFromDevice(ctx.control.device);
    }

    void DetectActiveInputMode()
    {
        if (Touchscreen.current != null && Touchscreen.current.wasUpdatedThisFrame)
            CurrentInputMode = InputMode.Touch;
    }

    void UpdateInputModeFromDevice(InputDevice device)
    {
        if (device is Touchscreen)
            CurrentInputMode = InputMode.Touch;
        else if (device is Gamepad)
            CurrentInputMode = InputMode.Gamepad;
        else if (device is Keyboard)
            CurrentInputMode = InputMode.Keyboard;
    }

    // ---------------- UI ----------------

    void UpdateTouchUI()
    {
        bool showTouch =
            DeviceDetector.isMobileWebGl &&
            CurrentInputMode == InputMode.Touch;

        touchUI.SetActive(showTouch);
    }

    // ---------------- DATA ----------------

    void BuildInputData()
    {
        if (InputBloqueado)
        {
            // Input bloqueado: devolver valores vacíos para detener movimiento y salto
            CurrentInput = new PlayerInputData
            {
                movement = Vector2.zero,
                jumpPressed = false,
                jumpHeld = false
            };
        }
        else
        {
            CurrentInput = new PlayerInputData
            {
                movement = moveInput,
                jumpPressed = jumpPressed,
                jumpHeld = jumpHeld
            };
        }

        jumpPressed = false; // reset frame-based input
    }
}

[System.Serializable]
public struct PlayerInputData
{
    public Vector2 movement;
    public bool jumpPressed;
    public bool jumpHeld;
}


public enum InputMode
{
    Touch,
    Keyboard,
    Gamepad
}