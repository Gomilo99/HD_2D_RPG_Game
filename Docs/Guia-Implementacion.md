# Guía de implementación

## Estructura técnica principal
- `Assets/Game/Scripts/`
  - `PlayerController.cs`: movimiento y entrada del jugador.
  - `PlayerAnimationController.cs`: control de animaciones del jugador.
  - `Combat/`: módulos del sistema de combate.
    - `AI/`: decisiones de combate.
    - `Actions/`: acciones ejecutables (ataque, defensa, habilidades, etc.).
    - `Core/`: orquestación del combate.
    - `Data/`: definiciones de datos (ScriptableObjects).
    - `Effects/`: efectos aplicados en combate.
    - `Interfaces/`: contratos compartidos.
    - `UI/`: control de UI de combate.
  - `Progression/`: sistema de nivel y loot.
    - `CharacterLevel.cs`: componente de nivel por personaje.
    - `LevelGrowthTable.cs`: ScriptableObject de curva de progresión.
    - `LootTable.cs`: ScriptableObject de loot por enemigo.
  - `Inventory/`: inventario y datos del jugador.
    - `PlayerInventory.cs`: singleton de consumibles y equipamiento.
    - `PlayerData.cs`: singleton de equipo y dinero.
    - `EquipmentData.cs`: ScriptableObject de equipamiento.
  - `Save/`: sistema de guardado.
    - `SaveManager.cs`: servicio principal (slots, JSON, checksum).
    - `SaveData.cs`: modelo de datos serializable.
    - `SaveCheckpoint.cs`: punto de guardado interactuable.
    - `ICheckpointProvider.cs`: interfaz de posición/escena.
  - `Lighting/`: gestión de iluminación y atmósfera.
    - `AtmosphereManager.cs`: transición entre presets (singleton).
    - `LightingPresetData.cs`: ScriptableObject de preset de atmósfera.
  - `World/`: elementos del mundo (patrulla, interactuables, tienda, transición).
    - `EnemyPatrolController.cs`: IA de patrulla y persecución.
    - `InteractableChest.cs`: cofre con recompensa real.
    - `ShopNPC.cs`: NPC de tienda.
    - `SceneTransitionManager.cs`: transición con fundido entre escenas.
    - `PlayerInteractionDetector.cs`: detector de IInteractable para el jugador.
    - `IInteractable.cs`: contrato para objetos interactuables del mundo.
  - `UX/`: feedback y telemetría.
    - `CombatFeedbackService.cs`: flash de daño, SFX, VFX.
    - `TelemetryService.cs`: métricas de sesión.

## Guías detalladas por sistema
Ver la carpeta `Docs/Guias/` para guías con corrida en frío de cada sistema:
- `SistemaDeCombate.md`
- `SistemaDeProgresion.md`
- `SistemaDeGuardado.md`
- `SistemaDeMundo.md`
- `SistemaDeIluminacion.md`
- `SistemaDeEconomia.md`

## Datos de combate
Los datos de habilidades, items y estadísticas están definidos en:
- `Assets/Game/Scripts/Combat/Data/`
  - `AbilityData.cs`
  - `ItemData.cs`
  - `CharacterStats.cs`

Crea ScriptableObjects en una carpeta de datos (por ejemplo `Assets/Game/Data/`) para mantener los assets configurables.

## Input
- Archivo principal de acciones: `Assets/InputSystem_Actions.inputactions`.
- Mantén las asignaciones de input centralizadas en este archivo.
- Añadir eje `"Interact"` en Project Settings → Input Manager (tecla E) para el detector de interactuables.

## Prefabs y escenas
- Prefabs: `Assets/Game/Prefabs/`
- Escenas: `Assets/Game/Scenes/`

## Reglas básicas de organización
- Cada sistema nuevo debe vivir bajo `Assets/Game/Scripts/<Sistema>/`.
- Evita mezclar recursos propios con terceros; todo lo externo va en `Assets/00-Thirds/`.
- Versiona cualquier asset crítico (prefabs, data, escenas) junto a su `.meta`.
- Patrones: SOLID, programar contra interfaces, singletons para servicios globales.
- Documentación: comentarios de clase en español, variables en inglés.
