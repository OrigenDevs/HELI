using System.Collections;
using TMPro;
using UnityEngine;

public class EscalameAnim : MonoBehaviour
{
    [Header("Texto")]
    public TMP_Text texto;

    [Header("Animacion")]
    [Tooltip("Arranca la animacion al activar el GameObject.")]
    public bool animarAlActivar = true;
    [Tooltip("Escala inicial de cada letra (desde afuera hacia adentro).")]
    public float escalaInicio = 2f;
    public float duracionLetra = 0.15f;
    public float retrasoEntreLetras = 0.05f;

    private Coroutine corrutina;

    void OnEnable()
    {
        if (animarAlActivar)
            Animar();
    }

    public void Animar()
    {
        if (corrutina != null) StopCoroutine(corrutina);
        if (texto != null)
            corrutina = StartCoroutine(AnimarLetraPorLetra());
    }

    IEnumerator AnimarLetraPorLetra()
    {
        texto.ForceMeshUpdate();

        int materialCount = texto.textInfo.materialCount;
        Vector3[][] posicionesOriginales = new Vector3[materialCount][];
        Color32[][] coloresOriginales = new Color32[materialCount][];

        for (int m = 0; m < materialCount; m++)
        {
            posicionesOriginales[m] = (Vector3[])texto.textInfo.meshInfo[m].vertices.Clone();
            coloresOriginales[m] = (Color32[])texto.textInfo.meshInfo[m].colors32.Clone();
        }

        int caracteres = texto.textInfo.characterCount;
        for (int i = 0; i < caracteres; i++)
        {
            TMP_CharacterInfo info = texto.textInfo.characterInfo[i];
            if (!info.isVisible) continue;

            int m = info.materialReferenceIndex;
            int v = info.vertexIndex;

            Vector3[] vertices = texto.textInfo.meshInfo[m].vertices;
            Color32[] colores = texto.textInfo.meshInfo[m].colors32;

            Vector3 centro = (posicionesOriginales[m][v] + posicionesOriginales[m][v + 1] +
                              posicionesOriginales[m][v + 2] + posicionesOriginales[m][v + 3]) * 0.25f;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / duracionLetra;
                float suavizado = Mathf.SmoothStep(0f, 1f, t);
                float escala = Mathf.Lerp(escalaInicio, 1f, suavizado);

                for (int k = 0; k < 4; k++)
                {
                    int idx = v + k;
                    vertices[idx] = centro + (posicionesOriginales[m][idx] - centro) * escala;
                    colores[idx] = Color32.Lerp(Color.white, coloresOriginales[m][idx], suavizado);
                }

                texto.UpdateVertexData();
                yield return null;
            }

            for (int k = 0; k < 4; k++)
            {
                int idx = v + k;
                vertices[idx] = posicionesOriginales[m][idx];
                colores[idx] = coloresOriginales[m][idx];
            }

            texto.UpdateVertexData();

            if (retrasoEntreLetras > 0f)
                yield return new WaitForSeconds(retrasoEntreLetras);
        }

        corrutina = null;
    }
}
