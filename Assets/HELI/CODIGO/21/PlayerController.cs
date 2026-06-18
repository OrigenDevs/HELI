using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Llamado desde el botón Hit en la UI
    public void OnHit()
    {
        GameManager gm = GameManager.Instance;
        if (gm.juegoTerminado || !gm.esturnoJugador) return;

        // Ya no está plantado si vuelve a pedir carta
        gm.jugadorPlantado = false;

        gm.uiManager.HabilitarBotonesJugador(false);
        StartCoroutine(EjecutarHit());
    }

    IEnumerator EjecutarHit()
    {
        GameManager gm = GameManager.Instance;
        int valor = gm.cartaActual.valor;

        // 1. Mostrar carta 3D
        gm.cardDisplay.MostrarCarta();

        // Esperar el delay antes de empezar animaciones de UI
        yield return new WaitForSeconds(gm.uiManager.delayAnimacionCajas);

        // 2. Animar puntos en texto y cajas del jugador
        yield return StartCoroutine(
            gm.uiManager.AnimarPuntos(valor, esJugador: true)
        );

        // 3. Sumar internamente al jugador
        gm.sumaJugador += valor;

        // 4. Eliminar carta y avanzar baraja
        gm.EliminarCartaActualYAvanzar();

        // 5. Verificar estado
        gm.VerificarEstado(esJugador: true);
    }

    // Llamado desde el botón Stand en la UI
    public void OnStand()
    {
        GameManager gm = GameManager.Instance;
        if (gm.juegoTerminado || !gm.esturnoJugador) return;

        gm.jugadorPlantado = true;
        gm.uiManager.HabilitarBotonesJugador(false);

        // Verificar si ambos plantados, si no: cambiar turno
        if (gm.jugadorPlantado && gm.iaPlantada)
            gm.VerificarEstado(esJugador: true);
        else
            gm.CambiarTurno();
    }
}