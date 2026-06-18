# CATÁLOGO DE SCRIPTS

Todos los scripts están en el **namespace global** (sin namespace). Usan el **New Input System**.

---

## Scripts Compartidos / Utilidades

| Archivo | Ruta | Propósito |
|---|---|---|
| `LoadScene.cs` | `CODIGO/2D/LoadScene.cs` | Carga escena por nombre desde botón. Usado en HOME y otras escenas. |
| `RestartScene.cs` | `CODIGO/2D/RestartScene.cs` | Recarga la escena actual desde botón. |
| `DestroyObjects.cs` | `CODIGO/2D/DestroyObjects.cs` | Destruye objetos y desactiva Canvas. |
| `OnEnableToggle.cs` | `CODIGO/2D/OnEnableToggle.cs` | Toggle de objetos al activarse el componente. |
| `DesactivarTiempo.cs` | `CODIGO/3D/DesactivarTiempo.cs` | Desactiva o destruye objeto tras un tiempo. |
| `FollowObject.cs` | `CODIGO/2D/FollowObject.cs` | Sigue un target con control por eje y suavizado. |
| `TriggerZone.cs` | `CODIGO/2D/TriggerZone.cs` | Trigger genérico que activa/desactiva objetos. |

---

## Sistema de Input

| Archivo | Clase | Propósito |
|---|---|---|
| `InputManager.cs` | `InputManager` (singleton), `PlayerInputData`, `InputMode` | Enruta input desde New Input System. Soporta teclado, gamepad, táctil. Bloqueo de input por estado del juego. |
| `PlayerInputActions.cs` | `PlayerInputActions` | Clase generada automáticamente del asset `.inputactions`. Acciones: Move, Look, Jump, Attack, Interact, Crouch, Sprint, Previous, Next. |
| `InputSystem_Actions.inputactions` | (Asset) | Archivo de configuración de acciones de input en la raíz de Assets. |

---

## Scripts de Editor (carpeta Assets/Editor/)

| Archivo | Propósito |
|---|---|
| `AutoSaveScene.cs` | Guarda la escena activa automáticamente cada 5 minutos. Menú: Tools/Auto-Save/ |
| `InspectorCleanup.cs` | Limpia la selección del Inspector al salir de Play Mode (evita errores). |

---

## Plugins WebGL

| Archivo | Propósito |
|---|---|
| `Assets/Plugins/WebGL/IsMobileDevice.jslib` | Plugin JavaScript para detectar si el dispositivo es móvil desde WebGL. Usado por `DeviceDetector.cs`. |

---

## Packages Clave (de manifest.json)

| Paquete | Versión | Uso |
|---|---|---|
| `com.unity.inputsystem` | 1.18.0 | Nuevo sistema de input |
| `com.unity.render-pipelines.universal` | 17.3.0 | URP |
| `com.unity.2d.sprite` | 1.0.0 | Sprites 2D |
| `com.unity.ai.navigation` | 2.0.9 | NavMesh/Navegación |
| `com.unity.ugui` | 2.0.0 | UI system |
| `com.unity.visualscripting` | 1.9.9 | Visual scripting |
| `com.unity.timeline` | 1.8.10 | Timeline |
| `com.unity.collab-proxy` | 2.11.2 | Collaboración |

---

## Notas para OpenCode

- **Para añadir una nueva escena**: crear en `Assets/HELI/ESCENAS/` y registrar en `File > Build Settings`.
- **Para crear nuevas cartas**: usar el menú `Assets > Create > Carta` (por `[CreateAssetMenu]` en `Carta.cs`).
- **Las escenas se cargan por nombre** con `SceneManager.LoadScene("Nombre")`. Verificar que el nombre coincida exactamente con el archivo .unity.
- **El InputManager es singleton**: acceder via `InputManager.Instance`.
- **Perfiles de build**: `Assets/Settings/Build Profiles/Web - Desktop - Release.asset` para WebGL.
