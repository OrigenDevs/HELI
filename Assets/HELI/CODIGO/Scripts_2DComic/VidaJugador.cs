using System.Collections.Generic;
using UnityEngine;

public class VidaJugador : MonoBehaviour
{
    [Header("Vida")]
    public float vida = 3f;
    public float invulnerabilidad = 0.5f;

    [Header("Particulas de daño")]
    public List<ParticleSystem> particulasDano;

    public System.Action onMuerte;
    public bool muerta;

    private float tiempoUltimoDaño;

    public void RecibirDano(float dano)
    {
        if (muerta || dano <= 0f) return;
        if (Time.time < tiempoUltimoDaño + invulnerabilidad) return;
        tiempoUltimoDaño = Time.time;

        vida -= dano;
        ReproducirParticula(0);

        Animator animator = GetComponentInChildren<Animator>();
        if (animator != null)
            animator.SetTrigger("golpeado");

        if (vida <= 0f)
            Morir();
    }

    public void ReproducirParticula(int indice)
    {
        if (particulasDano == null || indice < 0 || indice >= particulasDano.Count) return;
        particulasDano[indice].Play();
    }

    void Morir()
    {
        muerta = true;
        if (onMuerte != null) onMuerte();
    }
}