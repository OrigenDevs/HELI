using UnityEngine;
using UnityEngine.Events;

public class triggerZoneEvent : MonoBehaviour
{
    public UnityEvent alEntrar;
    public UnityEvent alSalir;

    void OnTriggerEnter2D(Collider2D other)
    {
        alEntrar?.Invoke();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        alSalir?.Invoke();
    }
}