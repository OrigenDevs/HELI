using System.Collections;
using UnityEngine;

public class CartaMatch : MonoBehaviour
{
    [Header("Caras")]
    public string nombreCara = "Cara";
    public string nombreReverso = "Reverso";
    public string nombreResaltado = "Outline";

    [Header("Volteo")]
    public float tiempoVolteo = 0.45f;
    [Tooltip("Grosor de la carta durante el volteo (1 = normal, 0 = filo).")]
    public AnimationCurve curvaGrosor = new AnimationCurve(
        new Keyframe(0f, 1f), new Keyframe(0.5f, 0.15f), new Keyframe(1f, 1f));
    [Tooltip("Altura extra durante el volteo (squash/stretch).")]
    public AnimationCurve curvaAltura = new AnimationCurve(
        new Keyframe(0f, 1f), new Keyframe(0.5f, 1.35f), new Keyframe(1f, 1f));

    public Sprite spriteCara { get; private set; }
    public bool volteada { get; private set; }
    public bool emparejada { get; private set; }

    private Coroutine corrutinaVolteo;

    public void AsignarCara(Sprite sprite)
    {
        spriteCara = sprite;
        SpriteRenderer cara = BuscarSpriteRenderer(nombreCara);
        if (cara != null) cara.sprite = sprite;
    }

    public void Voltear()
    {
        if (emparejada) return;
        if (corrutinaVolteo != null) StopCoroutine(corrutinaVolteo);
        corrutinaVolteo = StartCoroutine(AnimarVolteo());
    }

    public void Emparejar()
    {
        emparejada = true;
    }

    public void MostrarResaltado(bool mostrar)
    {
        Transform resaltado = !string.IsNullOrEmpty(nombreResaltado) ? transform.Find(nombreResaltado) : null;
        if (resaltado != null) resaltado.gameObject.SetActive(mostrar);
    }

    IEnumerator AnimarVolteo()
    {
        Quaternion cerrada = Quaternion.Euler(90f, 0f, 0f);
        Quaternion abierta = Quaternion.Euler(-90f, -90f, -90f);
        Quaternion desde = volteada ? abierta : cerrada;
        Quaternion hacia = volteada ? cerrada : abierta;

        Vector3 escalaBase = transform.localScale;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / tiempoVolteo;

            float rot = Mathf.SmoothStep(0f, 1f, t);
            transform.localRotation = Quaternion.Slerp(desde, hacia, rot);

            float grosor = curvaGrosor.Evaluate(t);
            float altura = curvaAltura.Evaluate(t);
            transform.localScale = new Vector3(
                escalaBase.x * grosor,
                escalaBase.y * altura,
                escalaBase.z * grosor);

            yield return null;
        }

        transform.localRotation = hacia;
        transform.localScale = escalaBase;
        volteada = !volteada;
        corrutinaVolteo = null;
    }

    SpriteRenderer BuscarSpriteRenderer(string nombre)
    {
        Transform cara = !string.IsNullOrEmpty(nombre) ? transform.Find(nombre) : null;
        if (cara == null) cara = transform;
        return cara.GetComponent<SpriteRenderer>();
    }
}
