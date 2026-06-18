# Estructura del Proyecto

## Carpetas Principales en Assets/

### `/HELI/` — Código y contenido del juego
| Carpeta | Contenido |
|---|---|
| `ANIMACIONES/` | Controladores y clips de animación |
| `CODIGO/` | Todos los scripts C# del juego |
| `ESCENAS/` | Escenas del juego (HOME, 2D, 3D, CARTAS) |
| `MATERIALES/` | Materiales de colores básicos y fondos 2D |
| `MODELOS/` | Modelos 3D (FBX), personaje y prefabs |
| `PREFABS/` | Prefabs adicionales (stands, cajas) |
| `SONIDO/` | Música (3 pistas) y efectos de sonido (6 SFX) |
| `SPRITES/` | Sprites 2D (animaciones, escenario, enemigos) |
| `UI/` | Imágenes de interfaz, fuentes, cartas |
| `DOCUMENTACION/` | Esta documentación |

### `/HELI/CODIGO/` — Scripts por escena/módulo

| Carpeta | Escena | Propósito |
|---|---|---|
| `2D/` | 2D.unity | Runner: movimiento, saltos, obstáculos, cámara |
| `3D/` | 3D.unity | Recolección 3D: personaje, gestión, diálogos, input |
| `21/` | CARTAS.unity | Blackjack: mánager, jugador, IA, cartas, UI |
| `CARTAS/` | CARTAS.unity | Datos de cartas (ScriptableObjects) y prefab |
| `MONTACARGA/` | 3D.unity | Animaciones de oscilación (forklift) |

### Otras carpetas en Assets/

| Carpeta | Propósito |
|---|---|
| `CREHANA/` | Activos 3D del entorno (modelos, prefabs, materiales) |
| `CartoonVFX9x/` | Efectos visuales estilo cómic (explosiones, golpes) |
| `Editor/` | Scripts editor: auto-guardado, limpieza de inspector |
| `Settings/` | Perfiles URP, perfiles de build, volúmenes globales |
| `Plugins/WebGL/` | Plugin JS para detección de dispositivos móviles |
| `TextMesh Pro/` | Sistema de texto TMP (fuentes, shaders, ejemplos) |
