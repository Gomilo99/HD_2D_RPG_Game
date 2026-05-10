# Sistema de Guardado — Guía técnica

## Propósito
Persistir el estado completo de la partida (posición, equipo, dinero, tiempo) en disco,
usando hasta 3 slots de guardado. Detecta corrupción básica con un checksum.

---

## Componentes clave

| Clase | Rol |
|---|---|
| `SaveManager` | Servicio principal: guarda, carga y borra slots |
| `SaveData` | Modelo de datos serializable (JSON) |
| `CharacterSaveData` | Datos de un personaje individual dentro del SaveData |
| `ICheckpointProvider` | Interfaz que reporta posición y escena actual del jugador |
| `SaveCheckpoint` | Objeto interactuable en el mundo que implementa ICheckpointProvider y dispara el guardado |

---

## Ruta de archivos
Los archivos se guardan en `Application.persistentDataPath`:
- Windows: `C:/Users/<usuario>/AppData/LocalLow/<empresa>/<juego>/`
- Android: `/storage/emulated/0/Android/data/<id>/files/`
- Formato: `save_slot1.json`, `save_slot2.json`, `save_slot3.json`

---

## Cómo configurar en la escena

1. Crear un GameObject vacío y añadir el componente `SaveManager`.
2. Crear otro GameObject y añadir `PlayerData` (singleton del equipo/dinero).
3. En el mundo, colocar un objeto con `SaveCheckpoint`:
   - Asignar `saveSlot = 1` (o 2/3).
   - Asignar `cityName` con el nombre de la zona (ej: "Pueblo del Inicio").
   - El jugador interactúa con él para guardar.
4. Para cargar al iniciar el juego, llamar `SaveManager.Instance.Load(1)`.

---

## Corrida en frío — Guardar partida

```
1. Jugador interactúa con SaveCheckpoint
   └─ SaveCheckpoint.Interact(jugador)
       ├─ playerReference = jugador
       ├─ SaveManager.Instance.SetCheckpointProvider(this)
       └─ SaveManager.Instance.Save(1)

2. SaveManager.Save(1)
   ├─ BuildSaveData(1)
   │   ├─ Posición y escena desde ICheckpointProvider
   │   ├─ dinero desde PlayerData.Instance.Money
   │   ├─ tiempo = Time.realtimeSinceStartup - sessionStartTime
   │   └─ Por cada personaje en PartyMembers:
   │       └─ CharacterLevel → level, totalExperience, habilidades desbloqueadas
   │
   ├─ data.checksum = CalcularChecksum(data)
   ├─ JsonUtility.ToJson(data)
   └─ File.WriteAllText("save_slot1.json", json)
       └─ GameSaved?.Invoke(1)
```

---

## Corrida en frío — Cargar partida

```
1. SaveManager.Instance.Load(1)
   ├─ File.Exists("save_slot1.json") → true
   ├─ json = File.ReadAllText(...)
   ├─ data = JsonUtility.FromJson<SaveData>(json)
   ├─ ValidarChecksum(data) → checksum coincide → OK
   └─ AplicarSaveData(data)
       ├─ Restaurar dinero → PlayerData.AddMoney(diff)
       ├─ Restaurar tiempo de sesión
       └─ Si sceneName ≠ escena actual → SceneTransitionManager.GoToCombat(sceneName)
```

---

## Checksum de integridad
Se calcula con una combinación simple de campos numéricos clave (versión, slot, dinero,
tiempo, número de personajes). Si el archivo fue modificado manualmente o se corrompió,
el checksum fallará y se registrará un error en la consola sin aplicar los datos.

---

## Errores frecuentes

| Error | Causa probable | Solución |
|---|---|---|
| "Checksum inválido" al cargar | El archivo fue modificado externamente o versión incorrecta | Borrar el slot con `DeleteSlot()` y crear una nueva partida |
| La posición se guarda como (0,0,0) | `ICheckpointProvider` no registrado antes de guardar | El `SaveCheckpoint` se registra automáticamente al interactuar |
| `PlayerData.Instance` es nulo al guardar | PlayerData no existe en la escena o no persiste | Asegurar que PlayerData esté en un GameObject con DontDestroyOnLoad |
| El slot no existe al intentar cargar | No se ha guardado antes en ese slot | Verificar con `SaveManager.SlotExists(slot)` antes de llamar `Load()` |
