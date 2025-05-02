using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveSet", menuName = "Waves/New Wave Set")]
public class WaveSet : ScriptableObject
{
    public bool isInfinite;
    public WaveData[] predefinedWaves;

    public WaveData GenerateWave(int waveIndex)
    {
        if (!isInfinite)
        {
            if (waveIndex < predefinedWaves.Length)
                return predefinedWaves[waveIndex];
            return null;
        }

        // Dynamically generate an empty shell wave (for survival mode only)
        var wave = ScriptableObject.CreateInstance<WaveData>();
        wave.name = $"Wave {waveIndex + 1}";
        wave.numberOfEnemies = Mathf.Min(5 + waveIndex * 2, 100);
        wave.spawnInterval = Mathf.Max(0.3f, 1.2f - waveIndex * 0.05f);
        wave.waveEvents = new List<WaveEventBase>();
        wave.zombieSpawnOptions = new List<ZombieSpawnOption>(); // <- Leave empty unless survival logic is needed

        return wave;
    }
}
