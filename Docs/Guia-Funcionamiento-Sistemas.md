# Guía detallada: cómo funcionan los sistemas

## Visión general
El proyecto implementa un **RPG por turnos** con estos bloques:
- **Movimiento y animación** del jugador en el mundo.
- **Combate por turnos** con acciones, habilidades, ítems, IA y estados.
- **Datos de juego** basados en ScriptableObjects.
- **UI de combate** para selección de acciones y objetivos.

---

## 1. Sistema de movimiento del jugador
**Archivo:** `Assets/Game/Scripts/PlayerController.cs`

### Flujo de datos
1. El **Input System** llama `OnMove(InputAction.CallbackContext)`.
2. Se lee un `Vector2` y se guarda en `movementX / movementY`.
3. En `FixedUpdate` se construye un vector 3D `(x, 0, z)`.
4. Se aplica velocidad en el `Rigidbody` con `rb.linearVelocity`.

### Puntos clave
- **Input desacoplado**: el script depende de `PlayerInput` usando *Send Messages*.
- **Movimiento plano**: el personaje se desplaza en XZ, manteniendo Y para gravedad.
- **Ajuste de altura**: existe un bloque comentado para proyectar al terreno con Raycast.

---

## 2. Sistema de animación del jugador
**Archivo:** `Assets/Game/Scripts/PlayerAnimationController.cs`

### Qué hace
- Lee la velocidad del `Rigidbody`.
- Calcula `planarSpeed` (magnitud XZ).
- Actualiza el parámetro del Animator `Velocity`.
- Realiza *flip* horizontal del sprite según el movimiento X.

### Puntos clave
- `velocityParam` permite renombrar el parámetro del Animator.
- `flipDeadZone` evita cambios de dirección por micro-movimientos.

---

## 3. Animaciones aleatorias para props o NPCs
**Archivo:** `Assets/Game/Scripts/World/RandomAnimatorController.cs`

### Qué hace
- Cambia un parámetro del Animator en intervalos aleatorios.
- Usa una semilla (`seed`) para reproducibilidad.
- Limita valores y tiempos con rangos configurables.

---

## 4. Interfaces y contratos
Estas interfaces separan **lógica** de **implementaciones** y permiten extender el sistema sin romper dependencias:

- **`ICombatant`**: contrato de un actor en combate. Expone stats, estado de vida y métodos de daño/curación/estados.
- **`ICombatAction`**: acción ejecutable con un objetivo (ataque, huida, habilidad, ítem).
- **`IMultiTargetCombatAction`**: acción capaz de ejecutar sobre múltiples objetivos.
- **`IActionSelector`**: fuente de decisiones del jugador (UI). Implementado por `BattleUIController`.
- **`IAIController`**: lógica de decisión de enemigos. Implementado por `RandomEnemyAIController`.
- **`IActionResolver`**: decide si una acción aplica a un objetivo o a muchos.
- **`ITurnQueue`**: orden de turnos (cola de combatientes).
- **`IVictoryCondition`**: evalúa condiciones de victoria/derrota.
- **`IStatusEffect`**: ciclo de vida de efectos (aplicar → tick → remover).
- **`IFleeHandler`**: encapsula la lógica de huida (implementada por `CombatManager`).

---

## 5. Sistema de combate por turnos
### 5.1 Orquestación del combate
**Archivo:** `Assets/Game/Scripts/Combat/Core/CombatManager.cs`

**Responsabilidades principales**:
- Mantener listas de **jugadores** y **enemigos** (`playerParty` / `enemyParty`).
- Convertirlas a `ICombatant` para el sistema interno.
- Iniciar el combate (`StartCombat`) y evaluar el final (`EndCombat`).
- Publicar eventos (`TurnStarted`, `CombatLog`, `CombatEnded`).

**Flujo base**:
1. `StartCombat()` construye listas y prepara la cola (`TurnQueue`).
2. Inicia el primer turno con `StartNextTurn()`.
3. El combatiente activo llama `ChooseAction()`.
4. La acción se resuelve (`ActionResolver`).
5. Se termina el turno y se evalúa victoria.

**Acciones clave**:
- `SubmitPlayerAction()` solo acepta acciones cuando el turno pertenece a `PlayerCharacter`.
- `TryFlee()` calcula la probabilidad: `baseFleeChance + Luck * 0.01f`.
- `GetTargetsFor()` calcula objetivos según bando y tipo de objetivo.
- `SetEnemyToList()` (rama dev) permite agregar enemigos dinámicamente antes de iniciar el combate.

### 5.2 Orden de turnos
**Archivo:** `Assets/Game/Scripts/Combat/Core/TurnQueue.cs`
- Ordena combatientes por `Speed` (descendente).
- Rota la lista para simular turnos continuos.
- Elimina muertos en cada ciclo con `RemoveDead()`.

### 5.3 Resolución de acciones
**Archivo:** `Assets/Game/Scripts/Combat/Core/ActionResolver.cs`
- Si la acción implementa `IMultiTargetCombatAction`, ejecuta sobre la lista completa.
- Si no, usa solo el primer objetivo.

### 5.4 Personajes de combate
**Archivos:**
- `BaseCharacter.cs`
- `PlayerCharacter.cs`
- `EnemyCharacter.cs`

**BaseCharacter**:
- Contiene `CharacterStats` y `RuntimeStats`.
- Mantiene una lista de `IStatusEffect` activos.
- Lanza eventos `StatsChanged` y `Defeated`.
- En rama dev, `runtimeStats` es `SerializeField` para inspección visual en Unity.

**PlayerCharacter**:
- Usa un `IActionSelector` (UI) para decidir.
- Si no hay selector, ejecuta ataque básico automático.
- Provee métodos para construir acciones (`CreateBasicAttack`, `CreateAbility`, etc.).

**EnemyCharacter**:
- Usa un `IAIController` para decidir.
- Si no hay IA, realiza ataque básico automático.

### 5.5 Estadísticas runtime
**Archivo:** `Assets/Game/Scripts/Combat/Core/RuntimeStats.cs`
- Copia valores base desde `CharacterStats`.
- Aplica modificadores temporales por efectos.
- Protege mínimos (`MaxHealth >= 1`).
- En rama dev, los valores base/modificadores son visibles en el inspector.

### 5.6 Condición de victoria
**Archivo:** `Assets/Game/Scripts/Combat/Core/BasicVictoryCondition.cs`
- Victoria: ningún enemigo vivo.
- Derrota: ningún jugador vivo (o ambos bandos muertos).

### 5.7 Encuentros por colisión
**Archivo:** `Assets/Game/Scripts/Combat/Core/PlayerEncounter.cs`
- `OnCollisionEnter()` añade el enemigo colisionado al `CombatManager`.
- Llama a `StartCombat()` automáticamente.
- Requiere `Collider` en ambos y `Rigidbody` en al menos uno.
- Recomiendo `autoStart = false` en el `CombatManager` para evitar que inicie al cargar escena.

---

## 6. Acciones disponibles
### 6.1 Ataque básico
**Archivo:** `Combat/Actions/BasicAttackAction.cs`
- Daño = `Attack - (Defense / 2)` con mínimo 1.
- Probabilidad de crítico según `Luck`.

### 6.2 Defender
**Archivo:** `Combat/Actions/DefendAction.cs`
- Aplica un `StatModifierEffect` temporal a `Memoria` (defensa).

### 6.3 Huir
**Archivo:** `Combat/Actions/FleeAction.cs`
- Invoca `CombatManager.TryFlee()`.

### 6.4 Habilidades
**Archivo:** `Combat/Actions/MathAbilityAction.cs`
- Usa `AbilityData` para elegir efecto y objetivos.
- Soporta objetivos múltiples cuando aplica.

### 6.5 Uso de ítems
**Archivo:** `Combat/Actions/UseItemAction.cs`
- Ejecuta efectos según `ItemEffectType`.
- No consume el ítem automáticamente (requiere inventario si se desea).

---

## 7. Efectos de estado
**Archivo:** `Combat/Effects/StatModifierEffect.cs`
- Aplica un modificador temporal a una estadística.
- Se revierte al terminar su duración.
- La duración se reduce en cada turno (`Tick`).

---

## 8. IA de enemigos
**Archivo:** `Combat/AI/RandomEnemyAIController.cs`
- Decide entre ataque básico o habilidad.
- `abilityChance` define probabilidad de usar habilidad.
- Usa `CombatManager.GetTargetsFor()` para objetivos válidos.

---

## 9. UI de combate
**Archivo:** `Combat/UI/BattleUIController.cs`

### Flujo
1. `CombatManager` llama a `PlayerCharacter.ChooseAction()`.
2. `BattleUIController.RequestAction()` abre el menú principal.
3. El jugador selecciona acción/objetivo.
4. `SubmitAction()` envía la decisión a `CombatManager`.

### Detalles internos
- `activePlayer` guarda el jugador que está decidiendo.
- `pendingAction` se usa cuando hay que elegir objetivo.
- `ShowPanel()` activa/desactiva paneles para mantener UI limpia.
- `CombatLog` actualiza `messageLogText` con mensajes del combate.

---

## 10. Datos de juego (ScriptableObjects)
**Ubicación de assets en rama dev:** `Assets/Game/Scriptable Objects/`

### CharacterStats
**Archivo:** `Combat/Data/CharacterStats.cs`
- Define stats base y habilidades iniciales.

### AbilityData
**Archivo:** `Combat/Data/AbilityData.cs`
- Define tipo de efecto, objetivo y potencia.

### ItemData
**Archivo:** `Combat/Data/ItemData.cs`
- Define tipo de ítem y potencia.

---

## 11. Extensiones recomendadas
- Crear nuevas acciones implementando `ICombatAction`.
- Crear estados nuevos implementando `IStatusEffect`.
- Añadir un inventario real para consumir ítems.
- Reemplazar la IA aleatoria por decisiones más complejas.
- Generar UI dinámica basada en datos del personaje.
