using UnityEngine;
using UnityEngine.Events;

public class OnEnableEvent : MonoBehaviour
{
    [Header("Eventos")]
    public UnityEvent onEnable;
    public UnityEvent onDisable;

    void OnEnable()
    {
        onEnable.Invoke();
    }

    void OnDisable()
    {
        onDisable.Invoke();
    }
}
