using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cuando el Player entra al trigger, el objeto empieza a reducir su escala
/// hasta que cualquier eje llegue a cero, momento en que se autodestruye.
/// También activa y desactiva listas de objetos al entrar.
/// </summary>
public class EscaladorYDestructor : MonoBehaviour
{
    [Header("Animación de Escala")]
    [Tooltip("Objeto que se escalará y rotará. Si se deja vacío se usará el mismo objeto del script.")]
    public Transform objetoAEscalar;

    [Tooltip("Velocidad a la que se reduce la escala por segundo.")]
    public float velocidadEscalado = 1f;

    [Tooltip("Velocidad de rotación en grados por segundo mientras se escala.")]
    public float velocidadRotacion = 180f;

    [Header("Objetos a Activar")]
    [Tooltip("GameObjects que se ACTIVARÁN cuando el Player entre al trigger.")]
    public List<GameObject> objetosAActivar = new List<GameObject>();

    [Header("Objetos a Apagar")]
    [Tooltip("GameObjects que se DESACTIVARÁN cuando el Player entre al trigger.")]
    public List<GameObject> objetosAApagar = new List<GameObject>();

    [Header("Configuración")]
    [Tooltip("Tag del jugador.")]
    public string tagJugador = "Player";

    private bool reduciendo = false;
    private Transform objetivo;

    void Start()
    {
        objetivo = (objetoAEscalar != null) ? objetoAEscalar : transform;
    }

    void OnTriggerEnter(Collider other)
    {
        if (reduciendo) return;
        if (!other.CompareTag(tagJugador)) return;

        reduciendo = true;

        foreach (GameObject obj in objetosAActivar)
            if (obj != null) obj.SetActive(true);

        foreach (GameObject obj in objetosAApagar)
            if (obj != null) obj.SetActive(false);
    }

    void Update()
    {
        if (!reduciendo) return;

        objetivo.localScale -= Vector3.one * velocidadEscalado * Time.deltaTime;
        objetivo.Rotate(Vector3.up, velocidadRotacion * Time.deltaTime, Space.World);

        // Destruir cuando cualquier eje llegue a cero o menos
        Vector3 s = objetivo.localScale;
        if (s.x <= 0f || s.y <= 0f || s.z <= 0f)
            Destroy(gameObject);
    }
}