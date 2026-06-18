using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cuando este GameObject se activa (SetActive true),
/// automáticamente activa y desactiva las listas configuradas.
/// </summary>
public class OnEnableToggle : MonoBehaviour
{
    [Header("Objetos que se ACTIVAN cuando este objeto se prende")]
    public List<GameObject> objetosActivar;

    [Header("Objetos que se DESACTIVAN cuando este objeto se prende")]
    public List<GameObject> objetosDesactivar;

    void OnEnable()
    {
        foreach (var obj in objetosActivar)
            if (obj != null) obj.SetActive(true);

        foreach (var obj in objetosDesactivar)
            if (obj != null) obj.SetActive(false);
    }
}