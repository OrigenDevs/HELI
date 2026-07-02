using UnityEngine;

public class Activar : MonoBehaviour
{
    public GameObject[] objetos;

    public void ActivarObjetos()
    {
        foreach (var o in objetos)
            if (o != null) o.SetActive(true);
    }

    public void DesactivarObjetos()
    {
        foreach (var o in objetos)
            if (o != null) o.SetActive(false);
    }
}
