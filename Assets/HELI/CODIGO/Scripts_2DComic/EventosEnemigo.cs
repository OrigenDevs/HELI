using UnityEngine;

public class EventosEnemigo : MonoBehaviour
{
    Enemigo ObtenerEnemigo()
    {
        return GetComponentInParent<Enemigo>();
    }

    EnemigoAtaque ObtenerAtaque()
    {
        return GetComponentInParent<EnemigoAtaque>();
    }

    public void EventoMuerte()
    {
        Enemigo e = ObtenerEnemigo();
        if (e != null) e.EventoMuerte();
    }

    public void ReproducirParticula(int indice)
    {
        Enemigo e = ObtenerEnemigo();
        if (e != null) e.ReproducirParticula(indice);
    }

    public void ActivarZonaAtaque()
    {
        EnemigoAtaque a = ObtenerAtaque();
        if (a != null) a.ActivarZonaAtaque();
    }

    public void DesactivarZonaAtaque()
    {
        EnemigoAtaque a = ObtenerAtaque();
        if (a != null) a.DesactivarZonaAtaque();
    }

    public void EventoParticula(int indice)
    {
        EnemigoAtaque a = ObtenerAtaque();
        if (a != null) a.EventoParticula(indice);
    }

    public void EventoAudio()
    {
        EnemigoAtaque a = ObtenerAtaque();
        if (a != null) a.EventoAudio();
    }
}