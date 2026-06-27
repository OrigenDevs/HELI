using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BotonManager : MonoBehaviour
{
    [Header("Botones del grupo")]
    public Button[] botones;

    [Header("Escala al seleccionar")]
    public float escalaSeleccionado = 1.15f;

    private Vector3[] escalasOriginales;
    private Color[] coloresOriginales;
    private bool inicializado = false;

    void Awake()
    {
        Inicializar();
    }

    void OnEnable()
    {
        if (!inicializado || botones == null || botones.Length == 0) return;
        StartCoroutine(SeleccionarPrimerBoton());
    }

    private System.Collections.IEnumerator SeleccionarPrimerBoton()
    {
        yield return null;
        if (EventSystem.current == null || botones[0] == null) yield break;
        if (botones[0].gameObject.activeInHierarchy)
            EventSystem.current.SetSelectedGameObject(botones[0].gameObject);
    }

    private void Inicializar()
    {
        escalasOriginales = new Vector3[botones.Length];
        coloresOriginales = new Color[botones.Length];

        for (int i = 0; i < botones.Length; i++)
        {
            if (botones[i] == null) continue;

            escalasOriginales[i] = botones[i].transform.localScale;
            coloresOriginales[i] = botones[i].colors.normalColor;

            var trigger = botones[i].gameObject.AddComponent<EventTrigger>();
            trigger.triggers.Clear();

            var selectEntry = new EventTrigger.Entry();
            selectEntry.eventID = EventTriggerType.Select;
            int idx = i;
            selectEntry.callback.AddListener((data) => OnSeleccionado(idx));
            trigger.triggers.Add(selectEntry);

            var deselectEntry = new EventTrigger.Entry();
            deselectEntry.eventID = EventTriggerType.Deselect;
            deselectEntry.callback.AddListener((data) => OnDeseleccionado(idx));
            trigger.triggers.Add(deselectEntry);
        }

        inicializado = true;
    }

    private void OnSeleccionado(int index)
    {
        if (index < 0 || index >= botones.Length) return;

        for (int i = 0; i < botones.Length; i++)
        {
            if (botones[i] == null) continue;

            if (i == index)
            {
                botones[i].transform.localScale = escalasOriginales[i] * escalaSeleccionado;
                var colors = botones[i].colors;
                colors.normalColor = coloresOriginales[i];
                botones[i].colors = colors;
            }
            else
            {
                botones[i].transform.localScale = escalasOriginales[i];
                var colors = botones[i].colors;
                colors.normalColor = Color.gray;
                botones[i].colors = colors;
            }
        }
    }

    private void OnDeseleccionado(int index)
    {
        if (index < 0 || index >= botones.Length || botones[index] == null) return;

        botones[index].transform.localScale = escalasOriginales[index];
        var colors = botones[index].colors;
        colors.normalColor = coloresOriginales[index];
        botones[index].colors = colors;
    }
}
