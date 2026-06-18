// Este archivo debe estar dentro de una carpeta llamada "Editor"
// Ejemplo: Assets/HELI/CODIGO/Editor/InspectorCleanup.cs

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class InspectorCleanup
{
    static InspectorCleanup()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        // Al salir del Play Mode, deseleccionar todo para que
        // el Inspector no intente acceder a objetos destruidos
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            Selection.activeGameObject = null;
        }
    }
}
#endif