using System.Collections;
using UnityEngine;

public class MoverOnEnabled : MonoBehaviour
{
    public enum Eje { X, Y, MenosX, MenosY }

    [Header("Movimiento")]
    public Eje eje = Eje.Y;
    [Tooltip("Desplazamiento inicial desde donde entra. Y entra desde abajo, X desde la izquierda, MenosX desde la derecha, MenosY desde arriba.")]
    public float distancia = 200f;
    public float duracion = 0.6f;

    private RectTransform rt;
    private Vector2 posicionOriginal;
    private bool guardada = false;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        if (rt == null) return;

        if (!guardada)
        {
            posicionOriginal = rt.anchoredPosition;
            guardada = true;
        }

        StartCoroutine(AnimarEntrada());
    }

    IEnumerator AnimarEntrada()
    {
        Vector2 inicio = posicionOriginal;
        switch (eje)
        {
            case Eje.X: inicio.x += distancia; break;
            case Eje.Y: inicio.y += distancia; break;
            case Eje.MenosX: inicio.x -= distancia; break;
            case Eje.MenosY: inicio.y -= distancia; break;
        }

        rt.anchoredPosition = inicio;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duracion;
            rt.anchoredPosition = Vector2.Lerp(inicio, posicionOriginal, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        rt.anchoredPosition = posicionOriginal;
    }
}
