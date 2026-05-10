# Sistema del Mundo — Guía técnica

## Propósito
Gestionar la exploración del jugador: enemigos con IA de patrulla/persecución,
objetos interactuables (cofres, NPCs de tienda), sistema de encuentro con enemigos
y transiciones de escena.

---

## Componentes clave

| Clase | Rol |
|---|---|
| `EnemyPatrolController` | IA de patrulla + persecución del enemigo en el mundo |
| `PlayerEncounter` | Detecta colisión jugador-enemigo e inicia el combate |
| `InteractableChest` | Cofre con recompensa al abrirlo (una sola vez) |
| `ShopNPC` | NPC de tienda con compra/venta de consumibles |
| `SceneTransitionManager` | Transición con fundido entre mundo, combate y menú |
| `PlayerInteractionDetector` | Detecta IInteractable cercanos y maneja el input de interacción |
| `IInteractable` | Interfaz base para todo objeto interactuable del mundo |

---

## Configurar EnemyPatrolController

1. Añadir el componente al GameObject del enemigo (que debe tener `Rigidbody`).
2. Crear Transforms vacíos como waypoints y asignarlos a la lista `Waypoints`.
3. Ajustar:
   - `Detection Range`: radio de detección del jugador (círculo amarillo en Gizmos).
   - `Aggro Range`: radio de persecución (círculo rojo); si el jugador supera esta distancia, el enemigo regresa.
   - `Move Speed`: velocidad de movimiento.
4. Asegurarse de que el GameObject del jugador tenga el tag `"Player"`.

---

## Corrida en frío — Patrulla → Persecución → Combate

```
1. Awake: EnemyPatrolController inicializa Rigidbody, playerTransform = GameObject.FindWithTag("Player")

2. FixedUpdate cada frame:
   a. ActualizarEstado()
      ├─ distToPlayer = distancia al jugador
      └─ Si estado == Patrolling y distToPlayer <= detectionRange → estado = Chasing

   b. EjecutarMovimiento()
      └─ Si estado == Chasing → MoverHacia(playerTransform.position)
          └─ rb.linearVelocity = dirección * moveSpeed

3. El enemigo alcanza al jugador y colisiona físicamente
   └─ PlayerEncounter.OnCollisionEnter(collision)
       ├─ collision.gameObject.CompareTag("Enemy") → true
       ├─ Si combatInSameScene == true:
       │   └─ combatManager.SetEnemyToList(enemy)
       │   └─ combatManager.StartCombat()
       └─ Si combatInSameScene == false:
           └─ SceneTransitionManager.GoToCombat()
```

---

## Corrida en frío — Cofre

```
1. PlayerInteractionDetector detecta InteractableChest (IInteractable) en el radio
2. Jugador presiona "E" (botón "Interact")
3. InteractableChest.Interact(jugador)
   ├─ opened = false → ejecutar
   ├─ Por cada ItemData en items:
   │   └─ PlayerInventory.Instance.AddItem(item, 1)
   ├─ PlayerData.Instance.AddMoney(moneyReward)
   ├─ chestAnimator.SetTrigger("Open")
   └─ opened = true → ya no se puede volver a abrir
```

---

## Corrida en frío — Transición Mundo → Combate → Mundo

```
1. SceneTransitionManager.GoToCombat()
   ├─ returnScene = SceneManager.GetActiveScene().name  ("Level1")
   └─ Coroutine TransitionTo("CombatScene")
       ├─ Fade(0 → 1)  [fundido negro de salida]
       ├─ SceneManager.LoadScene("CombatScene")
       └─ Fade(1 → 0)  [fundido negro de entrada]

2. Combate finaliza con Victoria
   └─ CombatResultController.OnContinuePressed()
       └─ SceneTransitionManager.ReturnToWorld()
           ├─ returnScene = "Level1"  (no vacío)
           └─ TransitionTo("Level1")
```

---

## Errores frecuentes

| Error | Causa probable | Solución |
|---|---|---|
| El enemigo no detecta al jugador | Tag "Player" no asignado al jugador | Asignar el tag en el Inspector del jugador |
| El cofre se puede abrir indefinidamente | `opened` no se persistió (escena recargada) | El cofre se recarga a su estado inicial al recargar la escena; para persistencia, guardar qué cofres fueron abiertos en `SaveData` |
| La interacción no funciona | Botón "Interact" no configurado en Input Manager | Ir a Project Settings → Input Manager y añadir eje "Interact" con tecla E |
| La transición se queda en negro | `transitionCanvas` no asignado o duración 0 | Asignar el CanvasGroup en el Inspector o verificar `fadeDuration > 0` |
