using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AlphaButton : MonoBehaviour
{
    public float alphaThreshold = 0.1f; // Umbral mínimo para considerar clic "válido"

    void Start()
    {
        this.GetComponent<Image>().alphaHitTestMinimumThreshold = alphaThreshold;
    }
}
