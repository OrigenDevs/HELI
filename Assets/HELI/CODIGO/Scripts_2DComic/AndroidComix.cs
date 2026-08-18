using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AndroidComix : MonoBehaviour
{
    [Header("Escena")]
    public bool esVertical;

    [Header("Referencias")]
    public GolpeJugador golpeJugador;
    public MovimientoBEU movimiento;

    [Header("Joystick")]
    public RectTransform areaJoystick;
    public RectTransform palancaJoystick;
    public float radioJoystick = 100f;

    [Header("Botones")]
    public Button botonGolpe;
    public Button botonSuper;

    private Vector2 direccionJoystick;
    private int punteroJoystick = -1;
    private float radioJoystickAbsoluto;

    void Start()
    {
        Application.targetFrameRate = 60;

#if UNITY_ANDROID
        Screen.orientation = esVertical ? ScreenOrientation.Portrait : ScreenOrientation.LandscapeLeft;
#endif

        ConfigurarBotones();
        ConfigurarJoystick();
    }

    void ConfigurarBotones()
    {
        if (botonGolpe != null)
            botonGolpe.onClick.AddListener(() => { if (golpeJugador != null) golpeJugador.PulsarGolpe(); });
        if (botonSuper != null)
            botonSuper.onClick.AddListener(() => { if (golpeJugador != null) golpeJugador.PulsarSuper(); });
    }

    void ConfigurarJoystick()
    {
        if (areaJoystick == null) return;

        radioJoystickAbsoluto = areaJoystick.rect.width * 0.5f;
        if (palancaJoystick != null)
            palancaJoystick.localPosition = Vector3.zero;

        EventTrigger trigger = areaJoystick.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = areaJoystick.gameObject.AddComponent<EventTrigger>();

        AnadirEntrada(trigger, EventTriggerType.PointerDown, data => IniciarJoystick((PointerEventData)data));
        AnadirEntrada(trigger, EventTriggerType.Drag, data => MoverJoystick((PointerEventData)data));
        AnadirEntrada(trigger, EventTriggerType.PointerUp, _ => DetenerJoystick());
        AnadirEntrada(trigger, EventTriggerType.PointerExit, _ => DetenerJoystick());
    }

    void AnadirEntrada(EventTrigger trigger, EventTriggerType tipo, UnityEngine.Events.UnityAction<BaseEventData> accion)
    {
        EventTrigger.Entry entrada = new EventTrigger.Entry();
        entrada.eventID = tipo;
        entrada.callback.AddListener(accion);
        trigger.triggers.Add(entrada);
    }

    void IniciarJoystick(PointerEventData data)
    {
        if (punteroJoystick != -1) return;
        punteroJoystick = data.pointerId;
        if (palancaJoystick != null)
            palancaJoystick.localPosition = Vector3.zero;
        MoverJoystick(data);
    }

    void MoverJoystick(PointerEventData data)
    {
        if (punteroJoystick != data.pointerId) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(areaJoystick, data.position, data.pressEventCamera, out Vector2 localPunto))
        {
            if (localPunto.magnitude > radioJoystickAbsoluto)
                localPunto = localPunto.normalized * radioJoystickAbsoluto;

            if (palancaJoystick != null)
                palancaJoystick.localPosition = localPunto;

            direccionJoystick = localPunto / radioJoystickAbsoluto;
            if (movimiento != null)
                movimiento.SetDireccionManual(direccionJoystick);
        }
    }

    void DetenerJoystick()
    {
        punteroJoystick = -1;
        direccionJoystick = Vector2.zero;
        if (palancaJoystick != null)
            palancaJoystick.localPosition = Vector3.zero;
        if (movimiento != null)
            movimiento.SetDireccionManual(Vector2.zero);
    }
}