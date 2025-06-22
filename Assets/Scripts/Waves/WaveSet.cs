using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Waves/Wave Set")]
public class WaveSet : ScriptableObject
{
    [Header("Wave Set Configuration")]
    public bool isInfinite = false;
    public string SceneToLoadWhenFinished = "";

    [Header("Normal Wave Mode")]
    [Tooltip("Pre-defined waves for normal mode")]
    public List<WaveData> waves = new List<WaveData>();

    /// <summary>
    /// Generates or retrieves a wave based on the wave index
    /// </summary>
    public virtual WaveData GenerateWave(int waveIndex)
    {
        // For normal wave sets, return from the predefined list
        if (!isInfinite && waves != null && waveIndex < waves.Count)
        {
            return waves[waveIndex];
        }

        // For infinite mode, this will be overridden by EndlessWaveSet
        if (isInfinite)
        {
            Debug.LogWarning($"[WaveSet] Infinite mode enabled but GenerateWave not overridden in {name}");
        }

        return null;
    }

    /// <summary>
    /// Get the total number of waves (for normal mode)
    /// </summary>
    public virtual int GetWaveCount()
    {
        if (isInfinite)
            return int.MaxValue; // Infinite waves

        return waves != null ? waves.Count : 0;
    }

    /// <summary>
    /// Check if there are more waves available
    /// </summary>
    public virtual bool HasMoreWaves(int currentWaveIndex)
    {
        if (isInfinite)
            return true;

        return currentWaveIndex < GetWaveCount();
    }

    private void OnValidate()
    {
        // Validation for normal wave mode
        if (!isInfinite && waves != null)
        {
            for (int i = 0; i < waves.Count; i++)
            {
                if (waves[i] == null)
                {
                    Debug.LogWarning($"[WaveSet] Wave {i} is null in {name}");
                }
            }
        }
    }
}