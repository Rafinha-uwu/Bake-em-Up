using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    public int enemyCount;

    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public int waveSize = 5;
    public float timeBetweenWaves = 3.0f;
    public float waveSpawnSpeed = 1.0f;
    private int waveNumber;

    private float sideWidth;

    void Start()
    {
        Renderer roulotteRenderer = LevelManager.Instance.roulote.GetComponent<Renderer>();
        if (roulotteRenderer != null)
        {
            sideWidth = roulotteRenderer.bounds.size.x; // Get world-space width
        }
        StartCoroutine(SpawnWaves());
    }


    IEnumerator SpawnWaves()
    {
        while (true)
        {
            waveNumber++;

            for (int i = 0; i < waveSize; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(waveSpawnSpeed);
            }
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    private void SpawnEnemy()
    {
        var element = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector3 randomPoint = EnemyNavigation.GetRandomPointOnSide(LevelManager.Instance.targetZombies.position, LevelManager.Instance.targetZombies.right, sideWidth, 1.5f);

        GameObject enemy = Instantiate(enemyPrefab, element.position, Quaternion.identity);
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            if (LevelManager.Instance != null)
            {
                agent.SetDestination(randomPoint);
            }
            
        }
    }
}
