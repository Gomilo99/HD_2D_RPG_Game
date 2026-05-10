# Guía detallada: cómo funcionan los sistemas

## Visión general
El proyecto implementa un **RPG por turnos** con los siguientes bloques principales:
- **Movimiento y animación** del jugador en el mundo.
- **Sistema de combate por turnos** con acciones, habilidades, ítems, IA y estados.
- **Datos de juego** basados en ScriptableObjects.
- **UI de combate** para selección de acciones y objetivos.

---

## 1. Sistema de movimiento del jugador
**Archivo:** `Assets/Game/Scripts/PlayerController.cs`

### Flujo de datos
1. El Input System envía un `Vector2` en `OnMove`.
2. `movementX` y `movementY` se actualizan con ese input.
3. En `FixedUpdate` se crea un vector 3D (X, 0, Z).
4. Se aplica velocidad fija al `Rigidbody` con `rb.linearVelocity`.

### Puntos clave
- **Velocidad constante**: el movimiento siempre tiene la misma magnitud.
- **Movimiento en plano**: se usa XZ para desplazamiento y Y para gravedad.
- **Ajuste de altura**: existe un bloque comentado que permite pegar el personaje al terreno usando Raycast.

---

## 2. Sistema de animación del jugador
**Archivo:** `Assets/Game/Scripts/PlayerAnimationController.cs`

### Qué hace
- Lee la velocidad del `Rigidbody`.
- Calcula `planarSpeed` (magnitud en XZ).
- Escribe ese valor en el parámetro del Animator `Velocity`.
- Hace flip horizontal del sprite según el movimiento X.

### Puntos clave
- `velocityParam` permite renombrar el parámetro si se desea.
- `flipDeadZone` evita micro-flips cuando la velocidad es casi cero.

---

## 3. Animaciones aleatorias para props o NPCs
**Archivo:** `Assets/Game/Scripts/World/RandomAnimatorController.cs`

### Qué hace
- Cada intervalo aleatorio, cambia un parámetro en el Animator.
- Usa una semilla (`seed`) para reproducibilidad.
- Controla rango de valores y tiempos.

### Ideal para
- NPCs con animaciones de idle variables.
- Props ambientales (fuego, luces, criaturas).

---

## 4. Sistema de combate por turnos
### 4.1 Orquestación del combate
**Archivo:** `Assets/Game/Scripts/Combat/Core/CombatManager.cs`

#### Flujo principal
1. `StartCombat()` construye listas de jugadores/enemigos.
2. Inicializa `TurnQueue` ordenando por velocidad.
3. Comienza el turno con `StartNextTurn()`.
4. Cada combatiente ejecuta `ChooseAction()`.
5. El resultado de la acción se resuelve y termina el turno.

#### Eventos disponibles
- `TurnStarted(ICombatant)`
- `CombatLog(string)`
- `CombatEnded(CombatResult)`

---

### 4.2 Orden de turnos
**Archivo:** `Assets/Game/Scripts/Combat/Core/TurnQueue.cs`

- Ordena combatientes por `Speed` (descendente).
- Cada turno rota la lista (el primero pasa al final).
- Elimina personajes muertos en cada ciclo.

---

### 4.3 Acciones y resolución
**Archivo:** `Assets/Game/Scripts/Combat/Core/ActionResolver.cs`

- Si la acción implementa `IMultiTargetCombatAction`, se ejecuta en todos los objetivos.
- Si no, se ejecuta solo con el primer objetivo.

---

### 4.4 Personajes de combate
**Archivos:**
- `BaseCharacter.cs`
- `PlayerCharacter.cs`
- `EnemyCharacter.cs`

#### BaseCharacter
- Gestiona estadísticas runtime y estados (`IStatusEffect`).
- Expone métodos: `TakeDamage`, `Heal`, `ModifyStat`.
- Maneja `StatsChanged` y `Defeated`.

#### PlayerCharacter
- Usa un `IActionSelector` (UI) para decidir acciones.
- Si no hay selector, ataca automáticamente.
- Expone constructores de acciones (ataque, defender, habilidad, ítem, huir).

#### EnemyCharacter
- Usa un `IAIController` para decidir.
- Si no hay IA, realiza ataque básico.

---

### 4.5 Estadísticas runtime
**Archivo:** `Assets/Game/Scripts/Combat/Core/RuntimeStats.cs`

- Copia valores desde `CharacterStats`.
- Aplica modificadores temporales.
- Protege valores mínimos (ej. `MaxHealth` >= 1).

---

### 4.6 Condición de victoria
**Archivo:** `Assets/Game/Scripts/Combat/Core/BasicVictoryCondition.cs`

- Victoria: ningún enemigo vivo.
- Derrota: ningún jugador vivo.
- En caso de ambos muertos, se considera derrota.

---

## 5. Acciones disponibles
### 5.1 Ataque básico
**Archivo:** `Combat/Actions/BasicAttackAction.cs`
- Daño = `Attack - (Defense / 2)` con mínimo 1.
- Probabilidad de crítico según `Luck`.

### 5.2 Defender
**Archivo:** `Combat/Actions/DefendAction.cs`
- Aplica un `StatModifierEffect` temporal a `Memoria` (defensa).

### 5.3 Huir
**Archivo:** `Combat/Actions/FleeAction.cs`
- Llama a `CombatManager.TryFlee`.
- Probabilidad = `baseFleeChance + Luck * 0.01f`.

### 5.4 Habilidades matemáticas
**Archivo:** `Combat/Actions/MathAbilityAction.cs`
- Usa `AbilityData` para decidir efecto.
- Soporta objetivos múltiples.
- Efectos disponibles:
  - Daño
  - Debuff Inteligencia
  - Debuff Memoria
  - Buff Memoria
  - Curación

### 5.5 Uso de ítems
**Archivo:** `Combat/Actions/UseItemAction.cs`
- Ejecuta efecto según `ItemEffectType`.
- Nota: el ítem **no se consume** automáticamente (esto debe añadirse si se desea inventario real).

---

## 6. Efectos de estado
**Archivo:** `Combat/Effects/StatModifierEffect.cs`

- Aplica una modificación de estadística por X turnos.
- Se aplica una sola vez y se revierte al terminar.
- Se reduce `RemainingTurns` en cada turno.

---

## 7. IA de enemigos
**Archivo:** `Combat/AI/RandomEnemyAIController.cs`

- Decide entre ataque básico o habilidad.
- `abilityChance` define la probabilidad de usar habilidad.
- Usa `CombatManager.GetTargetsFor` para objetivos válidos.

---

## 8. UI de combate
**Archivo:** `Combat/UI/BattleUIController.cs`

### Flujo
1. `CombatManager` llama a `PlayerCharacter.ChooseAction()`.
2. `BattleUIController.RequestAction()` muestra el menú principal.
3. El jugador presiona acción/objetivo.
4. `SubmitAction()` envía la decisión a `CombatManager`.

### Paneles
- **ActionMenuPanel**: atacar/defender/huir.
- **TargetSelectPanel**: selección de objetivo.
- **AbilityMenuPanel**: habilidades.
- **ItemMenuPanel**: ítems.
- **OverlayPanel**: panel opcional.

### Logs
- `CombatLog` actualiza `messageLogText` con mensajes del combate.

---

## 9. Datos de juego (ScriptableObjects)
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

## 10. Extensiones recomendadas
- Crear nuevas acciones implementando `ICombatAction`.
- Crear estados nuevos implementando `IStatusEffect`.
- Añadir un sistema de inventario real para consumir ítems.
- Reemplazar la IA aleatoria por árboles de decisión.
- Generar UI dinámica en base a datos (habilidades e ítems del personaje).
