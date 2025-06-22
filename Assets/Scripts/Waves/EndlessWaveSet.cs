using UnityEngine;

[CreateAssetMenu(fileName = "EndlessWaveSet", menuName = "Scriptable Objects/EndlessWaveSet")]
public class EndlessWaveSet : WaveSet
{
    [Header("Endless Mode Configuration")]
    [SerializeField] private EndlessWaveData endlessWaveData;

    [Header("Performance Settings")]
    [Tooltip("Cache generated waves to avoid regeneration")]
    public bool cacheWaves = true;

    [Tooltip("Maximum number of waves to cache")]
    public int maxCachedWaves = 10;

    private WaveData[] cachedWaves;
    private int currentCacheSize = 0;

    private void OnEnable()
    {
        // Mark this as infinite mode
        isInfinite = true;

        // Initialize cache if enabled
        if (cacheWaves)
        {
            cachedWaves = new WaveData[maxCachedWaves];
        }
    }

    public override WaveData GenerateWave(int waveIndex)
    {
        if (endlessWaveData == null)
        {
            Debug.LogError($"[EndlessWaveSet] EndlessWaveData is null in {name}");
            return null;
        }

        // Check cache first
        if (cacheWaves && waveIndex < maxCachedWaves && cachedWaves[waveIndex] != null)
        {
            return cachedWaves[waveIndex];
        }

        // Generate new wave
        WaveData generatedWave = endlessWaveData.GenerateWave(waveIndex);

        // Cache the wave if caching is enabled
        if (cacheWaves && waveIndex < maxCachedWaves)
        {
            cachedWaves[waveIndex] = generatedWave;
            currentCacheSize = Mathf.Max(currentCacheSize, waveIndex + 1);
        }

        return generatedWave;
    }

    /// <summary>
    /// Get preview of upcoming waves for UI display
    /// </summary>
    public WaveData[] GetWavePreview(int startWave, int count)
    {
        WaveData[] preview = new WaveData[count];

        for (int i = 0; i < count; i++)
        {
            preview[i] = GenerateWave(startWave + i);
        }

        return preview;
    }

    /// <summary>
    /// Clear the wave cache (useful for testing or memory management)
    /// </summary>
    public void ClearCache()
    {
        if (cachedWaves != null)
        {
            for (int i = 0; i < cachedWaves.Length; i++)
            {
                if (cachedWaves[i] != null)
                {
                    DestroyImmediate(cachedWaves[i]);
                    cachedWaves[i] = null;
                }
            }
        }
        currentCacheSize = 0;
    }

    /// <summary>
    /// Get statistics about the current endless configuration
    /// </summary>
    public EndlessWaveStats GetWaveStats(int waveNumber)
    {
        if (endlessWaveData == null) return new EndlessWaveStats();

        WaveData wave = GenerateWave(waveNumber);

        return new EndlessWaveStats
        {
            waveNumber = waveNumber,
            enemyCount = wave.numberOfEnemies,
            spawnInterval = wave.spawnInterval,
            startTimer = wave.startTimer,
            zombieTypesAvailable = wave.zombieSpawnOptions.Count,
            totalWeight = CalculateTotalWeight(wave),
            estimatedWaveDuration = CalculateWaveDuration(wave)
        };
    }

    private int CalculateTotalWeight(WaveData wave)
    {
        int total = 0;
        foreach (var option in wave.zombieSpawnOptions)
        {
            total += option.weight;
        }
        return total;
    }

    private float CalculateWaveDuration(WaveData wave)
    {
        // Estimate: spawn time + start timer
        float spawnTime = wave.numberOfEnemies * wave.spawnInterval;
        return spawnTime + wave.startTimer;
    }

    private void OnValidate()
    {
        // Ensure this is marked as infinite
        isInfinite = true;

        // Validate endless wave data
        if (endlessWaveData == null)
        {
            Debug.LogWarning($"[EndlessWaveSet] EndlessWaveData is not assigned in {name}");
        }
    }

    private void OnDestroy()
    {
        ClearCache();
    }
}

[System.Serializable]
public struct EndlessWaveStats
{
    public int waveNumber;
    public int enemyCount;
    public float spawnInterval;
    public float startTimer;
    public int zombieTypesAvailable;
    public int totalWeight;
    public float estimatedWaveDuration;
}