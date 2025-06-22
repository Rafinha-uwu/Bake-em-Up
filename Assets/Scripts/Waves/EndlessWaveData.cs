using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EndlessWaveData", menuName = "Scriptable Objects/EndlessWaveData")]
public class EndlessWaveData : ScriptableObject
{
    [Header("Base Configuration")]
    [Tooltip("Initial number of enemies in first wave")]
    public int baseNumberOfEnemies = 5;

    [Tooltip("How much to increase enemies each wave")]
    public int enemyIncreasePerWave = 2;

    [Tooltip("Maximum enemies per wave")]
    public int maxEnemiesPerWave = 50;

    [Header("Timing Configuration")]
    [Tooltip("Base spawn interval between enemies")]
    public float baseSpawnInterval = 2f;

    [Tooltip("Minimum spawn interval (gets faster over time)")]
    public float minSpawnInterval = 0.5f;

    [Tooltip("How much to decrease spawn interval per wave")]
    public float spawnIntervalDecrease = 0.1f;

    [Tooltip("Time between waves")]
    public float timeBetweenWaves = 15f;

    [Tooltip("Minimum time between waves")]
    public float minTimeBetweenWaves = 5f;

    [Tooltip("How much to decrease time between waves")]
    public float timeBetweenWavesDecrease = 0.5f;

    [Header("Zombie Types & Progression")]
    [Tooltip("All available zombie types with their unlock wave and weight progression")]
    public List<EndlessZombieType> zombieTypes = new List<EndlessZombieType>();

    [Header("Difficulty Scaling")]
    [Tooltip("Every X waves, increase difficulty significantly")]
    public int difficultySpike = 5;

    [Tooltip("Multiplier for enemy count on difficulty spikes")]
    public float spikeDifficultyMultiplier = 1.5f;



    /// <summary>
    /// Generates a wave based on the current wave number
    /// </summary>
    public WaveData GenerateWave(int waveNumber)
    {
        WaveData generatedWave = CreateInstance<WaveData>();

        // Calculate enemy count
        int enemyCount = CalculateEnemyCount(waveNumber);
        generatedWave.numberOfEnemies = enemyCount;

        // Calculate spawn interval
        float spawnInterval = CalculateSpawnInterval(waveNumber);
        generatedWave.spawnInterval = spawnInterval;

        // Calculate time until start
        float startTimer = CalculateStartTimer(waveNumber);
        generatedWave.startTimer = startTimer;

        // Set dialogue flag (endless mode doesn't wait for dialogue)
        generatedWave.StartsAfterDialogue = false;

        // Generate zombie spawn options for this wave
        generatedWave.zombieSpawnOptions = GenerateZombieSpawnOptions(waveNumber);

        // No events in endless mode - just zombies
        generatedWave.waveEvents = new List<WaveEventBase>();

        return generatedWave;
    }

    private int CalculateEnemyCount(int waveNumber)
    {
        int count = baseNumberOfEnemies + (enemyIncreasePerWave * waveNumber);

        // Apply difficulty spike
        if (waveNumber > 0 && waveNumber % difficultySpike == 0)
        {
            count = Mathf.RoundToInt(count * spikeDifficultyMultiplier);
        }

        return Mathf.Min(count, maxEnemiesPerWave);
    }

    private float CalculateSpawnInterval(int waveNumber)
    {
        float interval = baseSpawnInterval - (spawnIntervalDecrease * waveNumber);
        return Mathf.Max(interval, minSpawnInterval);
    }

    private float CalculateStartTimer(int waveNumber)
    {
        float timer = timeBetweenWaves - (timeBetweenWavesDecrease * waveNumber);
        return Mathf.Max(timer, minTimeBetweenWaves);
    }

    private List<ZombieSpawnOption> GenerateZombieSpawnOptions(int waveNumber)
    {
        List<ZombieSpawnOption> options = new List<ZombieSpawnOption>();

        foreach (var zombieType in zombieTypes)
        {
            // Check if this zombie type should be available at this wave
            if (waveNumber >= zombieType.unlockAtWave)
            {
                // Calculate weight based on wave progression
                int weight = CalculateZombieWeight(zombieType, waveNumber);

                if (weight > 0)
                {
                    options.Add(new ZombieSpawnOption
                    {
                        zombiePrefab = zombieType.zombiePrefab,
                        weight = weight
                    });
                }
            }
        }

        // Fallback: if no zombies available, add the first available one
        if (options.Count == 0 && zombieTypes.Count > 0)
        {
            var firstZombie = zombieTypes[0];
            options.Add(new ZombieSpawnOption
            {
                zombiePrefab = firstZombie.zombiePrefab,
                weight = firstZombie.baseWeight
            });
        }

        return options;
    }

    private int CalculateZombieWeight(EndlessZombieType zombieType, int waveNumber)
    {
        int wavesActive = waveNumber - zombieType.unlockAtWave;

        // Base weight modified by progression
        float weight = zombieType.baseWeight;

        // Apply weight progression over time
        if (zombieType.weightProgression != ZombieWeightProgression.Constant)
        {
            switch (zombieType.weightProgression)
            {
                case ZombieWeightProgression.Increasing:
                    weight += wavesActive * zombieType.weightChangePerWave;
                    break;
                case ZombieWeightProgression.Decreasing:
                    weight -= wavesActive * zombieType.weightChangePerWave;
                    break;
                case ZombieWeightProgression.PeakAndDecline:
                    if (wavesActive < zombieType.peakWave)
                        weight += wavesActive * zombieType.weightChangePerWave;
                    else
                        weight -= (wavesActive - zombieType.peakWave) * zombieType.weightChangePerWave;
                    break;
            }
        }

        return Mathf.Max(0, Mathf.RoundToInt(weight));
    }

}

[System.Serializable]
public class EndlessZombieType
{
    [Header("Zombie Configuration")]
    public GameObject zombiePrefab;
    public string zombieName;

    [Header("Unlock Settings")]
    [Tooltip("Wave number when this zombie type becomes available")]
    public int unlockAtWave = 0;

    [Header("Weight Settings")]
    [Tooltip("Base spawn weight for this zombie type")]
    public int baseWeight = 10;

    [Tooltip("How the weight changes over time")]
    public ZombieWeightProgression weightProgression = ZombieWeightProgression.Constant;

    [Tooltip("How much weight changes per wave")]
    public float weightChangePerWave = 1f;

    [Tooltip("For PeakAndDecline progression: wave where weight peaks")]
    public int peakWave = 10;
}


public enum ZombieWeightProgression
{
    Constant,        // Weight stays the same
    Increasing,      // Weight increases over time
    Decreasing,      // Weight decreases over time
    PeakAndDecline   // Weight increases then decreases
}