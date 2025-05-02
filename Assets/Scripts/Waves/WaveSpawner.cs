using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private WaveSet waveSet;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private TextMeshProUGUI waveDisplay;

    private int currentWaveIndex = 0;
    private bool isSpawning = false;
    private Coroutine waveCoroutine;
    private int holdWaveIndex = -1;

    public int CurrentWave => currentWaveIndex + 1;

    [SerializeField] private bool AutoStart = false;

    private void Start()
    {
        if (AutoStart) { StartWaves(); }
    }

    public void StartWaves()
    {
        StopWaves();

        if (isSpawning) return;

        isSpawning = true;
        currentWaveIndex = 0;
        waveCoroutine = StartCoroutine(SpawnWaveLoop());
    }

    public void StopWaves()
    {
        isSpawning = false;
        if (waveCoroutine != null)
            StopCoroutine(waveCoroutine);
    }

    public void ResumeWaves()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            waveCoroutine = StartCoroutine(SpawnWaveLoop());
        }
    }
    private IEnumerator SpawnWaveLoop()
    {
        while (true)
        {
            WaveData wave = waveSet.GenerateWave(currentWaveIndex);
            if (wave == null)
                yield break;

            DisplayWaveText(CurrentWave);

            // Trigger wave events
            if (holdWaveIndex != currentWaveIndex)
            {
                foreach (var waveEvent in wave.waveEvents)
                {
                    waveEvent?.Execute();
                }
            }
            holdWaveIndex = currentWaveIndex;

            yield return new WaitForSeconds(wave.startTimer);
            for (int i = 0; i < wave.numberOfEnemies; i++)
            {
                SpawnEnemy(wave);
                yield return new WaitForSeconds(wave.spawnInterval);
            }

            currentWaveIndex++;

            if (!waveSet.isInfinite && currentWaveIndex >= waveSet.predefinedWaves.Length)
                break;
        }

        isSpawning = false;
    }

    private void SpawnEnemy(WaveData wave)
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject selectedPrefab = GetWeightedRandomZombie(wave.zombieSpawnOptions);
        if (selectedPrefab == null) return;

        GameObject enemy = Instantiate(selectedPrefab, spawnPoint.position, Quaternion.identity);
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();

        if (agent != null && LevelManager.Instance != null)
        {
            Vector3 target = LevelManager.Instance.targetZombies.position;
            Vector3 right = LevelManager.Instance.targetZombies.right;
            float sideWidth = LevelManager.Instance.roulote.GetComponent<Renderer>().bounds.size.x;
            Vector3 randomPoint = EnemyNavigation.GetRandomPointOnSide(target, right, sideWidth, 0f);
            agent.SetDestination(randomPoint);
        }
    }

    private GameObject GetWeightedRandomZombie(List<ZombieSpawnOption> options)
    {
        if (options == null || options.Count == 0)
            return null;

        int totalWeight = 0;
        foreach (var option in options)
            totalWeight += option.weight;

        int randomValue = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;

        foreach (var option in options)
        {
            cumulativeWeight += option.weight;
            if (randomValue < cumulativeWeight)
                return option.zombiePrefab;
        }

        return options[0].zombiePrefab; 
    }

    private void DisplayWaveText(int waveNumber)
    {
        if (waveDisplay != null)
        {
            waveDisplay.text = $"Wave {waveNumber}";
        }
    }
}
