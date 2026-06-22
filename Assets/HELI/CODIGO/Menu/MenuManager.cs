using UnityEngine;
using UnityEngine.InputSystem;

public class Menu3DManager : MonoBehaviour
{
    [Header("Lista de Botones 3D")]
    public MenuButton3D[] botonesMenu;

    [Header("Componente Input System")]
    public PlayerInput playerInput;

    [Header("Configuración de Cámara")]
    public Transform camaraPrincipal;
    [Tooltip("Qué tan rápido y suave se desplaza la cámara al cambiar de botón.")]
    public float suavizadoCamara = 5f;

    [Tooltip("Distancia en el eje Z que mantendrá la cámara frente al botón para no meterse dentro.")]
    public float distanciaZ = -10f;
    [Tooltip("Desplazamiento vertical opcional si quieres que la cámara esté un poco más arriba.")]
    public float alturaY = 0f;

    private InputAction navigateAction;
    private int indiceActual = 0;
    private bool controlEjeBloqueado = false;
    private Vector3 posicionObjetivoCamara;

    void Awake()
    {
        if (playerInput != null)
        {
            navigateAction = playerInput.actions.FindAction("UI/Navigate");
        }

        if (camaraPrincipal == null && Camera.main != null)
        {
            camaraPrincipal = Camera.main.transform;
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

        // Calculamos la posición inicial con la distancia de seguridad en Z
        CalcularPosicionCamara();
        if (camaraPrincipal != null)
        {
            camaraPrincipal.position = posicionObjetivoCamara;
        }
    }

    void Update()
    {
        ManejarInput();
        MoverCamaraSuave();
    }

    private void ManejarInput()
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

    private void CambiarSeleccion(int direccion)
    {
        if (botonesMenu.Length == 0) return;

        botonesMenu[indiceActual].Deseleccionar();
        indiceActual += direccion;

        if (indiceActual >= botonesMenu.Length) indiceActual = 0;
        if (indiceActual < 0) indiceActual = botonesMenu.Length - 1;

        botonesMenu[indiceActual].Seleccionar();

        // Calculamos la nueva posición matemática a la que debe viajar la cámara
        CalcularPosicionCamara();
    }

    private void CalcularPosicionCamara()
    {
        if (botonesMenu[indiceActual] == null) return;

        // Tomamos la posición X e Y del botón actual, pero le aplicamos nuestros offsets manuales
        Vector3 posicionBoton = botonesMenu[indiceActual].transform.position;

        // La cámara se alineará perfectamente con el botón en X, tendrá la altura Y que quieras, 
        // y se quedará atrás en el eje Z según el valor de 'distanciaZ'
        posicionObjetivoCamara = new Vector3(posicionBoton.x, posicionBoton.y + alturaY, posicionBoton.z + distanciaZ);
    }

    private void MoverCamaraSuave()
    {
        if (camaraPrincipal == null) return;

        // Desplazamiento suave y continuo hacia la posición calculada
        camaraPrincipal.position = Vector3.Lerp(camaraPrincipal.position, posicionObjetivoCamara, Time.deltaTime * suavizadoCamara);
    }
}