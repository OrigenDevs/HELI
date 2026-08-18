using UnityEngine;

public class DefinirOrientacionDeAndroid : MonoBehaviour
{
    public enum Orientacion { Portrait, Landscape }

    public Orientacion orientacion = Orientacion.Portrait;

    void Awake()
    {
        switch (orientacion)
        {
            case Orientacion.Portrait:
                Screen.orientation = ScreenOrientation.Portrait;
                break;
            case Orientacion.Landscape:
                Screen.orientation = ScreenOrientation.LandscapeLeft;
                break;
        }
    }
}