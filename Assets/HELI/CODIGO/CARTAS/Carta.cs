using UnityEngine;
 
[CreateAssetMenu(fileName = "NuevaCarta", menuName = "Cartas/Carta")]
public class Carta : ScriptableObject
{
    [Header("Datos de la Carta")]
    public int valor;
    public Texture2D imagen;
}