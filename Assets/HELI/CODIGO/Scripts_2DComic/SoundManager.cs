using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instancia;

    public AudioClip[] musicaFondo;
    public bool aleatorio;

    private AudioSource fuenteSfx;
    private AudioSource fuenteMusica;
    private int indiceMusica;

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        fuenteSfx = GetComponent<AudioSource>();
        if (fuenteSfx == null)
            fuenteSfx = gameObject.AddComponent<AudioSource>();

        GameObject musicGO = new GameObject("Musica");
        musicGO.transform.SetParent(transform);
        fuenteMusica = musicGO.AddComponent<AudioSource>();
        fuenteMusica.loop = false;
    }

    void Start()
    {
        if (musicaFondo.Length > 0)
        {
            indiceMusica = aleatorio ? Random.Range(0, musicaFondo.Length) : 0;
            ReproducirMusica(indiceMusica);
        }
    }

    void ReproducirMusica(int indice)
    {
        if (indice < 0 || indice >= musicaFondo.Length) return;
        CancelInvoke(nameof(SiguienteCancion));
        indiceMusica = indice;
        fuenteMusica.clip = musicaFondo[indiceMusica];
        fuenteMusica.Play();
        Invoke(nameof(SiguienteCancion), fuenteMusica.clip.length);
    }

    void SiguienteCancion()
    {
        if (musicaFondo.Length == 0) return;

        if (aleatorio)
        {
            int nuevo;
            do { nuevo = Random.Range(0, musicaFondo.Length); }
            while (musicaFondo.Length > 1 && nuevo == indiceMusica);
            indiceMusica = nuevo;
        }
        else
        {
            indiceMusica = (indiceMusica + 1) % musicaFondo.Length;
        }

        ReproducirMusica(indiceMusica);
    }

    public void Reproducir(AudioClip clip)
    {
        if (clip != null)
            fuenteSfx.PlayOneShot(clip);
    }
}
