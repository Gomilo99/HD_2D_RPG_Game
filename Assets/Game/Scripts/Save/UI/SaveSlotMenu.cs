using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// UI helper for loading, saving, and selecting a slot for a new game.
/// </summary>
public class SaveSlotMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI[] slotLabels;

    [Header("New Game")]
    [SerializeField] private string newGameSceneName = "WorldScene";
    [SerializeField] private bool deleteSlotOnNewGame = true;

    private void OnEnable()
    {
        RefreshSlotLabels();
    }

    public void Open()
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }

        RefreshSlotLabels();
    }

    public void Close()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    public void SelectSlotForNewGame(int slot)
    {
        if (SaveManager.Instance == null)
        {
            return;
        }

        if (!SaveManager.Instance.SetActiveSlot(slot))
        {
            return;
        }

        if (deleteSlotOnNewGame && SaveManager.Instance.SlotExists(slot))
        {
            SaveManager.Instance.DeleteSlot(slot);
        }

        SceneManager.LoadScene(newGameSceneName);
    }

    public void LoadSlot(int slot)
    {
        if (SaveManager.Instance == null)
        {
            return;
        }

        SaveManager.Instance.SetActiveSlot(slot);
        SaveManager.Instance.Load(slot);
    }

    public void SaveSlot(int slot)
    {
        if (SaveManager.Instance == null)
        {
            return;
        }

        SaveManager.Instance.SetActiveSlot(slot);
        SaveManager.Instance.Save(slot);
    }

    public void RefreshSlotLabels()
    {
        if (slotLabels == null || SaveManager.Instance == null)
        {
            return;
        }

        for (int i = 0; i < slotLabels.Length; i++)
        {
            TextMeshProUGUI label = slotLabels[i];
            if (label == null)
            {
                continue;
            }

            int slot = i + 1;
            bool exists = SaveManager.Instance.SlotExists(slot);
            label.text = exists ? $"Slot {slot} (Used)" : $"Slot {slot} (Empty)";
        }
    }
}
