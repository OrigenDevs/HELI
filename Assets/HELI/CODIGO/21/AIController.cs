using System.Collections;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [Header("Umbral mínimo para siempre hacer Hit")]
    public int umbralHitSeguro = 11;

    [Header("Tiempo de 'pensar' de la IA (segundos)")]
    public float tiempoPensando = 2f;

    [Header("Objeto 'IA Stand' que se muestra al plantarse")]
    public GameObject objetoIAStand;

    [Header("Tiempo que se muestra el cartel IA Stand (segundos)")]
    public float tiempoIAStand = 2f;

    public void EjecutarTurnoIA()
    {
        StartCoroutine(TurnoIA());
    }

    IEnumerator TurnoIA()
    {
        GameManager gm = GameManager.Instance;

        // IA "piensa"
        yield return new WaitForSeconds(tiempoPensando);

        bool decision = DecidirHit(gm.sumaIA);

        if (decision)
            yield return StartCoroutine(EjecutarHitIA());
        else
            yield return StartCoroutine(EjecutarStandIA());
    }

    // ─────────────────────────────────────────
    //  ALGORITMO DE DECISIÓN
    // ─────────────────────────────────────────
    bool DecidirHit(int sumaActual)
    {
        // Si está por debajo del umbral seguro, siempre hit
        if (sumaActual < umbralHitSeguro) return true;

        // Calcular porcentaje de riesgo según proximidad al 21
        // En 11 → 90% riesgo | En 20 → 0% riesgo
        // Fórmula lineal: riesgo = 90 * (20 - suma) / (20 - 11)
        float riesgo = 90f * (20f - sumaActual) / (20f - umbralHitSeguro);
        riesgo = Mathf.Clamp(riesgo, 0f, 90f);

        float tirada = Random.Range(0f, 100f);
        return tirada < riesgo;
    }

    // ─────────────────────────────────────────
    //  HIT DE LA IA
    // ─────────────────────────────────────────
    IEnumerator EjecutarHitIA()
    {
        GameManager gm = GameManager.Instance;

        // Ya no está plantada
        gm.iaPlantada = false;

        int valor = gm.cartaActual.valor;

        // 1. Mostrar carta 3D
        gm.cardDisplay.MostrarCarta();

        // Esperar delay antes de animar UI
        yield return new WaitForSeconds(gm.uiManager.delayAnimacionCajas);

        // 2. Animar puntos de la IA
        yield return StartCoroutine(
            gm.uiManager.AnimarPuntos(valor, esJugador: false)
        );

        // 3. Sumar internamente
        gm.sumaIA += valor;

        // 4. Eliminar carta y avanzar
        gm.EliminarCartaActualYAvanzar();

        // 5. Verificar estado
        gm.VerificarEstado(esJugador: false);
    }

    // ─────────────────────────────────────────
    //  STAND DE LA IA
    // ─────────────────────────────────────────
    IEnumerator EjecutarStandIA()
    {
        GameManager gm = GameManager.Instance;
        gm.iaPlantada = true;

        // Mostrar cartel IA stand
        if (objetoIAStand != null)
        {
            objetoIAStand.SetActive(true);
            yield return new WaitForSeconds(tiempoIAStand);
            objetoIAStand.SetActive(false);
        }

        // Verificar si ambos plantados
        if (gm.jugadorPlantado && gm.iaPlantada)
            gm.VerificarEstado(esJugador: false);
        else
            gm.CambiarTurno();
    }
}