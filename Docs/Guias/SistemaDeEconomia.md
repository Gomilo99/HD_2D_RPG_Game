# Sistema de Economía — Guía técnica

## Propósito
Gestionar el inventario del jugador (consumibles y equipamiento), la tienda con
compra/venta, el loot que sueltan los enemigos y el dinero del jugador.

---

## Componentes clave

| Clase | Rol |
|---|---|
| `PlayerInventory` | Singleton: gestiona consumibles y objetos de equipamiento |
| `PlayerData` | Singleton: dinero y referencia al equipo de personajes |
| `LootTable` | ScriptableObject: define qué suelta un enemigo al morir |
| `LootEntry` | Datos de un ítem looteado (probabilidad, cantidad, dinero asociado) |
| `EquipmentData` | ScriptableObject: equipamiento con modificadores de stats |
| `ShopNPC` | NPC de tienda con compra/venta (IInteractable) |
| `ItemData` | ScriptableObject: ítem consumible con efecto y precio |

---

## Flujo del dinero

```
Ganado:
  └─ LootTable.Evaluate()     → PlayerData.AddMoney(dineroTotal)
  └─ InteractableChest        → PlayerData.AddMoney(moneyReward)
  └─ ShopNPC.SellItem()       → PlayerData.AddMoney(item.value)

Gastado:
  └─ ShopNPC.BuyItem()        → PlayerData.SpendMoney(item.value)
```

---

## Configurar una LootTable

1. Clic derecho → **RPG/Loot Table** → renombrar (ej: `Enemigo_BaseLoot`).
2. Configurar `experienceReward` (XP que da al morir).
3. Añadir entradas en `Entries`:
   ```
   item:         Poción de Cordura
   dropChance:   0.5   (50%)
   minQuantity:  1
   maxQuantity:  2
   moneyDrop:    10
   ```
4. Asignar el asset al campo `LootTable` del `EnemyCharacter` en el Inspector.

---

## Corrida en frío — Loot al derrotar un enemigo

```
1. EnemyCharacter.OnDefeated() invocado
   └─ lootTable.Evaluate()
       ├─ Por cada LootEntry:
       │   ├─ Random.value <= 0.5 → true (50% de probabilidad)
       │   ├─ quantity = Random.Range(1, 3) = 2
       │   ├─ PlayerInventory.Instance.AddItem(pocion, 2)
       │   └─ totalMoney += 10
       │
       └─ PlayerData.Instance.AddMoney(10)
```

---

## Configurar equipamiento

1. Clic derecho → **RPG/Equipment Data** → renombrar (ej: `AnteojosDeFoco`).
2. Configurar los modificadores:
   ```
   inteligenciaModifier:   +3
   abilityPowerMultiplier: 0.15  (15% más potencia en habilidades)
   value:                  150
   ```
3. Cuando el jugador lo equipa (lógica de UI a implementar):
   - Llamar `PlayerInventory.Instance.AddEquipment(data)`.
   - Aplicar los modificadores al personaje con `ModifyStat()`.
   - Al desequipar, revertir con `-modifier`.

---

## Corrida en frío — Comprar en tienda

```
1. Jugador interactúa con ShopNPC → abre el panel de tienda
2. Jugador selecciona "Poción de Cordura" (value = 50)
3. UI llama shopNPC.BuyItem(pocion)
   ├─ catalogItems.Contains(pocion) → true
   ├─ PlayerData.SpendMoney(50)
   │   ├─ money = 120 >= 50 → OK
   │   └─ money = 70
   ├─ PlayerInventory.AddItem(pocion, 1)
   └─ ItemPurchased?.Invoke(pocion) → UI actualiza stock/dinero
```

---

## Errores frecuentes

| Error | Causa probable | Solución |
|---|---|---|
| El loot no aparece en el inventario | `PlayerInventory.Instance` es nulo | Asegurar que PlayerInventory esté en la escena con DontDestroyOnLoad |
| El jugador puede comprar sin dinero | `ShopNPC.BuyItem()` no está verificando el dinero | El método ya verifica `SpendMoney()`; confirmar que `PlayerData.Instance` no sea nulo |
| Los objetos no tienen precio | Campo `value` en `ItemData` es 0 | Configurar el campo `value` en los assets ScriptableObject del ítem |
| El equipamiento no modifica las stats | La lógica de equipar/desequipar no llama `ModifyStat()` | Implementar la UI de equipamiento que llame `ModifyStat()` con los valores de EquipmentData |
