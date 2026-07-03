using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Coloca este script en Enemigos, Cajas y Línea de llegada.
/// Requiere un Collider 3D con "Is Trigger" activado en el objeto.
/// </summary>
public class TriggerZone : MonoBehaviour
{
    [Header("Objetos que APARECEN al activar este trigger")]
    public List<GameObject> objetosAparecer;

    [Header("Objetos que DESAPARECEN al activar este trigger")]
    public List<GameObject> objetosDesaparecer;

    private bool activado = false;

    void OnTriggerEnter(Collider other)
    {
        if (activado) return;
        if (!other.CompareTag("Player")) return;

        activado = true;

        foreach (var obj in objetosAparecer)
            if (obj != null) obj.SetActive(true);

        foreach (var obj in objetosDesaparecer)
            if (obj != null) obj.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (activado) return;
        if (!other.CompareTag("Player")) return;

        activado = true;

        foreach (var obj in objetosAparecer)
            if (obj != null) obj.SetActive(true);

        foreach (var obj in objetosDesaparecer)
            if (obj != null) obj.SetActive(false);
    }
}