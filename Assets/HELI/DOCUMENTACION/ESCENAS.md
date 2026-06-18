# DESCRIPCIÓN DE ESCENAS

## 1. HOME.unity — Menú Principal

Pantalla de inicio del videojuego. Escena de carga inicial desde la que se navega al resto de escenas.

**Mecánica**: Botones que cargan escenas por nombre mediante `LoadScene.cs`.

**Scripts relevantes:**
- `LoadScene.cs` — Asigna automáticamente la escena a cargar al botón onClick.

---

## 2. CARTAS.unity — Juego de 21 (Blackjack)

Juego de cartas 21 (Blackjack) contra una inteligencia artificial.

**Mecánica:**
- Baraja de 48 cartas (4 palos × 12 valores).
- El jugador pide carta (Hit) o se planta (Stand).
- La IA decide probabilísticamente basado en riesgo.
- Gana quien más se acerque a 21 sin pasarse.

**Scripts clave:**
- `GameManager.cs` (singleton) — Baraja, turnos, detección de victoria/derrota.
- `PlayerController.cs` — Acciones Hit/Stand del jugador, display de cartas.
- `AIController.cs` — IA con decisión probabilística y delay de "pensando".
- `CardDisplay.cs` — Display 3D de carta con textura, animación al mostrar/ocultar.
- `UIManager.cs` — Animación de puntuación con activación progresiva de cajas.
- `Carta.cs` (ScriptableObject) — Datos de carta (valor int, imagen Texture2D).

**Datos:** 48 assets de carta en `Assets/HELI/CODIGO/CARTAS/` (A01-C12, C01-C12, R01-R12, V01-V12).

---

## 3. 3D.unity — Juego de Recolección en Almacén

Juego en 3D en tercera persona donde el jugador recolecta basura/objetos en un entorno de almacén con montacargas.

**Mecánica:**
- Control de personaje en 3D con movimiento, salto y gravedad personalizada.
- Recolección de objetos de basura con barra de progreso.
- Sistema de diálogos con texto y audio sincronizado.
- Condición de victoria al completar la limpieza.

**Scripts clave:**
- `PlayerController2.cs` — Controlador 3D: movimiento, salto, detección de suelo, animaciones, rotación fix para WebGL.
- `GameManagerBasuras.cs` — Puntuación, barra de limpieza, condición de victoria. Contiene clases helper `DetectorBasura` y enum `EjeBarra`.
- `InputManager.cs` (singleton) — Enruta input desde New Input System, soporta teclado/gamepad/táctil, bloqueo de input.
- `DialogueSystem.cs` — Sistema de diálogos con máquina de escribir, audio sincronizado, botón continuar, saltar/repetir.
- `EscaladorYDestructor.cs` — Al trigger: encoge + rota objeto hasta destruirlo.
- `CameraFollow.cs` — Cámara que sigue al personaje suavemente con offset y límites.
- `DeviceDetector.cs` — Detecta móvil vs escritorio (WebGL JS + Android/iOS).

**Animaciones extra:**
- `RotationOscillator.cs`, `OscilacionEscala.cs`, `OscilacionAutomatica.cs` — Oscilaciones para objetos del montacargas.
- `SoxAtkDragTransform.cs` — Sistema de arrastre con delay tipo follow (world-space).

---

## 4. 2D.unity — Juego Runner (Side-Scroller)

Juego 2D de runner automático lateral donde el personaje corre, salta y esquiva obstáculos.

**Mecánica:**
- El personaje corre automáticamente en un eje.
- El jugador salta con tecla/botón (InputSystem).
- Detección de suelo para permitir/restringir salto.
- Obstáculos con tag "Obstaculo" que activan flujo de derrota.
- Enemigos que al chocar activan animación de ataque.

**Scripts clave:**
- `RunnerMovement.cs` — Movimiento automático del runner, salto, detección de suelo, estados de animación (correr/volar).
- `JumpController.cs` — Salto estilo caricatura con multiplicadores de gravedad separados (subida/bajada) y altura máxima.
- `ObstacleHandler.cs` — Detecta colisión con obstáculo (tag "Obstaculo"), activa flujo de derrota con menú retardado.
- `RunnerCameraTracker.cs` — Cámara que sigue al runner en un solo eje con offset y suavizado.
- `EnemyTrigger.cs` — Al trigger con Player: activa/desactiva objetos y reproduce animación de ataque.
- `FollowObject.cs` — Sigue un target con control por eje, offset y suavizado.
- `TriggerZone.cs` — Trigger genérico que activa/desactiva objetos al entrar Player.
- `DestroyObjects.cs` — Destruye GameObjects y desactiva Canvas al inicio o al llamar.
- `OnEnableToggle.cs` — Al activarse, activa/desactiva listas configuradas.
