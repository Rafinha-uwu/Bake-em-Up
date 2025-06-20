using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private WaveSet waveSet;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private TextMeshProUGUI waveDisplay;
    [SerializeField] private TextMeshProUGUI timeDisplay;

    [SerializeField]
    private List<RecipeData> _bakedRecipeToStart = new();

    private int currentWaveIndex = 0;
    private bool isSpawning = true;
    private bool _finishedEnemies = true;
    private List<GameObject> activeZombies = new();
    private AudioSource _audioSource;

    public int CurrentWave => currentWaveIndex + 1;

    public GameObject garageDoor;

    [SerializeField] private bool AutoStart = false;
    private float _countTime = 0;
    private bool _countOn = true;

    [SerializeField] private GameObject blackout;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        LevelEvents.OnBakedNewRecipe += CheckBakedRecipe;
        LevelManager.Instance.WaveSpawner = this;
        
        if (GameManager.Instance != null)
        {
            currentWaveIndex = GameManager.Instance.lastWaveIndex;
            if (currentWaveIndex == 2)
                currentWaveIndex = 0;
		}
		
        if (waveSet.isInfinite)
		{
			WaveData wave = waveSet.GenerateWave(0);

			foreach (var waveEvent in wave.waveEvents)
			{
				waveEvent.Execute();
			}
		}

		if (AutoStart)
        {
			StartWave();
        }

		DisplayWaveText(CurrentWave);

		GameManager.Instance.SaveProgress(SceneManager.GetActiveScene().name, currentWaveIndex);
	}

    private void OnDestroy()
    {
        LevelEvents.OnBakedNewRecipe -= CheckBakedRecipe;
    }

    public void Update()
    {
        if (waveSet.isInfinite && _countOn)
        {
            if (_countTime > 0)
            {
                _countTime -= Time.deltaTime;
                timeDisplay.text = $"{(int)_countTime}";
            }
            else if (_countTime <= 0f)
            {
                timeDisplay.text = $"{0}";
                _countOn = false;
            }
        }
    }

    public void StartWave()
    {
		WaveData wave = waveSet.GenerateWave(currentWaveIndex);
        if (!CheckCanStartWave(wave)) return;

        LevelManager.Instance.WaveStarted = true;
		_finishedEnemies = false;

        if (waveSet.isInfinite)
        {
			_countTime = wave.startTimer;
            _countOn = true;
            StartCoroutine(WaitTimeUntilStart(wave));
        }
        else
        {
		    OpenDoor();
            StartCoroutine(SpawnEnemy(wave));
        }
	}

    public void StartAfterDialogue()
    {
		WaveData wave = waveSet.GenerateWave(currentWaveIndex);
        if (wave.StartsAfterDialogue)
            StartWave();
	}

    private bool CheckCanStartWave(WaveData wave)
    {
		if (GameManager.Instance == null)
			return false;

		if (!_finishedEnemies) return false;

		if (wave.IsUnityNull())
			throw new NullReferenceException("WaveData missing");

        return true;
	}

    private IEnumerator WaitTimeUntilStart(WaveData wave)
    {
        yield return new WaitForSeconds(_countTime);
		
        OpenDoor();
		StartCoroutine(SpawnEnemy(wave));
	}

    private void WaveFinished()
    {
		_finishedEnemies = true;
		currentWaveIndex++;
		GameManager.Instance.SaveProgress(SceneManager.GetActiveScene().name, currentWaveIndex);
		DisplayWaveText(CurrentWave);

        if (waveSet.isInfinite)
        {
            StartWave();
            return;
        }

		if (currentWaveIndex == 2)
		{
			CloseDoor();
            if(!blackout.IsUnityNull())
			    blackout.GetComponent<Animator>().Play("Dark");
			Invoke(nameof(LoadScene), 5);
            return;
        }

        LevelEvents.PhonesStartRinging();
	}

    private void CheckBakedRecipe(RecipeData recipe)
    {
        if (!_finishedEnemies) return;

		WaveData wave = waveSet.GenerateWave(currentWaveIndex);
		if (_bakedRecipeToStart.Contains(recipe) && !wave.StartsAfterDialogue)
            StartWave();
    }

    private void LoadScene()
    {
        string scene = waveSet.SceneToLoadWhenFinished;

		if (!string.IsNullOrWhiteSpace(scene))
            SceneManager.LoadScene(scene);
    }

    private IEnumerator SpawnEnemy(WaveData wave)
    {
        for (int i = 0; i < wave.numberOfEnemies; i++)
        {
            Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            GameObject selectedPrefab = GetWeightedRandomZombie(wave.zombieSpawnOptions);
            if (selectedPrefab == null)
				throw new NullReferenceException("Zombie prefab missing");

            GameObject enemy = Instantiate(selectedPrefab, spawnPoint.position, Quaternion.identity);
            activeZombies.Add(enemy);

            enemy.GetComponent<Zombie>().Died.AddListener(() => OnZombieDeath(enemy));
            
            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
            if (agent != null && LevelManager.Instance != null)
            {
                Vector3 target = LevelManager.Instance.targetZombies.position;
                Vector3 right = LevelManager.Instance.targetZombies.right;
                float sideWidth = LevelManager.Instance.roulote.GetComponent<Renderer>().bounds.size.x;
                Vector3 randomPoint = EnemyNavigation.GetRandomPointOnSide(target, right, sideWidth, 0f);
                agent.SetDestination(randomPoint);
            }

			yield return new WaitForSeconds(wave.spawnInterval);
		}

		isSpawning = false;
	}

    private GameObject GetWeightedRandomZombie(List<ZombieSpawnOption> options)
    {
        if (options == null || options.Count == 0)
            return null;

        int totalWeight = 0;
        foreach (var option in options)
            totalWeight += option.weight;

        int randomValue = UnityEngine.Random.Range(0, totalWeight);
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
        Debug.Log(activeZombies.Count);
		if(!isSpawning && activeZombies.Count == 0)
		{
            WaveFinished();
		}
	}

    public void OpenDoor()
    {
        if (!garageDoor.IsUnityNull())
        {
            garageDoor.GetComponent<Animator>().SetBool("Open", true);
            _audioSource.Play();
            ChangeNarrativeEvent.ChangeNarrator("Gameplay");
        }
    }

    public void CloseDoor()
    {
        if (!garageDoor.IsUnityNull())
        {
            garageDoor.GetComponent<Animator>().SetBool("Open", false);
            _audioSource.Play();
        }
    }
}
