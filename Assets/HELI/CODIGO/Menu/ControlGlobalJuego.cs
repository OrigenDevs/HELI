using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ControlGlobalJuego : MonoBehaviour
{
    [Header("Menu Principal")]
    [Tooltip("Nombre exacto de la escena del menu principal (ej: HOME).")]
    public string nombreMenuPrincipal = "HOME";

    [Header("Teclas")]
    [Tooltip("Tecla para pausar / reanudar el juego.")]
    public Key teclaPausa = Key.Z;
    [Tooltip("Tecla para volver al menu principal.")]
    public Key teclaMenu = Key.X;
    [Tooltip("Tecla para cerrar la aplicacion.")]
    public Key teclaSalir = Key.C;

    private bool pausado;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current[teclaPausa].wasPressedThisFrame)
            AlternarPausa();

        if (Keyboard.current[teclaMenu].wasPressedThisFrame)
            VolverAlMenu();

        if (Keyboard.current[teclaSalir].wasPressedThisFrame)
            SalirDelJuego();
    }

    public void AlternarPausa()
    {
        pausado = !pausado;
        Time.timeScale = pausado ? 0f : 1f;
    }

    public void VolverAlMenu()
    {
        pausado = false;
        Time.timeScale = 1f;
        if (string.IsNullOrEmpty(nombreMenuPrincipal))
        {
            Debug.LogError("ControlGlobalJuego: Escribe el nombre de la escena del menu en nombreMenuPrincipal.");
            return;
        }
        SceneManager.LoadScene(nombreMenuPrincipal);
    }

    public void SalirDelJuego()
    {
        pausado = false;
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
