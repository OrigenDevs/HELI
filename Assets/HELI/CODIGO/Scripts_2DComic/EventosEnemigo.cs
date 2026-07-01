using UnityEngine;

public class EventosEnemigo : MonoBehaviour
{
    public void EventoMuerte()
    {
        GetComponentInParent<Enemigo>().EventoMuerte();
    }
}
