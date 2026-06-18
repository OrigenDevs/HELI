using UnityEngine;

public class CardDisplay : MonoBehaviour
{
    [Header("Material de la carta")]
    public Material materialCarta;

    [Header("Objeto 3D de la carta (con Animator)")]
    public GameObject objetoCarta3D;

    private Animator animatorCarta;

    void Awake()
    {
        if (objetoCarta3D != null)
            animatorCarta = objetoCarta3D.GetComponent<Animator>();

        objetoCarta3D.SetActive(false);
    }

    // Asigna la textura al material (albedo + emisivo)
    public void ActualizarMaterial(Texture2D textura)
    {
        if (materialCarta == null || textura == null) return;

        materialCarta.mainTexture = textura;                        // Albedo / color map
        materialCarta.SetTexture("_EmissionMap", textura);         // Emisivo
        materialCarta.EnableKeyword("_EMISSION");
    }

    // Activa el objeto de la carta y reinicia su animación
    public void MostrarCarta()
    {
        // Apagar y encender para reiniciar la animación del Animator
        objetoCarta3D.SetActive(false);
        objetoCarta3D.SetActive(true);

        if (animatorCarta != null)
        {
            animatorCarta.Rebind();
            animatorCarta.Update(0f);
        }
    }

    // Oculta la carta (puede llamarse al terminar la animación via Animation Event si se desea)
    public void OcultarCarta()
    {
        objetoCarta3D.SetActive(false);
    }
}