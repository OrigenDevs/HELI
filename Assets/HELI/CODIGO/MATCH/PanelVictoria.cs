using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelVictoria : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panel;

    [Header("Texto")]
    public TMP_Text textoVictoria;

    [Header("Resultado modo solo")]
    [Tooltip("Texto con el tiempo y los intentos al ganar en solitario.")]
    public TMP_Text textoResultado;

    [Header("Imagenes por jugador")]
    public Image imagenP1;
    public Image imagenP2;

    [Header("Animacion de aparicion")]
    public float duracionAnimacion = 0.6f;

    [Header("Ganador (0 = P1, 1 = P2)")]
    public MatchCards matchCards;

    private Vector2 escalaOriginalP1;
    private Vector2 escalaOriginalP2;
    private Color colorOriginalP1;
    private Color colorOriginalP2;
    private Vector2 escalaOriginalTexto;
    private Color colorOriginalTexto;
    private Coroutine corrutinaAnimacion;

    void Awake()
    {
        if (panel != null) panel.SetActive(false);

        if (imagenP1 != null)
        {
            escalaOriginalP1 = imagenP1.rectTransform.localScale;
            colorOriginalP1 = imagenP1.color;
        }
        if (imagenP2 != null)
        {
            escalaOriginalP2 = imagenP2.rectTransform.localScale;
            colorOriginalP2 = imagenP2.color;
        }
        if (textoVictoria != null)
        {
            escalaOriginalTexto = textoVictoria.rectTransform.localScale;
            colorOriginalTexto = textoVictoria.color;
        }
    }

    public void MostrarVictoria()
    {
        int ganador = matchCards != null ? matchCards.Ganador() : 0;

        if (panel != null) panel.SetActive(true);

        if (textoVictoria != null)
            textoVictoria.text = "YOU WIN";

        if (textoResultado != null)
        {
            if (matchCards != null && matchCards.jugadores == 1 && matchCards.hud != null)
            {
                float tiempo = matchCards.hud.TiempoActual();
                int minutos = Mathf.FloorToInt(tiempo / 60f);
                int segundos = Mathf.FloorToInt(tiempo % 60f);
                textoResultado.text = "Tiempo: " + string.Format("{0:00}:{1:00}", minutos, segundos) +
                                      "  Intentos: " + matchCards.Intentos;
                textoResultado.gameObject.SetActive(true);
            }
            else
            {
                textoResultado.gameObject.SetActive(false);
            }
        }

        if (corrutinaAnimacion != null) StopCoroutine(corrutinaAnimacion);
        corrutinaAnimacion = StartCoroutine(AnimarGanador(ganador));
    }

    public void Ocultar()
    {
        if (panel != null) panel.SetActive(false);
    }

    IEnumerator AnimarGanador(int ganador)
    {
        Image imagen = ganador == 0 ? imagenP1 : imagenP2;

        if (textoVictoria != null)
        {
            textoVictoria.gameObject.SetActive(true);

            RectTransform rtTexto = textoVictoria.rectTransform;
            rtTexto.localScale = new Vector3(escalaOriginalTexto.x, 0f, 1f);
            textoVictoria.color = Color.white;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / duracionAnimacion;
                float suavizado = Mathf.SmoothStep(0f, 1f, t);

                rtTexto.localScale = new Vector3(escalaOriginalTexto.x, Mathf.Lerp(0f, escalaOriginalTexto.y, suavizado), 1f);
                textoVictoria.color = Color.Lerp(Color.white, colorOriginalTexto, suavizado);

                yield return null;
            }

            rtTexto.localScale = new Vector3(escalaOriginalTexto.x, escalaOriginalTexto.y, 1f);
            textoVictoria.color = colorOriginalTexto;
        }

        if (imagen == null) yield break;

        Vector2 escalaFinal = ganador == 0 ? escalaOriginalP1 : escalaOriginalP2;
        Color colorFinal = ganador == 0 ? colorOriginalP1 : colorOriginalP2;

        imagen.gameObject.SetActive(true);

        RectTransform rt = imagen.rectTransform;
        Sprite spriteOriginal = imagen.sprite;

        rt.localScale = new Vector3(escalaFinal.x, 0f, 1f);
        imagen.sprite = null;
        imagen.color = Color.white;

        float tImagen = 0f;
        while (tImagen < 1f)
        {
            tImagen += Time.deltaTime / duracionAnimacion;
            float suavizado = Mathf.SmoothStep(0f, 1f, tImagen);

            rt.localScale = new Vector3(escalaFinal.x, Mathf.Lerp(0f, escalaFinal.y, suavizado), 1f);
            imagen.color = Color.Lerp(Color.white, colorFinal, suavizado);

            yield return null;
        }

        rt.localScale = new Vector3(escalaFinal.x, escalaFinal.y, 1f);
        imagen.color = colorFinal;
        imagen.sprite = spriteOriginal;

        corrutinaAnimacion = null;
    }
}
