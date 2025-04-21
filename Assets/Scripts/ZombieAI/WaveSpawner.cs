using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    public int enemyCount;

    public GameObject[] enemyPrefab;
    public Transform[] spawnPoints;
    public int waveSize = 5;
    public float timeBetweenWaves = 3.0f;
    public float waveSpawnSpeed = 1.0f;
    private int waveNumber;

    private float sideWidth;
    private int changeZombie;

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
        Vector3 randomPoint = EnemyNavigation.GetRandomPointOnSide(LevelManager.Instance.targetZombies.position, LevelManager.Instance.targetZombies.right, sideWidth, 0f);

        GameObject enemy = Instantiate(enemyPrefab[changeZombie], element.position, Quaternion.identity);
        if (changeZombie == 0)
        {
            changeZombie += 1;
        } else if(changeZombie == enemyPrefab.Length -1)
        {
            changeZombie -= 1;
        }
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
