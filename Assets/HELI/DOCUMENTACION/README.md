# HELI - Documentación para OpenCode

## Visión General

**HELI** es un videojuego publicitario desarrollado en **Unity 6000.3.6f1** con **URP** (Universal Render Pipeline). Utiliza el **New Input System** de Unity y apunta a plataforma **WebGL** (con detección móvil/escritorio).

El proyecto contiene **4 escenas** que conforman un mini-colección de experiencias de juego conectadas por una temática promocional.

---

## Escenas

| Escena | Ruta | Descripción |
|---|---|---|
| HOME | `Assets/HELI/ESCENAS/HOME.unity` | Menú principal / pantalla de inicio |
| CARTAS | `Assets/HELI/ESCENAS/CARTAS.unity` | Juego de 21 (Blackjack) contra IA |
| 3D | `Assets/HELI/ESCENAS/3D.unity` | Juego 3D de recolección en almacén |
| 2D | `Assets/HELI/ESCENAS/2D.unity` | Juego 2D de runner/side-scroller |

---

## Convenciones del Código

- **Sin namespaces**: todos los scripts están en el namespace global.
- **Input System**: usa `PlayerInputActions` (generado automáticamente) y `InputManager` (singleton).
- **Escenas cargadas por nombre**: los `LoadScene` usan `SceneManager.LoadScene("nombre")`.
- **Canvas autoconectado**: los botones se conectan automáticamente desde el script.

---

## Plataforma Objetivo

- WebGL con perfiles separados para Mobile y PC.
- Plugin `IsMobileDevice.jslib` para detección de móvil desde WebGL.
- Soporte táctil (`TouchCanvas.prefab`).
