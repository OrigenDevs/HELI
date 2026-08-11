using UnityEngine;

public class ZonaGolpe : MonoBehaviour
{
    public GolpeJugador jugador;

    void OnTriggerEnter2D(Collider2D other)
    {
        Enemigo enemigo = other.GetComponent<Enemigo>();
        if (enemigo != null && jugador != null)
            jugador.HitboxGolpear(enemigo);
    }
}
