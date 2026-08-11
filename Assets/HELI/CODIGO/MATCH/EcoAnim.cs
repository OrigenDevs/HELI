using System.Collections;
using UnityEngine;

public class EcoAnim : MonoBehaviour
{
    [Header("Eco")]
    [Tooltip("Activa/desactiva el pulso de escala.")]
    public bool activo = true;
    public float escalaBase = 1f;
    public float intensidad = 0.05f;
    public float velocidad = 6f;

    private RectTransform rt;
    private Coroutine corrutina;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        if (!activo) return;
        corrutina = StartCoroutine(AnimarEco());
    }

    void OnDisable()
    {
        if (corrutina != null)
        {
            StopCoroutine(corrutina);
            corrutina = null;
        }
    }

    public void Activar()
    {
        activo = true;
        if (corrutina == null && isActiveAndEnabled)
            corrutina = StartCoroutine(AnimarEco());
    }

    public void Desactivar()
    {
        activo = false;
        if (corrutina != null)
        {
            StopCoroutine(corrutina);
            corrutina = null;
        }
        if (rt != null) rt.localScale = Vector3.one * escalaBase;
    }

    IEnumerator AnimarEco()
    {
        while (true)
        {
            float pulso = Mathf.Sin(Time.time * velocidad) * intensidad;
            if (rt != null)
                rt.localScale = Vector3.one * (escalaBase + pulso);
            yield return null;
        }
    }
}
