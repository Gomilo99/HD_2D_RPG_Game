# Sistema de Progresión — Guía técnica

## Propósito
Gestionar el crecimiento de los personajes del equipo: experiencia, niveles,
crecimiento de estadísticas y desbloqueo de habilidades nuevas.

---

## Componentes clave

| Clase | Rol |
|---|---|
| `CharacterLevel` | Componente por personaje; acumula experiencia y sube de nivel |
| `LevelGrowthTable` | ScriptableObject; define la curva de progresión y el crecimiento de stats por nivel |
| `LevelEntry` | Datos de una entrada individual de nivel (experiencia, gains, habilidades) |

---

## Integración in-game (pasos Unity)
1. Crear un asset `LevelGrowthTable` para cada personaje o arquetipo.
2. En cada miembro del equipo, añadir `CharacterLevel` y asignar su `growthTable`.
3. Confirmar que `PlayerData.PartyMembers` referencia a los personajes activos.
4. En enemigos, asignar `LootTable` con `experienceReward` para distribuir XP.
5. (Opcional) En UI de combate, combinar habilidades iniciales + desbloqueadas.

---

## Cómo configurar una tabla de progresión

1. Clic derecho en el Project → **RPG/Level Growth Table** → renombrar (ej: `Alicia_GrowthTable`).
2. Añadir entradas en la lista `Entries` (una por nivel disponible):

```
Nivel 1:  experienceRequired = 0,    corduraGain = 0,  inteligenciaGain = 0
Nivel 2:  experienceRequired = 100,  corduraGain = 5,  inteligenciaGain = 1
Nivel 3:  experienceRequired = 250,  corduraGain = 5,  inteligenciaGain = 1,  abilitiesUnlocked = [Función_Cuadrática]
Nivel 5:  experienceRequired = 600,  corduraGain = 8,  inteligenciaGain = 2,  abilitiesUnlocked = [Función_Paralisis]
```

3. Asignar la tabla al componente `CharacterLevel` del personaje en el Inspector.

---

## Dependencias
- `BaseCharacter` → para aplicar el crecimiento de stats (`Heal`, `ModifyStat`)
- `EnemyCharacter` → llama `GainExperience()` en todos los personajes del equipo al morir

---

## Corrida en frío — El jugador gana experiencia

```
1. EnemyCharacter.OnDefeated() es invocado
   └─ lootTable.ExperienceReward = 80 XP

2. Por cada BaseCharacter en PlayerData.PartyMembers:
   └─ CharacterLevel levelComp = member.GetComponent<CharacterLevel>()
   └─ levelComp.GainExperience(80)

3. CharacterLevel.GainExperience(80)
   ├─ totalExperience += 80 → totalExperience = 80
   └─ VerificarSubidaDENivel()

4. VerificarSubidaDENivel()
   ├─ growthTable.GetNextLevelExperience(1) = 100
   ├─ 80 < 100 → no sube de nivel todavía
   └─ (sale del while)

--- Segunda batalla ---

5. GainExperience(50) → totalExperience = 130

6. VerificarSubidaDENivel()
   ├─ growthTable.GetNextLevelExperience(1) = 100
   ├─ 130 >= 100 → LevelUp()
   │   ├─ currentLevel = 2
   │   ├─ AplicarCrecimientoStats(entry nivel 2):
   │   │   └─ baseCharacter.Heal(5)       // +5 Cordura
   │   │   └─ baseCharacter.ModifyStat(Inteligencia, +1)
   │   └─ LeveledUp?.Invoke(2)
   │
   └─ Continuar mientras totalExperience >= siguiente umbral
```

---

## Errores frecuentes

| Error | Causa probable | Solución |
|---|---|---|
| El personaje nunca sube de nivel | `LevelGrowthTable` no asignada en `CharacterLevel` | Asignar la tabla en el Inspector |
| Las habilidades desbloqueadas no aparecen en el combate | `CharacterLevel.UnlockedAbilities` solo registra las ganadas por progresión, no las iniciales | Combinar `stats.startingAbilities` + `levelComp.UnlockedAbilities` en la UI |
| Se sube de nivel al instante sin razón | El `experienceRequired` de nivel 2 es 0 | Revisar la tabla y asignar valores positivos |
