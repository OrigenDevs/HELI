using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class BotonManager : MonoBehaviour
{
    [Header("Botones del grupo")]
    public Button[] botones;

    [Header("Escala al seleccionar")]
    public float escalaSeleccionado = 1.15f;

    [Header("Animacion de eco")]
    [Tooltip("Activa un pulso de escala repetido en el boton seleccionado.")]
    public bool animacionEco = false;
    public float intensidadEco = 0.05f;
    public float velocidadEco = 6f;

    private Vector3[] escalasOriginales;
    private Color[] coloresOriginales;
    private bool inicializado = false;
    private Coroutine corrutinaEco;
    private InputAction accionJumpInterna;

    void Awake()
    {
        // Crea la acción por código apuntando directamente a "Jump" del mapa por defecto (Keyboard & Mouse / Gamepad)
        accionJumpInterna = new InputAction("JumpAction", binding: "*/{PrimaryAction}");
        // O si prefieres mapearlo directo al espacio/botón de salto estándar:
        // accionJumpInterna.AddBinding("<Keyboard>/space");
        // accionJumpInterna.AddBinding("<Gamepad>/buttonSouth");

        // Alternativa robusta buscando el enlace estándar de Jump:
        accionJumpInterna.AddBinding("<Keyboard>/space");
        accionJumpInterna.AddBinding("<Gamepad>/buttonSouth");

        Inicializar();
    }

    void OnEnable()
    {
        if (accionJumpInterna != null)
            accionJumpInterna.Enable();

        if (!inicializado || botones == null || botones.Length == 0) return;
        StartCoroutine(SeleccionarPrimerBoton());
    }

    void OnDisable()
    {
        if (accionJumpInterna != null)
            accionJumpInterna.Disable();
    }

    void OnDestroy()
    {
        accionJumpInterna?.Dispose();
    }

    void Update()
    {
        if (DetectarSaltoPresionado())
        {
            SimularClickBotonSeleccionado();
        }
    }

    private bool DetectarSaltoPresionado()
    {
        if (accionJumpInterna != null)
        {
            return accionJumpInternalTriggered();
        }
        return false;
    }

    private bool accionJumpInternalTriggered()
    {
        return accionJumpInterna.triggered;
    }

    private void SimularClickBotonSeleccionado()
    {
        if (EventSystem.current == null) return;

        GameObject objetoActual = EventSystem.current.currentSelectedGameObject;
        if (objetoActual != null)
        {
            Button botonActual = objetoActual.GetComponent<Button>();
            if (botonActual != null && botonActual.interactable)
            {
                botonActual.onClick.Invoke();
            }
        }
    }

    private IEnumerator SeleccionarPrimerBoton()
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

        if (animacionEco)
        {
            if (corrutinaEco != null) StopCoroutine(corrutinaEco);
            corrutinaEco = StartCoroutine(AnimacionEco(index));
        }

        BotonEventosUI eventos = botones[index].GetComponent<BotonEventosUI>();
        if (eventos != null) eventos.onEntrar.Invoke();
    }

    private void OnDeseleccionado(int index)
    {
        if (index < 0 || index >= botones.Length || botones[index] == null) return;

        if (animacionEco && corrutinaEco != null)
        {
            StopCoroutine(corrutinaEco);
            corrutinaEco = null;
            botones[index].transform.localScale = escalasOriginales[index];
        }

        var colors = botones[index].colors;
        colors.normalColor = coloresOriginales[index];
        botones[index].colors = colors;

        BotonEventosUI eventos = botones[index].GetComponent<BotonEventosUI>();
        if (eventos != null) eventos.onSalir.Invoke();
    }

    private IEnumerator AnimacionEco(int index)
    {
        while (true)
        {
            float pulso = Mathf.Sin(Time.time * velocidadEco) * intensidadEco;
            if (botones[index] != null)
                botones[index].transform.localScale = escalasOriginales[index] * (escalaSeleccionado + pulso);
            yield return null;
        }
    }
}