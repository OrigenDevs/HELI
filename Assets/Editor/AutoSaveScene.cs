using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Sistema de auto-guardado automático para Unity Editor
/// Guarda la escena activa cada X minutos
/// </summary>
[InitializeOnLoad]
public class AutoSaveScene
{
    // ⚙️ CONFIGURA AQUÍ EL TIEMPO DE AUTO-GUARDADO
    private static float autoSaveMinutos = 5f; // Cambia este número para modificar el intervalo
    
    // Variables privadas
    private static double siguienteGuardado;
    private static bool autoSaveActivado = true;

    // Constructor estático - se ejecuta al cargar Unity
    static AutoSaveScene()
    {
        // Calcular el siguiente guardado
        siguienteGuardado = EditorApplication.timeSinceStartup + (autoSaveMinutos * 60);
        
        // Suscribirse al update del editor
        EditorApplication.update += AutoSave;
        
        Debug.Log($"✅ Auto-Save activado: guardará cada {autoSaveMinutos} minutos");
    }

    // Método que se ejecuta en cada frame del editor
    static void AutoSave()
    {
        // Solo guardar si el auto-save está activado
        if (!autoSaveActivado)
            return;

        // Solo guardar si no estamos en Play Mode
        if (EditorApplication.isPlaying || EditorApplication.isPaused)
            return;

        // Verificar si ya es hora de guardar
        if (EditorApplication.timeSinceStartup > siguienteGuardado)
        {
            // Guardar la escena activa
            GuardarEscena();
            
            // Programar el siguiente guardado
            siguienteGuardado = EditorApplication.timeSinceStartup + (autoSaveMinutos * 60);
        }
    }

    // Método que guarda la escena
    static void GuardarEscena()
    {
        // Verificar si hay una escena activa
        if (EditorSceneManager.GetActiveScene().path == "")
        {
            Debug.LogWarning("⚠️ Auto-Save: No se puede guardar una escena sin nombre. Guárdala manualmente primero.");
            return;
        }

        // Guardar la escena
        bool guardadoExitoso = EditorSceneManager.SaveOpenScenes();
        
        if (guardadoExitoso)
        {
            // Obtener la hora actual
            string hora = System.DateTime.Now.ToString("HH:mm:ss");
            Debug.Log($"💾 Auto-Save: Escena guardada automáticamente a las {hora}");
        }
        else
        {
            Debug.LogError("❌ Auto-Save: Error al guardar la escena");
        }
    }

    // Menú para activar/desactivar el auto-save
    [MenuItem("Tools/Auto-Save/Activar Auto-Save")]
    static void ActivarAutoSave()
    {
        autoSaveActivado = true;
        Debug.Log("✅ Auto-Save ACTIVADO");
    }

    [MenuItem("Tools/Auto-Save/Desactivar Auto-Save")]
    static void DesactivarAutoSave()
    {
        autoSaveActivado = false;
        Debug.Log("⏸️ Auto-Save DESACTIVADO");
    }

    [MenuItem("Tools/Auto-Save/Guardar Ahora")]
    static void GuardarAhora()
    {
        GuardarEscena();
    }

    [MenuItem("Tools/Auto-Save/Configurar Intervalo/2 Minutos")]
    static void Intervalo2Min()
    {
        autoSaveMinutos = 2f;
        siguienteGuardado = EditorApplication.timeSinceStartup + (autoSaveMinutos * 60);
        Debug.Log("⏱️ Auto-Save configurado a 2 minutos");
    }

    [MenuItem("Tools/Auto-Save/Configurar Intervalo/5 Minutos")]
    static void Intervalo5Min()
    {
        autoSaveMinutos = 5f;
        siguienteGuardado = EditorApplication.timeSinceStartup + (autoSaveMinutos * 60);
        Debug.Log("⏱️ Auto-Save configurado a 5 minutos");
    }

    [MenuItem("Tools/Auto-Save/Configurar Intervalo/10 Minutos")]
    static void Intervalo10Min()
    {
        autoSaveMinutos = 10f;
        siguienteGuardado = EditorApplication.timeSinceStartup + (autoSaveMinutos * 60);
        Debug.Log("⏱️ Auto-Save configurado a 10 minutos");
    }

    [MenuItem("Tools/Auto-Save/Configurar Intervalo/15 Minutos")]
    static void Intervalo15Min()
    {
        autoSaveMinutos = 15f;
        siguienteGuardado = EditorApplication.timeSinceStartup + (autoSaveMinutos * 60);
        Debug.Log("⏱️ Auto-Save configurado a 15 minutos");
    }
}