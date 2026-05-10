# Guía paso a paso: integración de sistemas in-game

## Objetivo
Esta guía explica **cómo aplicar e integrar en el mundo del juego** los scripts y sistemas técnicos creados en la rama copilot. El foco está en **pasos concretos de Unity** para que el jugador pueda moverse, animarse y entrar a un combate por turnos funcional.

## Requisitos previos
- Unity **6000.3.6f1** (ver `ProjectSettings/ProjectVersion.txt`).
- Proyecto abierto desde `HD_2D_RPG_Game`.
- Input System activo con el asset `Assets/InputSystem_Actions.inputactions`.

---

## 1. Preparar la escena base del mundo
1. Crea o abre una escena en `Assets/Game/Scenes/`.
2. Coloca suelo/terreno y colisiones básicas.
3. Añade una **Main Camera** y ajusta su posición para vista 2.5D (eje Z como profundidad).
4. (Opcional) Crea un objeto vacío `WorldRoot` para organizar personajes y props.

---

## 2. Integrar el personaje jugable (movimiento + input)
1. Arrastra el prefab `Assets/Game/Prefabs/Player.prefab` a la escena **o** crea un nuevo GameObject `Player`.
2. Asegúrate de tener estos componentes en el objeto del jugador:
   - `Rigidbody`
   - `CapsuleCollider` o colisionador equivalente
   - `PlayerController` (`Assets/Game/Scripts/PlayerController.cs`)
   - `PlayerInput` (Input System)
3. Configura el `PlayerInput`:
   - **Actions**: `Assets/InputSystem_Actions.inputactions`.
   - **Behavior**: *Send Messages* (o equivalente) para disparar `OnMove`.
4. En `PlayerController` ajusta:
   - `speed`: velocidad de movimiento.
   - `groundDist` y `TerrainLayer` (si quieres reactivar el ajuste de altura).
5. Prueba el movimiento en Play Mode.

---

## 3. Integrar animación 2D del jugador
1. En el mismo GameObject del jugador añade:
   - `SpriteRenderer`
   - `Animator`
   - `PlayerAnimationController` (`Assets/Game/Scripts/PlayerAnimationController.cs`)
2. En el `Animator` crea un parámetro **float** llamado `Velocity`.
3. Vincula el controlador de animación al `Animator`.
4. El `PlayerAnimationController`:
   - Actualiza `Velocity` con la velocidad del Rigidbody.
   - Hace *flip* horizontal del sprite según el movimiento X.

---

## 4. Animaciones aleatorias en props o NPCs
Para objetos que requieren animaciones aleatorias:
1. Añade `RandomAnimatorController` (`Assets/Game/Scripts/World/RandomAnimatorController.cs`) al GameObject.
2. Configura en el inspector:
   - `randomParam`: nombre del parámetro en el Animator.
   - `minValue` / `maxValue`: rango del valor aleatorio.
   - `minInterval` / `maxInterval`: tiempo entre cambios.

---

## 5. Crear datos de combate (ScriptableObjects)
Crea estos assets en una carpeta como `Assets/Game/Data/`:

### 5.1 Estadísticas de personajes
- **Create → RPG → Character Stats** (`CharacterStats`)
- Configura:
  - `characterName`, `maxCordura`, `inteligencia`, `memoria`, `rapidez`, `fealdad`.
  - `startingAbilities`: lista de habilidades iniciales.

### 5.2 Habilidades
- **Create → RPG → Ability** (`AbilityData`)
- Configura:
  - `abilityName`, `effectType`, `targetType`, `power`, `durationTurns`.

### 5.3 Ítems
- **Create → RPG → Item** (`ItemData`)
- Configura:
  - `itemName`, `description`, `effectType`, `power`.

---

## 6. Preparar personajes para combate
### 6.1 Jugadores
1. Crea GameObjects de jugadores (sprites o prefabs).
2. Agrega `PlayerCharacter` (`Assets/Game/Scripts/Combat/Core/PlayerCharacter.cs`).
3. Asigna:
   - `stats` con un `CharacterStats`.
   - `startingItems` (lista de `ItemData`).
   - `actionSelectorComponent` apuntando a un `BattleUIController` (ver paso 7).

### 6.2 Enemigos
1. Crea GameObjects de enemigos.
2. Agrega `EnemyCharacter` (`Assets/Game/Scripts/Combat/Core/EnemyCharacter.cs`).
3. Asigna:
   - `stats` con un `CharacterStats`.
   - `startingItems` si aplica.
   - `aiControllerComponent` apuntando a un `RandomEnemyAIController`.

---

## 7. Configurar el sistema de combate en la escena
1. Crea un GameObject `CombatManager`.
2. Agrega el componente `CombatManager` (`Assets/Game/Scripts/Combat/Core/CombatManager.cs`).
3. En el inspector:
   - Añade todos los **PlayerCharacter** a `playerParty`.
   - Añade todos los **EnemyCharacter** a `enemyParty`.
   - `autoStart`: true si quieres iniciar automáticamente.
   - `baseFleeChance`: probabilidad base de huida.

---

## 8. Montar la UI de combate
1. Crea un `Canvas` y dentro:
   - `ActionMenuPanel`
   - `TargetSelectPanel`
   - `AbilityMenuPanel`
   - `ItemMenuPanel`
   - `OverlayPanel` (opcional)
   - `MessageLogText` (UI Text)
2. Crea un GameObject `BattleUIController` y añade el script `BattleUIController`.
3. Asigna en el inspector las referencias de paneles y el `messageLogText`.
4. Configura los botones:
   - **Ataque** → `BattleUIController.OnAttackPressed`
   - **Defender** → `BattleUIController.OnDefendPressed`
   - **Huir** → `BattleUIController.OnFleePressed`
5. Para habilidades e ítems:
   - Crea botones dinámicos (o estáticos) y enlázalos a:
     - `OnAbilityPressed(AbilityData)`
     - `OnItemPressed(ItemData)`
6. Para selección de objetivos:
   - Crea botones por cada objetivo válido y enlázalos a:
     - `OnTargetSelected(BaseCharacter)`

---

## 9. Disparar el combate desde el mundo
Opciones recomendadas:
- **Auto start**: deja `autoStart = true` en `CombatManager`.
- **Desde un trigger**: deja `autoStart = false` y llama a `CombatManager.StartCombat()` desde un script de evento (por ejemplo, al entrar a un área).

> Consejo: si cambias de escena (exploración → combate), conserva datos del jugador con `DontDestroyOnLoad` o mediante un GameManager.

---

## 10. Verificación rápida
1. Abre la escena de combate.
2. Presiona **Play**.
3. Verifica:
   - Movimiento del jugador en el mundo.
   - Animaciones y flip del sprite.
   - Inicio del combate y turnos.
   - UI respondiendo a botones.
   - Daño, curación, buffs y fin de combate.

---

## Siguientes pasos sugeridos
- Agregar transiciones visuales al iniciar/finalizar combate.
- Crear un selector de objetivos más dinámico (lista UI generada en tiempo real).
- Añadir consumos de ítems (remover del inventario al usar).
