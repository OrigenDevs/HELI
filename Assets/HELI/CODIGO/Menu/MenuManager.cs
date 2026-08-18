using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class Menu3DManager : MonoBehaviour
{
    [Header("Lista de Botones 3D")]
    public MenuButton3D[] botonesMenu;

    [Header("Configuraci�n de Input (New Input System)")]
    [Tooltip("Acci�n para confirmar (Jump / Bot�n Sur).")]
    public InputActionReference actionJump;
    [Tooltip("Puedes arrastrar una referencia de acci�n para navegar o dejarla vac�a para que use 'WASD' y el Stick Izquierdo por defecto.")]
    public InputActionReference actionNavigate;

    [Header("Configuraci�n de C�mara")]
    public Transform camaraPrincipal;
    public float suavizadoCamara = 5f;
    public float distanciaZ = -10f;
    public float alturaY = 0f;

    [Header("Efecto Shake (Onda de Choque)")]
    [Tooltip("Intensidad inicial del temblor (un valor bajo como 0.15f o 0.2f es ideal para algo sutil).")]
    public float intensidadShake = 0.2f;
    [Tooltip("Qu� tan r�pido se disipa el temblor despu�s del impacto.")]
    public float disipacionShake = 5f;

    private float shakeActual = 0f;
    private Vector3 offsetShake = Vector3.zero;

    [Header("Efecto de Pantalla (Fundido a Blanco Final)")]
    public Image imagenFlashBlanco;
    public float velocidadSubidaBlanco = 2f;

    [Header("Control de Post-Processing")]
    [Tooltip("Arrastra aqu� el objeto de la escena que tiene el Post-Process Volume que quieres activar.")]
    public Volume volumenPostProcess;
    [Tooltip("Qu� tan r�pido sube el volumen de 0 a 1.")]
    public float velocidadSubidaVolumen = 3f;

    [Header("Sistema de Audio Centralizado")]
    public AudioSource audioSourceManager;
    public AudioClip sfxPasarCarta;
    public AudioClip sfxSeleccionarCarta;

    private InputAction jumpActionDinamica;
    private InputAction navigateActionDinamica;

    private int indiceActual = 0;
    private bool controlEjeBloqueado = false;
    private Vector3 posicionObjetivoCamara;

    private float opacidadFlashActual = 0f;
    private float targetOpacidadFlash = 0f;
    private bool navegacionBloqueada = false;

    // Control interno del peso del volumen
    private float pesoVolumenActual = 0f;
    private float targetPesoVolumen = 0f;

    void Awake()
    {
        // Si no asignas una InputActionReference para Jump, creamos una por defecto
        if (actionJump == null || actionJump.action == null)
        {
            jumpActionDinamica = new InputAction("JumpActionDefault");
            jumpActionDinamica.AddBinding("<Keyboard>/space");
            jumpActionDinamica.AddBinding("<Gamepad>/buttonSouth");
        }

        // Si no asignas una InputActionReference para Navigate, creamos una por defecto con WASD y Stick
        if (actionNavigate == null || actionNavigate.action == null)
        {
            navigateActionDinamica = new InputAction("NavigateActionDefault");
            navigateActionDinamica.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
            navigateActionDinamica.AddBinding("<Gamepad>/leftStick");
        }

        if (camaraPrincipal == null && Camera.main != null)
        {
            camaraPrincipal = Camera.main.transform;
        }

        if (imagenFlashBlanco != null)
        {
            Color c = imagenFlashBlanco.color;
            imagenFlashBlanco.color = new Color(c.r, c.g, c.b, 0f);
            imagenFlashBlanco.gameObject.SetActive(true);
        }

        if (audioSourceManager == null)
        {
            audioSourceManager = GetComponent<AudioSource>();
        }

        // Nos aseguramos de que el volumen empiece completamente apagado en el frame 1
        if (volumenPostProcess != null)
        {
            volumenPostProcess.weight = 0f;
        }
    }

    void OnEnable()
    {
        if (actionJump != null && actionJump.action != null && !actionJump.action.enabled)
            actionJump.action.Enable();

        if (jumpActionDinamica != null)
            jumpActionDinamica.Enable();

        if (actionNavigate != null && actionNavigate.action != null && !actionNavigate.action.enabled)
            actionNavigate.action.Enable();

        if (navigateActionDinamica != null)
            navigateActionDinamica.Enable();
    }

    void OnDisable()
    {
        if (actionJump != null && actionJump.action != null && actionJump.action.enabled)
            actionJump.action.Disable();

        if (jumpActionDinamica != null)
            jumpActionDinamica.Disable();

        if (actionNavigate != null && actionNavigate.action != null && actionNavigate.action.enabled)
            actionNavigate.action.Disable();

        if (navigateActionDinamica != null)
            navigateActionDinamica.Disable();
    }

    void OnDestroy()
    {
        jumpActionDinamica?.Dispose();
        navigateActionDinamica?.Dispose();
    }

    void Start()
    {
        if (botonesMenu.Length == 0) return;

        foreach (var boton in botonesMenu)
        {
            if (boton != null) boton.Deseleccionar();
        }

        indiceActual = 0;
        botonesMenu[indiceActual].Seleccionar();

        CalcularPosicionCamara();
        if (camaraPrincipal != null) camaraPrincipal.position = posicionObjetivoCamara;
    }

    void Update()
    {
        if (!navegacionBloqueada)
        {
            ManejarInputNavegacion();
            ManejarInputConfirmacion();
        }

        MoverCamaraSuave();
        ControlarFundidoBlanco();
        AnimarPesoVolumen();
    }

    private void ManejarInputNavegacion()
    {
        Vector2 direccionInput = Vector2.zero;

        // Comprueba si se us� la referencia p�blica o la interna por defecto para navegar
        if (actionNavigate != null && actionNavigate.action != null)
        {
            direccionInput = actionNavigate.action.ReadValue<Vector2>();
        }
        else if (navigateActionDinamica != null)
        {
            direccionInput = navigateActionDinamica.ReadValue<Vector2>();
        }

        if (Mathf.Abs(direccionInput.x) > 0.5f)
        {
            if (!controlEjeBloqueado)
            {
                controlEjeBloqueado = true;
                int cambio = direccionInput.x > 0 ? 1 : -1;
                CambiarSeleccion(cambio);
            }
        }
        else
        {
            controlEjeBloqueado = false;
        }
    }

    private void ManejarInputConfirmacion()
    {
        bool triggerAccion = false;

        if (actionJump != null && actionJump.action != null)
        {
            triggerAccion = actionJump.action.triggered;
        }
        else if (jumpActionDinamica != null)
        {
            triggerAccion = jumpActionDinamica.triggered;
        }

        if (triggerAccion)
        {
            navegacionBloqueada = true;

            foreach (var boton in botonesMenu)
            {
                if (boton != null) boton.EjecutarConfirmacion();
            }

            ReproducirSonido(sfxSeleccionarCarta);

            targetOpacidadFlash = 1f;
            targetPesoVolumen = 1f;
            shakeActual = intensidadShake;

            Debug.Log("Confirmado con Action Jump: Ejecutando Onda de Choque en C�mara.");
        }
    }

    public void CambiarSeleccionPorIndice(int nuevoIndice)
    {
        if (botonesMenu.Length == 0) return;
        if (nuevoIndice < 0 || nuevoIndice >= botonesMenu.Length) return;
        if (nuevoIndice == indiceActual) return;

        botonesMenu[indiceActual].Deseleccionar();
        indiceActual = nuevoIndice;
        botonesMenu[indiceActual].Seleccionar();

        ReproducirSonido(sfxPasarCarta);

        CalcularPosicionCamara();
    }

    public void Confirmar()
    {
        if (navegacionBloqueada) return;
        navegacionBloqueada = true;

        foreach (var boton in botonesMenu)
        {
            if (boton != null) boton.EjecutarConfirmacion();
        }

        ReproducirSonido(sfxSeleccionarCarta);

        targetOpacidadFlash = 1f;
        targetPesoVolumen = 1f;
        shakeActual = intensidadShake;
    }

    public void FijarCamara(Vector3 posicion)
    {
        posicionObjetivoCamara = posicion;
        if (camaraPrincipal != null)
            camaraPrincipal.position = posicion;
    }

    private void AnimarPesoVolumen()
    {
        if (volumenPostProcess == null || !navegacionBloqueada) return;

        pesoVolumenActual = Mathf.MoveTowards(pesoVolumenActual, targetPesoVolumen, Time.deltaTime * velocidadSubidaVolumen);
        volumenPostProcess.weight = pesoVolumenActual;
    }

    private void ControlarFundidoBlanco()
    {
        if (imagenFlashBlanco == null || !navegacionBloqueada) return;

        opacidadFlashActual = Mathf.MoveTowards(opacidadFlashActual, targetOpacidadFlash, Time.deltaTime * velocidadSubidaBlanco);

        Color c = imagenFlashBlanco.color;
        imagenFlashBlanco.color = new Color(c.r, c.g, c.b, opacidadFlashActual);

        if (opacidadFlashActual >= 0.95f)
        {
            targetOpacidadFlash = opacidadFlashActual;

            MenuButton3D botonSeleccionado = botonesMenu[indiceActual];
            if (botonSeleccionado != null && !string.IsNullOrEmpty(botonSeleccionado.nombreEscenaDestino))
            {
                SceneManager.LoadScene(botonSeleccionado.nombreEscenaDestino);
            }
            else
            {
                Debug.LogError("Menu3DManager: La carta seleccionada no tiene un nombre de escena asignado.");
            }
        }
    }

    private void CambiarSeleccion(int direccion)
    {
        if (botonesMenu.Length == 0) return;

        botonesMenu[indiceActual].Deseleccionar();
        indiceActual += direccion;

        if (indiceActual >= botonesMenu.Length) indiceActual = 0;
        if (indiceActual < 0) indiceActual = botonesMenu.Length - 1;

        botonesMenu[indiceActual].Seleccionar();

        ReproducirSonido(sfxPasarCarta);

        CalcularPosicionCamara();
    }

    private void CalcularPosicionCamara()
    {
        if (botonesMenu[indiceActual] == null) return;
        Vector3 posicionBoton = botonesMenu[indiceActual].transform.position;
        posicionObjetivoCamara = new Vector3(posicionBoton.x, posicionBoton.y + alturaY, posicionBoton.z + distanciaZ);
    }

    private void MoverCamaraSuave()
    {
        if (camaraPrincipal == null) return;

        if (shakeActual > 0.001f)
        {
            offsetShake = Random.insideUnitSphere * shakeActual;
            shakeActual = Mathf.MoveTowards(shakeActual, 0f, Time.deltaTime * disipacionShake);
        }
        else
        {
            offsetShake = Vector3.zero;
        }

        Vector3 posicionBaseLerp = Vector3.Lerp(camaraPrincipal.position - offsetShake, posicionObjetivoCamara, Time.deltaTime * suavizadoCamara);
        camaraPrincipal.position = posicionBaseLerp + offsetShake;
    }

    private void ReproducirSonido(AudioClip clip)
    {
        if (audioSourceManager != null && clip != null)
        {
            audioSourceManager.PlayOneShot(clip);
        }
    }
}