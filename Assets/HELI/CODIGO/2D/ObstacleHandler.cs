using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adjunta este script al GameObject del Player (tag "Player").
/// Detecta colisión con objetos tag "Obstaculo" y gestiona la derrota.
/// </summary>
public class ObstacleHandler : MonoBehaviour
{
    [Header("Menú de derrota")]
    [Tooltip("GameObject del menú de derrota")]
    public GameObject menu_derrota;

    [Tooltip("Segundos de retraso antes de mostrar el menú")]
    public float tiempoRetrasoDerrota = 1.5f;

    [Header("Objetos que se PRENDEN al perder")]
    public List<GameObject> objetosPrender;

    [Header("Objetos que se APAGAN al perder")]
    public List<GameObject> objetosApagar;

    private RunnerMovement runnerMovement;
    private bool gameOver = false;

    void Awake()
    {
        runnerMovement = GetComponent<RunnerMovement>();

        // Asegurarse de que el menú empiece apagado
        if (menu_derrota != null)
            menu_derrota.SetActive(false);
    }

    // Colisión física normal con obstáculo
    void OnCollisionEnter(Collision collision)
    {
        if (!gameOver && collision.gameObject.CompareTag("Obstaculo"))
            IniciarDerrota();
    }

    // Por si el obstáculo usa trigger
    void OnTriggerEnter(Collider other)
    {
        if (!gameOver && other.CompareTag("Obstaculo"))
            IniciarDerrota();
    }

    private void IniciarDerrota()
    {
        if (gameOver) return;
        gameOver = true;

        // Detener el personaje
        if (runnerMovement != null)
            runnerMovement.Detener();

        // Encender/apagar objetos de inmediato
        foreach (var obj in objetosPrender)
            if (obj != null) obj.SetActive(true);

        foreach (var obj in objetosApagar)
            if (obj != null) obj.SetActive(false);

        // Mostrar menú con retraso
        StartCoroutine(MostrarMenuDerrota());
    }

    private IEnumerator MostrarMenuDerrota()
    {
        yield return new WaitForSeconds(tiempoRetrasoDerrota);

        if (menu_derrota != null)
            menu_derrota.SetActive(true);
    }

    public bool EsGameOver() => gameOver;
}