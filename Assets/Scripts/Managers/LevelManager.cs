using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public Transform roulote;
    public Transform targetZombies;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Debug.LogWarning("Duplicate SettingsManager detected. Destroying new instance.");
            Destroy(gameObject);
        }
        else if (Instance.gameObject == null)
        {
            // Reassign if the existing instance was destroyed
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        // Clear the static reference when this instance is destroyed
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
}
