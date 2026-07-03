using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Mision : MonoBehaviour
{
    public Enemigo[] enemigos;
    public UnityEvent onCompletado;

    [Header("Flecha indicadora")]
    public Image flechaDerecha;
    public Image flechaIzquierda;
    public bool flechaALaDerecha = true;
    public int segundosFlecha = 3;

    void Start()
    {
        foreach (var e in enemigos)
        {
            if (e != null)
                e.onDerrotado += Verificar;
        }
        if (flechaDerecha != null) flechaDerecha.gameObject.SetActive(false);
        if (flechaIzquierda != null) flechaIzquierda.gameObject.SetActive(false);
    }

    void Verificar()
    {
        foreach (var e in enemigos)
        {
            if (e != null && !e.muerto) return;
        }
        onCompletado.Invoke();

        CancelInvoke(nameof(OcultarFlecha));

        if (flechaALaDerecha)
        {
            if (flechaDerecha != null) flechaDerecha.gameObject.SetActive(true);
            if (flechaIzquierda != null) flechaIzquierda.gameObject.SetActive(false);
        }
        else
        {
            if (flechaIzquierda != null) flechaIzquierda.gameObject.SetActive(true);
            if (flechaDerecha != null) flechaDerecha.gameObject.SetActive(false);
        }

        Invoke(nameof(OcultarFlecha), segundosFlecha);
    }

    void OcultarFlecha()
    {
        if (flechaDerecha != null) flechaDerecha.gameObject.SetActive(false);
        if (flechaIzquierda != null) flechaIzquierda.gameObject.SetActive(false);
    }
}
