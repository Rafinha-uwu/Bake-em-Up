using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Waves/Wave Data")]
public class WaveData : ScriptableObject
{
    public int numberOfEnemies = 5;
    public float spawnInterval = 1f;
    public float startTimer = 10f;

    [Header("Events to trigger when this wave starts")]
    public List<WaveEventBase> waveEvents = new List<WaveEventBase>();

    [Header("Enemy Spawn Configuration")]
    public List<ZombieSpawnOption> zombieSpawnOptions = new List<ZombieSpawnOption>();

    public void TriggerEvents()
    {
        foreach (var evt in waveEvents)
        {
            evt?.Execute();
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        float total = 0;
        foreach (var z in zombieSpawnOptions) total += z.weight;
        if (total <= 0)
        {
            Debug.LogWarning($"[WaveData] Total zombie spawn weight is 0 in '{this.name}'");
        }
    }
#endif
}
