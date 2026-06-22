using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering; // NECESARIO para interactuar con el componente Volume

public class Menu3DManager : MonoBehaviour
{
    [Header("Lista de Botones 3D")]
    public MenuButton3D[] botonesMenu;

    [Header("Componente Input System")]
    public PlayerInput playerInput;

    [Header("Configuración de Cámara")]
    public Transform camaraPrincipal;
    public float suavizadoCamara = 5f;
    public float distanciaZ = -10f;
    public float alturaY = 0f;

    [Header("Efecto Shake (Onda de Choque)")]
    [Tooltip("Intensidad inicial del temblor (un valor bajo como 0.15f o 0.2f es ideal para algo sutil).")]
    public float intensidadShake = 0.2f;
    [Tooltip("Qué tan rápido se disipa el temblor después del impacto.")]
    public float disipacionShake = 5f;

    private float shakeActual = 0f;
    private Vector3 offsetShake = Vector3.zero;

    [Header("Efecto de Pantalla (Fundido a Blanco Final)")]
    public Image imagenFlashBlanco;
    public float velocidadSubidaBlanco = 2f;

    [Header("Control de Post-Processing")]
    [Tooltip("Arrastra aquí el objeto de la escena que tiene el Post-Process Volume que quieres activar.")]
    public Volume volumenPostProcess;
    [Tooltip("Qué tan rápido sube el volumen de 0 a 1.")]
    public float velocidadSubidaVolumen = 3f;

    [Header("Sistema de Audio Centralizado")]
    public AudioSource audioSourceManager;
    public AudioClip sfxPasarCarta;
    public AudioClip sfxSeleccionarCarta;

    private InputAction navigateAction;
    private InputAction submitAction;

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
        if (playerInput != null)
        {
            navigateAction = playerInput.actions.FindAction("UI/Navigate");
            submitAction = playerInput.actions.FindAction("UI/Submit");
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
        AnimarPesoVolumen(); // <--- Transición del Post Process
    }

    private void ManejarInputNavegacion()
    {
        if (navigateAction == null) return;
        Vector2 direccionInput = navigateAction.ReadValue<Vector2>();

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
        if (submitAction == null) return;

        if (submitAction.WasPerformedThisFrame())
        {
            navegacionBloqueada = true;

            foreach (var boton in botonesMenu)
            {
                if (boton != null) boton.EjecutarConfirmacion();
            }

            ReproducirSonido(sfxSeleccionarCarta);

            // ACTIVACIÓN: Activamos el flash, el post-process y disparamos la intensidad del shake
            targetOpacidadFlash = 1f;
            targetPesoVolumen = 1f;
            shakeActual = intensidadShake; // <-- Comienza el temblor aquí

            Debug.Log("Confirmado: Ejecutando Onda de Choque en Cámara.");
        }
    }

  

    private void AnimarPesoVolumen()
    {
        if (volumenPostProcess == null || !navegacionBloqueada) return;

        // Subimos linealmente el peso hacia el target (1f)
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

        // 1. Calculamos el temblor de forma independiente si está activo
        if (shakeActual > 0.001f)
        {
            // Generamos un desfase aleatorio en una esfera pequeña 3D multiplicado por la intensidad actual
            offsetShake = Random.insideUnitSphere * shakeActual;

            // Reducimos linealmente la fuerza del temblor frame a frame para que se apague suave
            shakeActual = Mathf.MoveTowards(shakeActual, 0f, Time.deltaTime * disipacionShake);
        }
        else
        {
            offsetShake = Vector3.zero;
        }

        // 2. Interpolamos la posición base de la cámara hacia el botón como siempre
        Vector3 posicionBaseLerp = Vector3.Lerp(camaraPrincipal.position - offsetShake, posicionObjetivoCamara, Time.deltaTime * suavizadoCamara);

        // 3. Aplicamos la posición final sumándole el offset del temblor
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