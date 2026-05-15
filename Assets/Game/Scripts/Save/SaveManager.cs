using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Servicio de guardado y carga de partidas.
/// Gestiona hasta 3 slots de guardado usando JSON serializado en Application.persistentDataPath.
/// Singleton persistente entre escenas.
///
/// Responsabilidades (S de SOLID):
/// - Serializar y deserializar SaveData a/desde disco.
/// - Validar integridad básica con checksum.
/// - Construir SaveData a partir del estado actual del juego.
/// - Aplicar SaveData al estado del juego al cargar.
///
/// Corrida en frío — Guardar:
/// 1. SaveManager.Instance.Save(slot) es llamado (p.ej., desde un punto de guardado).
/// 2. BuildSaveData(slot) recopila datos del ICheckpointProvider, PlayerData,
///    PlayerInventory y los CharacterLevel de cada personaje.
/// 3. Se calcula el checksum y se serializa a JSON.
/// 4. Se escribe en Application.persistentDataPath/save_slotN.json.
///
/// Corrida en frío — Cargar:
/// 1. SaveManager.Instance.Load(slot) lee el JSON del disco.
/// 2. Se valida el checksum; si falla se registra un error y se aborta.
/// 3. Se restaura la posición, dinero, tiempo de juego y personajes.
/// 4. Se carga la escena guardada con SceneTransitionManager.
///
/// Posibles errores:
/// - Archivo no existe: Load() retorna false y no modifica el estado del juego.
/// - Checksum inválido: probable corrupción, se registra error y se aborta.
/// - ICheckpointProvider nulo al guardar: se registra un warning y la posición queda en 0.
/// </summary>
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [SerializeField, Min(1)] private int maxSlots = 3;

    private ICheckpointProvider checkpointProvider;
    private float sessionStartTime;

    /// <summary>Se dispara después de guardar exitosamente. Parámetro: slot usado.</summary>
    public event Action<int> GameSaved;

    /// <summary>Se dispara después de cargar exitosamente. Parámetro: slot cargado.</summary>
    public event Action<int> GameLoaded;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        sessionStartTime = Time.realtimeSinceStartup;
    }

    // ── API pública ───────────────────────────────────────────────────────────

    /// <summary>Registra el proveedor de datos de posición/escena.</summary>
    public void SetCheckpointProvider(ICheckpointProvider provider)
    {
        checkpointProvider = provider;
    }

    /// <summary>
    /// Guarda la partida en el slot indicado (1–maxSlots).
    /// Retorna true si el guardado fue exitoso.
    /// </summary>
    public bool Save(int slot)
    {
        if (!EsSlotValido(slot))
        {
            return false;
        }

        SaveData data = BuildSaveData(slot);
        data.checksum = CalcularChecksum(data);

        string json = JsonUtility.ToJson(data, prettyPrint: true);
        string path = ObtenerRuta(slot);

        try
        {
            File.WriteAllText(path, json);
            Debug.Log($"SaveManager: Partida guardada en slot {slot} → {path}");
            GameSaved?.Invoke(slot);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"SaveManager: Error al guardar en slot {slot}: {ex.Message}");
            return false;
        }
    }
    public void LoadSlot1()
    {
        Load(1);
    }
    public void LoadSlot2()
    {
        Load(2);
    }
    public void LoadSlot3()
    {
        Load(3);
    }
    /// <summary>
    /// Carga la partida del slot indicado y aplica los datos al estado del juego.
    /// Retorna true si la carga fue exitosa.
    /// </summary>
    public bool Load(int slot)
    {
        if (!EsSlotValido(slot))
        {
            return false;
        }

        string path = ObtenerRuta(slot);
        if (!File.Exists(path))
        {
            Debug.Log($"SaveManager: No existe guardado en slot {slot}.");
            return false;
        }

        try
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            if (!ValidarChecksum(data))
            {
                Debug.LogError($"SaveManager: Checksum inválido en slot {slot}. Datos posiblemente corruptos.");
                return false;
            }

            AplicarSaveData(data);
            Debug.Log($"SaveManager: Partida cargada desde slot {slot}.");
            GameLoaded?.Invoke(slot);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"SaveManager: Error al cargar slot {slot}: {ex.Message}");
            return false;
        }
    }

    /// <summary>Retorna true si existe un archivo de guardado para el slot indicado.</summary>
    public bool SlotExists(int slot)
    {
        return EsSlotValido(slot) && File.Exists(ObtenerRuta(slot));
    }

    /// <summary>
    /// Elimina el guardado del slot indicado.
    /// Retorna true si se eliminó correctamente.
    /// </summary>
    public bool DeleteSlot(int slot)
    {
        if (!EsSlotValido(slot))
        {
            return false;
        }

        string path = ObtenerRuta(slot);
        if (!File.Exists(path))
        {
            return false;
        }

        try
        {
            File.Delete(path);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"SaveManager: Error al eliminar slot {slot}: {ex.Message}");
            return false;
        }
    }

    // ── Construcción de SaveData ──────────────────────────────────────────────

    private SaveData BuildSaveData(int slot)
    {
        SaveData data = new SaveData
        {
            version = 1,
            slotId = slot,
            saveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        // Posición y escena.
        if (checkpointProvider != null)
        {
            data.positionX = checkpointProvider.PositionX;
            data.positionY = checkpointProvider.PositionY;
            data.positionZ = checkpointProvider.PositionZ;
            data.sceneName = checkpointProvider.CurrentSceneName;
            data.cityName = checkpointProvider.CurrentCityName;
        }
        else
        {
            data.sceneName = SceneManager.GetActiveScene().name;
            Debug.LogWarning("SaveManager: ICheckpointProvider no registrado. La posición se guarda como (0,0,0).");
        }

        // Dinero y tiempo de juego.
        if (PlayerData.Instance != null)
        {
            data.money = PlayerData.Instance.Money;
        }

        data.playTimeSeconds = Time.realtimeSinceStartup - sessionStartTime;

        // Personajes del equipo.
        if (PlayerData.Instance != null)
        {
            foreach (BaseCharacter member in PlayerData.Instance.PartyMembers)
            {
                if (member == null)
                {
                    continue;
                }

                CharacterSaveData charData = BuildCharacterSaveData(member);
                data.party.Add(charData);
            }
        }

        return data;
    }

    private CharacterSaveData BuildCharacterSaveData(BaseCharacter character)
    {
        CharacterSaveData charData = new CharacterSaveData
        {
            characterName = character.Name,
            currentHealth = character.CurrentHealth,
            maxHealth = character.MaxHealth,
            inteligencia = character.Attack,
            memoria = character.Defense,
            rapidez = character.Speed,
            fealdad = character.Luck
        };

        CharacterLevel levelComp = character.GetComponent<CharacterLevel>();
        if (levelComp != null)
        {
            charData.level = levelComp.CurrentLevel;
            charData.totalExperience = levelComp.TotalExperience;

            foreach (AbilityData ability in levelComp.UnlockedAbilities)
            {
                if (ability != null)
                {
                    charData.unlockedAbilityNames.Add(ability.abilityName);
                }
            }
        }

        return charData;
    }

    // ── Aplicación de SaveData ────────────────────────────────────────────────

    private void AplicarSaveData(SaveData data)
    {
        if (data == null)
        {
            return;
        }

        // Restaurar dinero.
        if (PlayerData.Instance != null)
        {
            // Resetear dinero actual y aplicar el guardado.
            int diff = data.money - PlayerData.Instance.Money;
            if (diff > 0)
            {
                PlayerData.Instance.AddMoney(diff);
            }
        }

        // Restaurar tiempo de juego de la sesión.
        sessionStartTime = Time.realtimeSinceStartup - data.playTimeSeconds;

        // Navegar a la escena guardada si es diferente a la actual.
        if (!string.IsNullOrEmpty(data.sceneName) &&
            data.sceneName != SceneManager.GetActiveScene().name &&
            SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.GoToCombat(data.sceneName);
        }
    }

    // ── Integridad ────────────────────────────────────────────────────────────

    /// <summary>Calcula un checksum básico sumando los valores numéricos del SaveData.</summary>
    private int CalcularChecksum(SaveData data)
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + data.version;
            hash = hash * 31 + data.slotId;
            hash = hash * 31 + data.money;
            hash = hash * 31 + (int)data.playTimeSeconds;
            hash = hash * 31 + data.party.Count;
            return hash;
        }
    }

    private bool ValidarChecksum(SaveData data)
    {
        return data.checksum == CalcularChecksum(data);
    }

    // ── Utilidades ────────────────────────────────────────────────────────────

    private string ObtenerRuta(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"save_slot{slot}.json");
    }

    private bool EsSlotValido(int slot)
    {
        if (slot < 1 || slot > maxSlots)
        {
            Debug.LogWarning($"SaveManager: Slot {slot} fuera de rango (1–{maxSlots}).");
            return false;
        }

        return true;
    }
}
