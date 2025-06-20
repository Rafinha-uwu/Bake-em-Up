using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public string sceneToContinue = "1";
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

    public void SaveProgress(string scene, int waveIndex)
    {
        sceneToContinue = scene;
        lastWaveIndex = waveIndex;
		PlayerPrefs.SetString("SavedSceneIndex", sceneToContinue);
		PlayerPrefs.SetInt("SavedWaveIndex", lastWaveIndex);
        PlayerPrefs.Save();
    }

    public void LoadSavedWave()
    {
		sceneToContinue = PlayerPrefs.GetString("SavedSceneIndex", "1");
		lastWaveIndex = PlayerPrefs.GetInt("SavedWaveIndex", 0);
    }
}