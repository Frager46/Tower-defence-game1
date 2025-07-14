using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level2Manager : MonoBehaviour
{
    public static Level2Manager main;

    [Header("Path Settings")]
    public Transform StartPoint;
    public Transform[] path;

    [Header("Wave Settings")]
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    public List<Wave> waves = new List<Wave>();
    public VillageHealth villageHealth;

    [Header("Wave Delay")]
    public float waveDelay = 2f;

    private int currentWave = 0;
    private bool waveInProgress = false;
    private bool gameStarted = false;
    private int enemiesInCurrentWave = 0;
    private int enemiesReachedEnd = 0;

    private void Awake()
    {
        if (main == null)
        {
            main = this;
            Debug.Log("Level2Manager: Awake called, object active: " + gameObject.activeInHierarchy);
        }
    }

    private void Start()
    {
        gameStarted = false;
        Debug.Log("Level2Manager: Start called, waves count: " + waves.Count);
        if (enemyPrefab == null) Debug.LogError("Level2Manager: enemyPrefab is null!");
        if (spawnPoint == null) Debug.LogError("Level2Manager: spawnPoint is null!");
        if (path == null || path.Length == 0) Debug.LogError("Level2Manager: path is null or empty!");
        if (villageHealth == null) Debug.LogError("Level2Manager: villageHealth is null in Start!");
        foreach (Wave wave in waves)
        {
            Debug.Log($"Level2Manager: Wave: enemyCount={wave.enemyCount}, spawnDelay={wave.spawnDelay}");
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level2Scene")
        {
            Debug.Log("Level2Manager: Level2Scene loaded");
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                if (enemy != null && enemy != gameObject)
                {
                    Debug.Log($"Level2Manager: Destroying residual enemy: {enemy.name}");
                    Destroy(enemy);
                }
            }
        }
    }

    public void StartGame()
    {
        Debug.Log("Level3Manager: StartGame called, gameStarted set to true, currentWave: " + currentWave);
        if (villageHealth != null) villageHealth.SetLevelIndex(1); // Устанавливаем индекс для Level2
        gameStarted = true;
        StartCoroutine(StartWaves());
    }

    public void StopGame()
    {
        Debug.Log("Level2Manager: StopGame called");
        StopAllCoroutines();
        currentWave = 0;
        waveInProgress = false;
        gameStarted = false;
    }

    public void NotifyEnemyReachedEnd()
    {
        enemiesReachedEnd++;
        Debug.Log($"Level2Manager: Enemy reached end, total: {enemiesReachedEnd} of {enemiesInCurrentWave}");

        if (enemiesReachedEnd >= enemiesInCurrentWave && villageHealth != null)
        {
            ApplyWaveDamage();
        }
    }

    IEnumerator StartWaves()
    {
        Debug.Log($"Level2Manager: StartWaves started, total waves: {waves.Count}, currentWave: {currentWave}");
        while (currentWave < waves.Count && gameStarted)
        {
            if (!waveInProgress)
            {
                Debug.Log($"Level2Manager: Spawning wave {currentWave + 1}");
                enemiesInCurrentWave = waves[currentWave].enemyCount;
                enemiesReachedEnd = 0;
                yield return StartCoroutine(SpawnWave(waves[currentWave]));
                currentWave++;
            }
            else
            {
                yield return null;
            }
            if (currentWave < waves.Count)
            {
                Debug.Log($"Level2Manager: Waiting {waveDelay} seconds before next wave");
                yield return new WaitForSeconds(waveDelay);
            }
        }
        if (currentWave >= waves.Count && villageHealth != null && villageHealth.GetCurrentHealth() > 0)
        {
            if (GameState.Instance != null)
            {
                GameState.Instance.CompleteLevel(2);
                Debug.Log("Level2Manager: Level 2 completed, notified GameState");
            }
        }
    }

    IEnumerator SpawnWave(Wave wave)
    {
        Debug.Log($"Level2Manager: SpawnWave started, enemyCount={wave.enemyCount}");
        waveInProgress = true;

        Coroutine enemyCoroutine = StartCoroutine(SpawnEnemies(wave.enemyCount, wave.spawnDelay));
        yield return enemyCoroutine;

        waveInProgress = false;
    }

    IEnumerator SpawnEnemies(int enemyCount, float spawnDelay)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void SpawnEnemy()
    {
        if (!gameStarted)
        {
            Debug.LogWarning("Level2Manager: SpawnEnemy skipped, game not started");
            return;
        }

        if (enemyPrefab == null) Debug.LogError("Level2Manager: enemyPrefab is null!");
        if (spawnPoint == null) Debug.LogError("Level2Manager: spawnPoint is null!");
        if (path == null || path.Length == 0) Debug.LogError("Level2Manager: path is null or empty!");

        if (enemyPrefab != null && spawnPoint != null && path != null && path.Length > 0)
        {
            Debug.Log($"Level2Manager: Spawning enemy at position {spawnPoint.position}");
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
            if (enemyMovement != null)
            {
                enemyMovement.path = path;
                Debug.Log($"Level2Manager: Enemy created: {enemy.name}");
            }
            else
            {
                Debug.LogError("Level2Manager: EnemyMovement component missing on prefab!");
            }
        }
    }

    void ApplyWaveDamage()
    {
        if (villageHealth != null)
        {
            if (currentWave == 1)
            {
                Debug.Log("Level2Manager: First wave passed, no damage to Village.");
            }
            else if (currentWave == 2)
            {
                int halfHealth = villageHealth.GetCurrentHealth() / 2;
                villageHealth.TakeDamage(halfHealth);
                Debug.Log("Level2Manager: Second wave passed, Village health halved.");
            }
            else if (currentWave == 3)
            {
                villageHealth.TakeDamage(villageHealth.GetCurrentHealth());
                Debug.Log("Level2Manager: Third wave passed, Village destroyed.");
            }
        }
    }
}