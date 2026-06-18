using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Arrastra este script directamente al botón de UI.
/// Escribe el nombre exacto de la escena destino en el campo "nombreEscena".
/// No necesita configuración adicional en el botón.
/// </summary>
[RequireComponent(typeof(Button))]
public class LoadScene : MonoBehaviour
{
    [Tooltip("Nombre exacto de la escena destino (debe estar en Build Settings)")]
    public string nombreEscena;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(CargarEscena);
    }

    private void CargarEscena()
    {
        if (string.IsNullOrEmpty(nombreEscena))
        {
            Debug.LogWarning("LoadScene: no hay ninguna escena asignada.");
            return;
        }

        SceneManager.LoadScene(nombreEscena);
    }
}