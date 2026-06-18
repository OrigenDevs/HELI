using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Coloca este script SOLO en objetos con tag "Enemy".
/// Complementa a TriggerZone: además de las listas de objetos,
/// activa la animación "ataque" del enemigo por un tiempo configurable.
/// </summary>
public class EnemyTrigger : MonoBehaviour
{
    [Header("Animación de ataque")]
    [Tooltip("Duración de la animación de ataque en segundos")]
    public float duracionAtaque = 1.2f;

    [Header("Objetos que APARECEN al activar este trigger")]
    public List<GameObject> objetosAparecer;

    [Header("Objetos que DESAPARECEN al activar este trigger")]
    public List<GameObject> objetosDesaparecer;

    private Animator playerAnimator;
    private bool activado = false;

    void OnTriggerEnter(Collider other)
    {
        if (activado) return;
        if (!other.CompareTag("Player")) return;

        activado = true;

        // Tomar el Animator del Player al momento del contacto
        playerAnimator = other.GetComponent<Animator>();

        foreach (var obj in objetosAparecer)
            if (obj != null) obj.SetActive(true);

        foreach (var obj in objetosDesaparecer)
            if (obj != null) obj.SetActive(false);

        // Animación de ataque en el Player
        if (playerAnimator != null)
            StartCoroutine(AnimarAtaque());
    }

    private IEnumerator AnimarAtaque()
    {
        playerAnimator.SetTrigger("ataque");
        yield return new WaitForSeconds(duracionAtaque);
        // Aquí puedes agregar lógica post-ataque si lo necesitas
    }
}