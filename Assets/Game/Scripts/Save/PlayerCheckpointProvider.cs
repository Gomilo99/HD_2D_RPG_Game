using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Simple checkpoint provider attached to the player for manual saves.
/// </summary>
public class PlayerCheckpointProvider : MonoBehaviour, ICheckpointProvider
{
    [SerializeField] private string cityName = "Unknown";

    private void OnEnable()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SetCheckpointProvider(this);
        }
    }

    public float PositionX => transform.position.x;
    public float PositionY => transform.position.y;
    public float PositionZ => transform.position.z;
    public string CurrentSceneName => SceneManager.GetActiveScene().name;
    public string CurrentCityName => cityName;
}
