using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int lastWaveIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveProgress(int waveIndex)
    {
        lastWaveIndex = waveIndex;
        PlayerPrefs.SetInt("SavedWaveIndex", waveIndex);
        PlayerPrefs.Save();
    }

    public int LoadSavedWave()
    {
        return PlayerPrefs.GetInt("SavedWaveIndex", 0);
    }
}