# Sistema de Combate — Guía técnica

## Propósito
El sistema de combate gestiona batallas por turnos entre el equipo del jugador y los enemigos.
Sigue principios SOLID: cada responsabilidad vive en su propia clase o interfaz.

---

## Estructura de carpetas
```
Assets/Game/Scripts/Combat/
├── AI/           → Decisiones de la IA enemiga
├── Actions/      → Acciones ejecutables (ataque, defensa, habilidades, huida, objeto)
├── Core/         → Orquestación del combate (CombatManager, turno, personajes, etc.)
├── Data/         → ScriptableObjects de datos (CharacterStats, AbilityData, ItemData)
├── Effects/      → Efectos de estado (veneno, parálisis, modificador de stat)
├── Interfaces/   → Contratos compartidos (ICombatant, ICombatAction, IStatusEffect…)
└── UI/           → Controlador de UI de combate (BattleUIController)
```

---

## Componentes clave

| Clase | Rol |
|---|---|
| `CombatManager` | Orquesta el combate: turnos, acciones, victoria/derrota |
| `TurnQueue` | Cola de turno ordenada por Rapidez |
| `ActionResolver` | Resuelve la ejecución de acciones sobre objetivos |
| `BasicVictoryCondition` | Evalúa si el combate terminó |
| `BattleUIController` | Muestra paneles y envía acciones del jugador |
| `CombatResultController` | Muestra pantallas de victoria/derrota |
| `PlayerCharacter` | Personaje del jugador: delega la acción al IActionSelector |
| `EnemyCharacter` | Enemigo: delega la acción al IAIController |

---

## Dependencias
- `PlayerData` → para distribuir experiencia al morir un enemigo
- `SceneTransitionManager` → para regresar al mundo o reiniciar tras el combate
- `TelemetryService` → registra métricas (opcional)
- `CombatFeedbackService` → efectos visuales/sonoros (opcional)

---

## Integración in-game (pasos Unity)
1. Crear o abrir la escena de combate (ej: `CombatScene`).
2. Crear un GameObject `CombatManager` y añadir el componente `CombatManager`.
3. Colocar los `PlayerCharacter` y `EnemyCharacter` en la escena y asignarlos en:
   - `playerParty` (jugadores)
   - `enemyParty` (enemigos)
4. En cada `PlayerCharacter`, asignar `actionSelectorComponent` a un `BattleUIController`.
5. En cada `EnemyCharacter`, asignar `aiControllerComponent` (ej: `RandomEnemyAIController`).
6. Crear un Canvas de combate y configurar `BattleUIController` con paneles y botones.
7. Ajustar `autoStart`:
   - `true` para iniciar al cargar escena.
   - `false` si el combate se dispara desde el mundo (ver `PlayerEncounter` / `StartCombat()`).

---

## Corrida en frío — Flujo completo de un turno

**Escenario:** Turno del jugador → elige Ataque básico → derrota al enemigo → victoria.

```
1. CombatManager.StartNextTurn()
   ├─ TurnQueue.RemoveDead()         // Limpia muertos de la cola
   ├─ victoryCondition.Evaluate()    // ¿Sigue en curso? → Ongoing
   ├─ currentCombatant = TurnQueue.GetNext()   // Obtiene PlayerCharacter
   ├─ currentCombatant.TickStatusEffects()     // Aplica veneno/parálisis
   ├─ ¿IsActionBlocked == true?      // No → continúa
   ├─ awaitingPlayerAction = true
   ├─ TurnStarted?.Invoke(player)    // BattleUIController muestra panel
   └─ player.ChooseAction(this)      // Llama IActionSelector.RequestAction()

2. Jugador presiona "Atacar" en la UI
   ├─ BattleUIController.OnAttackPressed()
   │   └─ pendingAction = player.CreateBasicAttack()
   │   └─ ShowTargetSelection(SingleEnemy)
   │
3. Jugador selecciona al enemigo
   ├─ BattleUIController.OnTargetSelected(enemy)
   └─ combatManager.SubmitPlayerAction(basicAttack, [enemy])

4. CombatManager.ExecuteAction(player, basicAttack, [enemy])
   ├─ actionResolver.Resolve(player, basicAttack, [enemy])
   │   └─ basicAttack.Execute(player, enemy)
   │       └─ daño = Inteligencia - (Memoria/2) con mínimo 1
   │       └─ enemy.TakeDamage(daño)
   │           └─ enemy.Defeated?.Invoke()    // Enemigo muerto
   │           └─ EnemyCharacter.OnDefeated() // Evalúa loot + experiencia
   └─ EndTurn()

5. CombatManager.StartNextTurn()
   ├─ TurnQueue.RemoveDead()         // Enemigo eliminado
   └─ victoryCondition.Evaluate()    // Enemigos = 0 → Victory
       └─ EndCombat(Victory)
           └─ CombatEnded?.Invoke(Victory)
               ├─ CombatResultController.OnCombatEnded() → ShowVictory()
               └─ TelemetryService.OnCombatEnded() → victorias++
```

---

## Estados alterados

| Efecto | Clase | Comportamiento |
|---|---|---|
| Veneno | `PoisonStatusEffect` | Daño fijo por turno durante N turnos |
| Parálisis | `ParalysisStatusEffect` | Pierde el turno durante N turnos (IActionBlockingEffect) |
| Buff/Debuff de stat | `StatModifierEffect` | Modifica una estadística N turnos; se revierte al expirar |

---

## Errores frecuentes

| Error | Causa probable | Solución |
|---|---|---|
| El combate no inicia | `autoStart = true` pero `playerParty` o `enemyParty` están vacíos | Asignar prefabs en el Inspector o llamar `SetEnemyToList()` antes de `StartCombat()` |
| La UI nunca muestra opciones | `BattleUIController` no está asignado como `IActionSelector` en `PlayerCharacter` | Asignar el componente en `actionSelectorComponent` del Inspector |
| El turno se salta sin que el jugador actúe | El personaje tiene un `IActionBlockingEffect` activo | Revisar los efectos de estado aplicados; es comportamiento correcto de parálisis |
| Acciones multi-objetivo no ejecutan en todos los blancos | `IMultiTargetCombatAction` no implementado en la acción | Implementar `IMultiTargetCombatAction` (ver `MathAbilityAction`) |
