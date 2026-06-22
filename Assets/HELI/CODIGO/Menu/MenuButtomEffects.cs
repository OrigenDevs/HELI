using UnityEngine;

public class MenuButton3D : MonoBehaviour
{
    [Header("Enfoque de Cámara")]
    public Transform puntoCamara;

    [Header("Efecto Visual Principal")]
    public Transform modeloVisual;
    public Vector3 escalaNormal = Vector3.one;
    public Vector3 escalaSeleccionado = new Vector3(1.15f, 1.15f, 1.15f);

    [Header("Efecto MvC3 (Mismo tamaño que el Padre)")]
    public Renderer[] ghostRenderers;

    [Tooltip("Hacia adelante (eje Y local en tu configuración).")]
    public float empujeHaciaCamaraY = 2.5f;
    [Tooltip("Desfase diagonal en X mientras avanza.")]
    public float desvioDiagonalX = 0.3f;
    [Tooltip("Desfase diagonal en Z mientras avanza.")]
    public float desvioDiagonalZ = 0.1f;

    public float intensidadBurst = 1.8f;

    [Header("Renderers del Material")]
    public Renderer meshRenderer;
    public float velocidadCambio = 12f;

    [Header("Configuración de Destino")]
    [Tooltip("Escribe el nombre exacto de la escena que debe cargar este botón específico.")]
    public string nombreEscenaDestino;

    private bool seleccionado = false;
    private Material miMaterial;
    private Material[] ghostMateriales;
    private float targetFade = 1f;
    private float targetGrey = 1f;
    private float factorBurstActual = 0f;

    private Vector3[] posOriginalesGhosts;

    void Start()
    {
        if (meshRenderer != null) miMaterial = meshRenderer.material;

        if (ghostRenderers != null && ghostRenderers.Length > 0)
        {
            ghostMateriales = new Material[ghostRenderers.Length];
            posOriginalesGhosts = new Vector3[ghostRenderers.Length];

            for (int i = 0; i < ghostRenderers.Length; i++)
            {
                if (ghostRenderers[i] != null)
                {
                    ghostMateriales[i] = ghostRenderers[i].material;
                    ghostMateriales[i].SetFloat("_FadeAmount", 1f);
                    ghostMateriales[i].renderQueue = 3050;

                    posOriginalesGhosts[i] = ghostRenderers[i].transform.localPosition;
                    ghostRenderers[i].enabled = false;
                }
            }
        }
    }

    void Update()
    {
        if (modeloVisual != null)
        {
            Vector3 escalaObjetivo = seleccionado ? escalaSeleccionado : escalaNormal;
            modeloVisual.localScale = Vector3.Lerp(modeloVisual.localScale, escalaObjetivo, Time.deltaTime * velocidadCambio);
        }

        if (miMaterial != null)
        {
            float actualFade = miMaterial.GetFloat("_FadeAmount");
            float actualGrey = miMaterial.GetFloat("_ChangeToGrey");
            miMaterial.SetFloat("_FadeAmount", Mathf.Lerp(actualFade, targetFade, Time.deltaTime * velocidadCambio));
            miMaterial.SetFloat("_ChangeToGrey", Mathf.Lerp(actualGrey, targetGrey, Time.deltaTime * velocidadCambio));
        }

        AnimarFantasmas();
    }

    public void Seleccionar()
    {
        seleccionado = true;
        targetFade = 0f;
        targetGrey = 0f;
    }

    public void Deseleccionar()
    {
        seleccionado = false;
        targetFade = 1f;
        targetGrey = 1f;
    }

    public void EjecutarConfirmacion()
    {
        factorBurstActual = intensidadBurst;

        if (ghostRenderers != null)
        {
            for (int i = 0; i < ghostRenderers.Length; i++)
            {
                if (ghostRenderers[i] != null) ghostRenderers[i].enabled = true;
            }
        }
    }

    private void AnimarFantasmas()
    {
        factorBurstActual = Mathf.Lerp(factorBurstActual, 0f, Time.deltaTime * 5f);

        if (ghostRenderers == null || ghostRenderers.Length == 0) return;

        float progresoEfecto = 1f - (factorBurstActual / intensidadBurst);

        for (int i = 0; i < ghostRenderers.Length; i++)
        {
            if (ghostRenderers[i] == null) continue;

            Transform ghostTransform = ghostRenderers[i].transform;

            if (factorBurstActual > 0.01f)
            {
                float separacionCapa = (i + 1);

                float avanceY = empujeHaciaCamaraY * progresoEfecto * separacionCapa;
                float desvioX = desvioDiagonalX * progresoEfecto * separacionCapa;
                float desvioZ = (desvioDiagonalZ - (i * 0.05f)) * progresoEfecto;

                ghostTransform.localPosition = posOriginalesGhosts[i] + new Vector3(desvioX, avanceY, desvioZ);
                ghostTransform.localScale = Vector3.one;

                if (ghostMateriales[i] != null)
                {
                    ghostMateriales[i].SetFloat("_FadeAmount", Mathf.Clamp01(progresoEfecto));
                    if (miMaterial != null)
                    {
                        ghostMateriales[i].SetFloat("_ChangeToGrey", miMaterial.GetFloat("_ChangeToGrey"));
                    }
                }
            }
            else
            {
                ghostTransform.localPosition = posOriginalesGhosts[i];
                ghostTransform.localScale = Vector3.one;

                if (ghostMateriales[i] != null)
                {
                    ghostMateriales[i].SetFloat("_FadeAmount", 1f);
                    ghostRenderers[i].enabled = false;
                }
            }
        }
    }
}