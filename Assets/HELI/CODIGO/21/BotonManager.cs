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

    void Start()
    {
        if (botones == null || botones.Length == 0) return;

        escalasOriginales = new Vector3[botones.Length];
        coloresOriginales = new Color[botones.Length];

        for (int i = 0; i < botones.Length; i++)
        {
            if (botones[i] == null) continue;

            escalasOriginales[i] = botones[i].transform.localScale;
            coloresOriginales[i] = botones[i].colors.normalColor;

            var trigger = botones[i].gameObject.AddComponent<EventTrigger>();

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
    }

    private void OnSeleccionado(int index)
    {
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
        if (botones[index] == null) return;
        botones[index].transform.localScale = escalasOriginales[index];
    }
}
