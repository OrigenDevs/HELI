using UnityEngine;

public class MusicaEscena : MonoBehaviour
{
    public AudioClip[] musicaFondo;
    public bool reiniciar = true;

    void Start()
    {
        if (SoundManager.instancia == null) return;

        if (musicaFondo.Length > 0)
            SoundManager.instancia.EstablecerMusica(musicaFondo, reiniciar);
        else
            SoundManager.instancia.DetenerMusica();
    }
}
