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
Los datos (clases ScriptableObject) de habilidades, items y estadísticas están definidos en:
- `Assets/Game/Scripts/Combat/Data/`
  - `AbilityData.cs`
  - `ItemData.cs`
  - `CharacterStats.cs`

Los assets configurables se almacenan en la rama dev en:
- `Assets/Game/Scriptable Objects/`
  - `Abilities/`
  - `Items/`
  - `Players/`
  - `Enemies/`

## Input
- Archivo principal de acciones: `Assets/InputSystem_Actions.inputactions`.
- Mantén las asignaciones de input centralizadas en este archivo.
- Añadir eje `"Interact"` en Project Settings → Input Manager (tecla E) para el detector de interactuables.

## Prefabs y escenas
- Prefabs: `Assets/Game/Prefabs/`
- Escenas: `Assets/Game/Scenes/`

## UI de combate (configuración esencial)
1. Crea un `Canvas` con **EventSystem** activo.
2. Añade paneles: `ActionMenuPanel`, `TargetSelectPanel`, `AbilityMenuPanel`, `ItemMenuPanel`, `OverlayPanel`.
3. Crea un objeto `BattleUIController` y asigna:
   - Referencias a paneles.
   - `messageLogText` con un **Text** (UI).
4. Botones de acciones básicas:
   - **Attack** → `BattleUIController.OnAttackPressed`
   - **Defend** → `BattleUIController.OnDefendPressed`
   - **Flee** → `BattleUIController.OnFleePressed`
5. Botones de habilidades/ítems (configuración adicional):
   - Crea un botón por habilidad/ítem.
   - En **OnClick** asigna:
     - `OnAbilityPressed(AbilityData)` con el asset de habilidad.
     - `OnItemPressed(ItemData)` con el asset de ítem.
6. Selección de objetivos:
   - Crea un botón por objetivo y enlázalo a `OnTargetSelected(BaseCharacter)`.
   - Arrastra el GameObject del objetivo al campo del evento.

## Inicio automático de batalla por colisión (rama dev)
Para arrancar el combate al chocar jugador/enemigo:
1. Añade `PlayerEncounter` (`Assets/Game/Scripts/Combat/Core/PlayerEncounter.cs`) al enemigo.
2. Asigna `combatManager` en el inspector.
3. Asegura que **jugador y enemigo** tengan `Collider` y que al menos uno tenga `Rigidbody` (requisito de `OnCollisionEnter`).
4. En `CombatManager`, deja `autoStart = false` para evitar inicio inmediato.
5. `PlayerEncounter` llama a `CombatManager.SetEnemyToList()` y luego `StartCombat()` cuando ocurre la colisión.

## Reglas básicas de organización
- Cada sistema nuevo debe vivir bajo `Assets/Game/Scripts/<Sistema>/`.
- Evita mezclar recursos propios con terceros; todo lo externo va en `Assets/00-Thirds/`.
- Versiona cualquier asset crítico (prefabs, data, escenas) junto a su `.meta`.
- Patrones: SOLID, programar contra interfaces, singletons para servicios globales.
- Documentación: comentarios de clase en español, variables en inglés.
