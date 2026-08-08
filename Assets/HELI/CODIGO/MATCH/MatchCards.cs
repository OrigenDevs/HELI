using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MatchCards : MonoBehaviour
{
    [Header("Cartas")]
    public List<Sprite> sprites;
    public GameObject prefabCarta;
    public int filas = 4;
    public int columnas = 4;
    public float separacionX = 1.2f;
    public float separacionZ = 1.2f;
    public Vector3 centroTablero = Vector3.zero;
    public Transform contenedorCartas;
    public string nombreCara = "Cara";

    [Header("Logica")]
    public float tiempoEspera = 0.6f;
    public float tiempoMuestra = 0.8f;

    public UnityEvent onVictoria;

    public Transform[,] cartas;

    private CartaMatch primera;
    private CartaMatch segunda;
    private bool comprobando;
    private int paresEncontrados;

    public Vector2Int TamanoGrid => new Vector2Int(filas, columnas);

    void Start()
    {
        CrearTablero();
    }

    [ContextMenu("Crear Tablero")]
    public void CrearTablero()
    {
        if (prefabCarta == null)
        {
            Debug.LogError("MatchCards: asignale el prefabCarta.");
            return;
        }

        LimpiarTablero();

        int total = filas * columnas;
        if (total % 2 != 0)
        {
            Debug.LogError("MatchCards: filas * columnas debe ser par.");
            return;
        }

        if (sprites.Count < total / 2)
        {
            Debug.LogError("MatchCards: faltan sprites para llenar el tablero. Necesitas al menos " + total / 2 + ".");
            return;
        }

        List<Sprite> caras = new List<Sprite>();
        for (int i = 0; i < total / 2; i++)
        {
            caras.Add(sprites[i]);
            caras.Add(sprites[i]);
        }
        Barajar(caras);

        Transform cont = contenedorCartas != null ? contenedorCartas : transform;
        cartas = new Transform[filas, columnas];

        int indice = 0;
        for (int f = 0; f < filas; f++)
        {
            for (int c = 0; c < columnas; c++)
            {
                GameObject instancia = Instantiate(prefabCarta, cont);
                instancia.name = "Carta " + f + "," + c;
                instancia.transform.localPosition = PosicionLocal(f, c);
                AsignarCara(instancia, caras[indice]);
                cartas[f, c] = instancia.transform;
                indice++;
            }
        }
    }

    Vector3 PosicionLocal(int f, int c)
    {
        float x = (c - (columnas - 1) * 0.5f) * separacionX;
        float z = (f - (filas - 1) * 0.5f) * separacionZ;
        return centroTablero + new Vector3(x, 0f, z);
    }

    public Vector3 PosicionCarta(Vector2Int coord)
    {
        Transform carta = CartaEn(coord);
        return carta != null ? carta.position : transform.position;
    }

    public Transform CartaEn(Vector2Int coord)
    {
        if (cartas == null || coord.x < 0 || coord.y < 0 || coord.x >= filas || coord.y >= columnas)
            return null;
        return cartas[coord.x, coord.y];
    }

    public void Resaltar(Vector2Int coord, bool mostrar)
    {
        Transform t = CartaEn(coord);
        if (t == null) return;

        CartaMatch carta = t.GetComponent<CartaMatch>();
        if (carta != null) carta.MostrarResaltado(mostrar);
    }

    public void Seleccionar(Vector2Int coord)
    {
        if (comprobando) return;

        Transform t = CartaEn(coord);
        if (t == null) return;

        CartaMatch carta = t.GetComponent<CartaMatch>();
        if (carta == null) return;
        if (carta.volteada || carta.emparejada) return;

        carta.Voltear();

        if (primera == null)
        {
            primera = carta;
            return;
        }

        segunda = carta;
        comprobando = true;
        StartCoroutine(ComprobarPareja());
    }

    IEnumerator ComprobarPareja()
    {
        yield return new WaitForSeconds(tiempoEspera);

        if (primera.spriteCara == segunda.spriteCara)
        {
            primera.Emparejar();
            segunda.Emparejar();
            paresEncontrados++;
            if (paresEncontrados >= filas * columnas / 2)
                onVictoria.Invoke();
        }
        else
        {
            yield return new WaitForSeconds(tiempoMuestra);
            primera.Voltear();
            segunda.Voltear();
        }

        primera = null;
        segunda = null;
        comprobando = false;
    }

    void AsignarCara(GameObject carta, Sprite sprite)
    {
        CartaMatch cartaMatch = carta.GetComponent<CartaMatch>();
        if (cartaMatch != null)
        {
            cartaMatch.AsignarCara(sprite);
            return;
        }

        Transform cara = !string.IsNullOrEmpty(nombreCara) ? carta.transform.Find(nombreCara) : null;
        if (cara == null) cara = carta.transform;

        SpriteRenderer sr = cara.GetComponent<SpriteRenderer>();
        if (sr != null) sr.sprite = sprite;
    }

    void Barajar(List<Sprite> lista)
    {
        for (int i = lista.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Sprite temp = lista[i];
            lista[i] = lista[j];
            lista[j] = temp;
        }
    }

    void LimpiarTablero()
    {
        if (cartas != null)
        {
            foreach (Transform t in cartas)
                if (t != null) Destroy(t.gameObject);
            cartas = null;
        }

        Transform cont = contenedorCartas != null ? contenedorCartas : transform;
        while (cont.childCount > 0)
            Destroy(cont.GetChild(0).gameObject);

        primera = null;
        segunda = null;
        comprobando = false;
        paresEncontrados = 0;
    }
}
