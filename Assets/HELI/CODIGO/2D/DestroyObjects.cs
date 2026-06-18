using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Destruye una lista de objetos y desactiva todos los Canvas que encuentre en ellos.
/// Llama a Ejecutar() desde otro script o desde el Start si activarAlIniciar = true.
/// </summary>
public class DestroyObjects : MonoBehaviour
{
    [Header("Objetos a destruir")]
    public List<GameObject> objetosADestruir;

    [Tooltip("Si es true, se ejecuta automáticamente al iniciar la escena")]
    public bool activarAlIniciar = false;

    void Start()
    {
        if (activarAlIniciar)
            Ejecutar();
    }

    public void Ejecutar()
    {
        foreach (var obj in objetosADestruir)
        {
            if (obj == null) continue;

            // Desactivar Canvas si tiene uno
            Canvas canvas = obj.GetComponent<Canvas>();
            if (canvas != null) canvas.enabled = false;

            // Buscar Canvas en hijos también
            foreach (var c in obj.GetComponentsInChildren<Canvas>())
                c.enabled = false;

            // Destruir el objeto
            Destroy(obj);
        }

        objetosADestruir.Clear();
    }
}