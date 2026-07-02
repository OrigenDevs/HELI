using UnityEngine;
using UnityEngine.Events;

public class Mision : MonoBehaviour
{
    public Enemigo[] enemigos;
    public UnityEvent onCompletado;

    void Start()
    {
        foreach (var e in enemigos)
        {
            if (e != null)
                e.onDerrotado += Verificar;
        }
    }

    void Verificar()
    {
        foreach (var e in enemigos)
        {
            if (e != null && !e.muerto) return;
        }
        onCompletado.Invoke();
    }
}
