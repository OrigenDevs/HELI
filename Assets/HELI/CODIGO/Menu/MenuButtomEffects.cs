using UnityEngine;

public class MenuButton3D : MonoBehaviour
{
    [Header("Enfoque de Cámara")]
    public Transform puntoCamara;

    [Header("Efecto Visual Principal")]
    public Transform modeloVisual;
    public Vector3 escalaNormal = Vector3.one;
    public Vector3 escalaSeleccionado = new Vector3(1.15f, 1.15f, 1.15f);

    [Header("Efecto MvC3 (Afterimages)")]
    [Tooltip("Arrastra aquí 2 o 3 duplicados del modelo visual que actuarán como ecos.")]
    public Renderer[] ghostRenderers;
    [Tooltip("Dirección hacia la que saldrán disparados los fantasmas (X, Y, Z).")]
    public Vector3 direccionDesfase = new Vector3(-0.5f, 0.3f, 0.1f);
    [Tooltip("Fuerza del estallido inicial.")]
    public float intensidadBurst = 1.5f;

    [Header("Renderers del Material")]
    public Renderer meshRenderer;
    public float velocidadCambio = 12f;

    private bool seleccionado = false;
    private Material miMaterial;
    private Material[] ghostMateriales;

    // Control de animación manual
    private float targetFade = 1f;
    private float targetGrey = 1f;
    private float factorBurstActual = 0f;

    void Start()
    {
        if (meshRenderer != null) miMaterial = meshRenderer.material;

        // Inicializamos los materiales de los fantasmas para que sean independientes
        if (ghostRenderers != null && ghostRenderers.Length > 0)
        {
            ghostMateriales = new Material[ghostRenderers.Length];
            for (int i = 0; i < ghostRenderers.Length; i++)
            {
                if (ghostRenderers[i] != null)
                {
                    ghostMateriales[i] = ghostRenderers[i].material;
                    // Empezan invisibles/oscuros
                    ghostMateriales[i].SetFloat("_FadeAmount", 1f);
                }
            }
        }
    }

    void Update()
    {
        // 1. Escalado suave del botón principal
        if (modeloVisual != null)
        {
            Vector3 escalaObjetivo = seleccionado ? escalaSeleccionado : escalaNormal;
            modeloVisual.localScale = Vector3.Lerp(modeloVisual.localScale, escalaObjetivo, Time.deltaTime * velocidadCambio);
        }

        // 2. Transición del Shader en el botón principal
        if (miMaterial != null)
        {
            float actualFade = miMaterial.GetFloat("_FadeAmount");
            float actualGrey = miMaterial.GetFloat("_ChangeToGrey");
            miMaterial.SetFloat("_FadeAmount", Mathf.Lerp(actualFade, targetFade, Time.deltaTime * velocidadCambio));
            miMaterial.SetFloat("_ChangeToGrey", Mathf.Lerp(actualGrey, targetGrey, Time.deltaTime * velocidadCambio));
        }

        // 3. Animación manual estilo Marvel vs Capcom 3 para los fantasmas
        AnimarFantasmas();
    }

    public void Seleccionar()
    {
        seleccionado = true;
        targetFade = 0f;
        targetGrey = 0f;

        // ¡Boom! Activamos el estallido de las imágenes fantasma
        factorBurstActual = intensidadBurst;
    }

    public void Deseleccionar()
    {
        seleccionado = false;
        targetFade = 1f;
        targetGrey = 1f;
    }

    private void AnimarFantasmas()
    {
        // El factor de estallido va disminuyendo frame a frame regresando a 0
        factorBurstActual = Mathf.Lerp(factorBurstActual, 0f, Time.deltaTime * velocidadCambio);

        if (ghostRenderers == null || ghostRenderers.Length == 0) return;

        for (int i = 0; i < ghostRenderers.Length; i++)
        {
            if (ghostRenderers[i] == null) continue;

            Transform ghostTransform = ghostRenderers[i].transform;

            if (seleccionado && factorBurstActual > 0.01f)
            {
                // Cada fantasma (i) se desfasa un poco más que el anterior multiplicando el índice
                float multiplicadorCapa = (i + 1) * 0.6f;
                Vector3 posicionDesfasada = modeloVisual.localPosition + (direccionDesfase * factorBurstActual * multiplicadorCapa);

                ghostTransform.localPosition = posicionDesfasada;
                ghostTransform.localScale = modeloVisual.localScale * (1f + (factorBurstActual * 0.1f * multiplicadorCapa));

                // Los hacemos totalmente visibles y a color durante el estallido
                if (ghostMateriales[i] != null)
                {
                    ghostMateriales[i].SetFloat("_FadeAmount", 0f);
                    ghostMateriales[i].SetFloat("_ChangeToGrey", 0f);
                }
            }
            else
            {
                // Si no está seleccionado o el estallido terminó, regresan al centro del botón y se apagan
                ghostTransform.localPosition = Vector3.Lerp(ghostTransform.localPosition, modeloVisual.localPosition, Time.deltaTime * velocidadCambio);
                ghostTransform.localScale = Vector3.Lerp(ghostTransform.localScale, modeloVisual.localScale, Time.deltaTime * velocidadCambio);

                if (ghostMateriales[i] != null)
                {
                    float f = ghostMateriales[i].GetFloat("_FadeAmount");
                    ghostMateriales[i].SetFloat("_FadeAmount", Mathf.Lerp(f, 1f, Time.deltaTime * velocidadCambio)); // Se desvanecen a negro
                }
            }
        }
    }
}