using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Arrastra este script directamente al botón de UI.
/// Al hacer clic reinicia la escena actual automáticamente.
/// No necesita configuración adicional.
/// </summary>
[RequireComponent(typeof(Button))]
public class RestartScene : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Reiniciar);
    }

    private void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}