using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using Unity.VisualScripting;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private WaveSet waveSet;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private TextMeshProUGUI waveDisplay;
    [SerializeField] private TextMeshProUGUI timeDisplay;

    private int currentWaveIndex = 0;
    private bool isSpawning = false;
    private Coroutine waveCoroutine;
    private int holdWaveIndex = -1;
    private List<GameObject> activeZombies = new List<GameObject>();

    public int CurrentWave => currentWaveIndex + 1;

    [SerializeField] private bool AutoStart = false;
    [SerializeField] private float CountTime = 0;
    [SerializeField] private bool CountOn = true;
    public void Update()
    {
        if (CountOn)
        {
            if (CountTime > 0)
            {
                CountTime -= Time.deltaTime;
                timeDisplay.text = $"{(int)CountTime}";
            }
            else if (CountTime <= 0f)
            {
                timeDisplay.text = $"{0}";
                CountOn = false;
            }
        }

    }
    private void Start()
    {
        if (GameManager.Instance != null)
        {
            currentWaveIndex = GameManager.Instance.lastWaveIndex;
        }

        if (AutoStart)
        {
            StartWaves();
        }
    }

    public void StartWaves()
    {
        StopWaves();

        if (isSpawning) return;

        isSpawning = true;
        currentWaveIndex = 0;
        waveCoroutine = StartCoroutine(SpawnWaveLoop());
    }


    public void Restart()
    {
        StopWaves();
        if (isSpawning) return;

        // Destroy all active zombies
        foreach (GameObject zombie in activeZombies)
        {
            if (zombie != null)
                Destroy(zombie);
        }
        activeZombies.Clear();

        /*
        Transform playerTransform = Camera.main?.transform; // Or use a direct reference to the player object
        if (playerTransform != null && LevelManager.Instance != null)
        {
            playerTransform.position = LevelManager.Instance.playerStartPosition.position;
            playerTransform.rotation = LevelManager.Instance.playerStartPosition.rotation;
        }*/

        isSpawning = true;
        currentWaveIndex = GameManager.Instance.lastWaveIndex;
        waveCoroutine = StartCoroutine(SpawnWaveLoop());
    }

    public void StopWaves()
    {
        isSpawning = false;
        if (waveCoroutine != null)
            StopCoroutine(waveCoroutine);
        GameManager.Instance.SaveProgress(currentWaveIndex);
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

            CountTime = wave.startTimer;
            CountOn = true;
            yield return new WaitForSeconds(wave.startTimer);

            for (int i = 0; i < wave.numberOfEnemies; i++)
            {
                SpawnEnemy(wave);
                yield return new WaitForSeconds(wave.spawnInterval);
            }

            while (activeZombies.Count > 0)
            {
                yield return null;
            }

            currentWaveIndex++;

            if (!waveSet.isInfinite && currentWaveIndex >= waveSet.predefinedWaves.Length)
                break;
        }
        GameManager.Instance.SaveProgress(currentWaveIndex);
        isSpawning = false;
    }

    private void SpawnEnemy(WaveData wave)
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject selectedPrefab = GetWeightedRandomZombie(wave.zombieSpawnOptions);
        if (selectedPrefab == null) return;

        GameObject enemy = Instantiate(selectedPrefab, spawnPoint.position, Quaternion.identity);
        activeZombies.Add(enemy);
        enemy.GetComponent<Zombie>()?.Died.AddListener(() => OnZombieDeath(enemy));
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
    private void OnZombieDeath(GameObject zombie)
    {
        activeZombies.Remove(zombie);
    }
}
