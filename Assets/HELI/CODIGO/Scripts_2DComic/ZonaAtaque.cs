using UnityEngine;

public class ZonaAtaque : MonoBehaviour
{
    public EnemigoAtaque enemigo;

    void OnTriggerEnter2D(Collider2D other)
    {
        VidaJugador jugador = other.GetComponent<VidaJugador>();
        if (jugador != null && enemigo != null)
            enemigo.GolpearJugador(jugador);
    }
}