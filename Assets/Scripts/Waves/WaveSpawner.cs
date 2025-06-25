using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives;
using Yarn.Unity;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private TextMeshProUGUI waveDisplay;
    [SerializeField] private TextMeshProUGUI timeDisplay;
    [SerializeField] private TextMeshProUGUI zombies_remainingDisplay;

    [SerializeField] private WaveSet waveSet; // Para modo normal
    [SerializeField] private EndlessWaveSet endlessWaveSet; // Para endless mode
    [SerializeField] private bool useEndlessMode = false;

    private bool _isEndlessActive = false;
    private int _endlessWavesSurvived = 0;

    [SerializeField]
    private List<RecipeData> _bakedRecipeToStart = new();

    private int currentWaveIndex = 0;
    private bool isSpawning = true;
    private bool _finishedEnemies = true;
    private List<GameObject> activeZombies = new();
    private AudioSource _audioSource;
    public AudioSource audioSource2;

    [SerializeField] private AudioClip cookingMusic_sound;
    [SerializeField] private AudioClip normalWaveMusic_sound;
    [SerializeField] private AudioClip lastWaveMusic_sound;

    public int CurrentWave => currentWaveIndex + 1;

    public GameObject garageDoor;

    [SerializeField]
    private GameObject tutorial_manager;

    [SerializeField]
    private GameObject dialogue_system;

    [SerializeField] private bool AutoStart = false;
    private float _countTime = 0;
    private bool _countOn = true;

    [SerializeField] private GameObject blackout;

    public bool End = false;

    private float zombiesKilled;

    public bool Military;
    public float ZombieTreshHold;

    private void Awake()
    {
        useEndlessMode = PlayerPrefs.GetInt("IsEndlessMode", 0) == 1;
        if (useEndlessMode)
        {
            tutorial_manager.SetActive(false);
            dialogue_system.SetActive(false);
        }
    }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        

        if (useEndlessMode)
        {
            Debug.Log("Modo Endless ativado!");
            // Sua lógica para endless mode
        }
        LevelEvents.OnBakedNewRecipe += CheckBakedRecipe;
        LevelManager.Instance.WaveSpawner = this;

        // Choose which wave set to use
        if (useEndlessMode && endlessWaveSet != null)
        {
            waveSet = endlessWaveSet;
            _isEndlessActive = true;
            currentWaveIndex = 0; // Always start from 0 in endless mode
        }
        else if (GameManager.Instance != null)
        {
            currentWaveIndex = GameManager.Instance.lastWaveIndex;
            if (currentWaveIndex == 2)
                currentWaveIndex = 0;
        }

        if (waveSet.isInfinite)
        {
            StartWave();
            // No events in endless mode - wave events removed
        }

        if (AutoStart)
        {
            StartWave();
        }

        DisplayWaveText(CurrentWave);

        // Only save progress if not in endless mode
        if (!_isEndlessActive && GameManager.Instance != null)
        {
            GameManager.Instance.SaveProgress(SceneManager.GetActiveScene().name, currentWaveIndex);
        }
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


    public void StartWave(WaveData? wavedata = null)
    {
        WaveData wave = waveSet.GenerateWave(currentWaveIndex);
        if (!CheckCanStartWave(wave)) return;

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
        Debug.Log("Acabou os zombies: " + currentWaveIndex);
        _finishedEnemies = true;
        currentWaveIndex++;
        zombiesKilled = 0;

        if (_isEndlessActive)
        {
            _endlessWavesSurvived++;

            // Check if roulotte is still alive
            if (IsRoullotteDestroyed())
            {
                EndEndlessMode();
                return;
            }

            // Continue endless mode
            DisplayWaveText(CurrentWave);
            StartWave();
            return;
        }

        // Original wave mode logic
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveProgress(SceneManager.GetActiveScene().name, currentWaveIndex);
        }

        DisplayWaveText(CurrentWave);
        CloseDoor();

        if (currentWaveIndex == 2)
        {
            if (!blackout.IsUnityNull())
                blackout.GetComponent<Animator>().Play("Dark");
            Invoke(nameof(LoadScene), 5);
            return;
        }

        LevelEvents.PhonesStartRinging();
    }

    private bool IsRoullotteDestroyed()
    {
        // Assumindo que tens um component de vida na roulotte
        if (LevelManager.Instance != null && LevelManager.Instance.roulote_object != null)
        {
            var healthComponent = LevelManager.Instance.roulote_object.GetComponent<RoulotteHealth>();
            if (healthComponent != null)
            {
                return healthComponent.hp <= 0;
            }
        }
        return false;
    }

    private void EndEndlessMode()
    {
        Debug.Log($"Endless Mode Ended! Waves Survived: {_endlessWavesSurvived}");

        // Save high score
        SaveEndlessScore();

        // Show game over screen or load main menu
        CloseDoor();
        if (!blackout.IsUnityNull())
            blackout.GetComponent<Animator>().Play("Dark");

        Invoke(nameof(LoadEndlessGameOver), 3f);
    }

    private void SaveEndlessScore()
    {
        if (GameManager.Instance != null)
        {
            int currentBest = PlayerPrefs.GetInt("EndlessBestScore", 0);
            if (_endlessWavesSurvived > currentBest)
            {
                PlayerPrefs.SetInt("EndlessBestScore", _endlessWavesSurvived);
                PlayerPrefs.Save();
                Debug.Log($"New Endless Mode High Score: {_endlessWavesSurvived}");
            }
        }
    }

    private void LoadEndlessGameOver()
    {
        // Load game over scene or main menu
        SceneManager.LoadScene("GameOver"); // Substitui pelo nome da tua scene
    }


    private void CheckBakedRecipe(RecipeData recipe)
    {
        if (!_finishedEnemies) return;

		WaveData wave = waveSet.GenerateWave(currentWaveIndex);
		if (_bakedRecipeToStart.Contains(recipe) && !wave.StartsAfterDialogue)
        {
            StartWave();
			LevelManager.Instance.WaveStarted = true;
		}
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
            zombies_remainingDisplay.text = $"{(int)activeZombies.Count}";

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
            if (_isEndlessActive)
            {
                waveDisplay.text = $"Endless Wave {waveNumber}";
            }
            else
            {
                waveDisplay.text = $"Wave {waveNumber}";
            }
        }
    }
    private void OnZombieDeath(GameObject zombie)
    {
        activeZombies.Remove(zombie);

        zombies_remainingDisplay.text = $"{(int)activeZombies.Count}";
        zombiesKilled++;
        if (!isSpawning && activeZombies.Count == 0)
        {
            WaveFinished();
        }

        if (currentWaveIndex == 2 && End && zombiesKilled > ZombieTreshHold)
        {
            if (Military)
            {
                gameObject.GetComponent<EndGame>().On = true;
            }
            else
            {
                LevelManager.Instance.DialogueRunner.StartDialogue("Scientist_Cure");
                
            }

        }
    }

    public void OpenDoor()
    {
        Debug.Log("A tentar abrir a porta da garagem");
        if (!garageDoor.IsUnityNull())
        {
            garageDoor.GetComponent<Animator>().SetBool("Open", true);
            audioSource2.Play();
            ChangeNarrativeEvent.ChangeNarrator("Gameplay");
            if (currentWaveIndex == 2 && End)
            {
                LastWaveMusicSound();
            }
            else
            {
                PlayNormalWaveMusicSound();
            }
        }
    }

    public void CloseDoor()
    {
        if (!garageDoor.IsUnityNull())
        {
            garageDoor.GetComponent<Animator>().SetBool("Open", false);
            audioSource2.Play();
            PlayCookMusicSound();
        }
    }

    public void StartEndlessMode()
    {
        if (endlessWaveSet != null)
        {
            useEndlessMode = true;
            waveSet = endlessWaveSet;
            _isEndlessActive = true;
            currentWaveIndex = 0;
            _endlessWavesSurvived = 0;

            DisplayWaveText(CurrentWave);
            StartWave();
        }
    }

    // Propriedades públicas para UI
    public bool IsEndlessMode => _isEndlessActive;
    public int EndlessWavesSurvived => _endlessWavesSurvived;
    public int EndlessBestScore => PlayerPrefs.GetInt("EndlessBestScore", 0);

    // Método para obter estatísticas da wave atual (para UI)
    public EndlessWaveStats GetCurrentWaveStats()
    {
        if (_isEndlessActive && endlessWaveSet != null)
        {
            return endlessWaveSet.GetWaveStats(currentWaveIndex);
        }
        return new EndlessWaveStats();
    }

    private void PlayCookMusicSound()
    {
        _audioSource.clip = cookingMusic_sound;
        _audioSource.loop = true;
        _audioSource.Play();
    }

    private void LastWaveMusicSound()
    {
        _audioSource.clip = lastWaveMusic_sound;
        _audioSource.loop = true;
        _audioSource.Play();
    }

    private void PlayNormalWaveMusicSound()
    {
        _audioSource.clip = normalWaveMusic_sound;
        _audioSource.loop = true;
        _audioSource.Play();
    }

}
