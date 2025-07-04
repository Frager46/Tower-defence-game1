using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    [Header("Path Settings")]
    public Transform StartPoint;
    public Transform[] path; // Path for all enemies

    [Header("Wave Settings")]
    public GameObject enemyPrefab; // Enemy prefab
    public Transform spawnPoint; // Spawn point for all enemies
    public List<Wave> waves = new List<Wave>();
    public VillageHealth villageHealth; // Reference to VillageHealth

    [Header("Wave Delay")]
    public float waveDelay = 2f; // Delay between waves

    private int currentWave = 0;
    private bool waveInProgress = false;
    private bool gameStarted = false;
    private int enemiesInCurrentWave = 0; // Number of enemies in current wave
    private int enemiesReachedEnd = 0; // Number of enemies that reached the end

    private void Awake()
    {
        if (main == null)
        {
            main = this;
            Debug.Log("LevelManager: Awake called, object active: " + gameObject.activeInHierarchy);
        }
    }

    private void Start()
    {
        gameStarted = false;
        Debug.Log("LevelManager: Start called, waves count: " + waves.Count);
        if (enemyPrefab == null) Debug.LogError("LevelManager: enemyPrefab is null!");
        if (spawnPoint == null) Debug.LogError("LevelManager: spawnPoint is null!");
        if (path == null || path.Length == 0) Debug.LogError("LevelManager: path is null or empty!");
        if (villageHealth == null) Debug.LogError("LevelManager: villageHealth is null in Start!");
        foreach (Wave wave in waves)
        {
            Debug.Log($"LevelManager: Wave: enemyCount={wave.enemyCount}, spawnDelay={wave.spawnDelay}");
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level1Scene")
        {
            Debug.Log("LevelManager: Level1Scene loaded");
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                if (enemy != null && enemy != gameObject)
                {
                    Debug.Log($"LevelManager: Destroying residual enemy: {enemy.name}");
                    Destroy(enemy);
                }
            }
        }
    }

    public void StartGame()
    {
        Debug.Log("LevelManager: StartGame called");
        gameStarted = true;
        StartCoroutine(StartWaves());
    }

    public void StopGame()
    {
        Debug.Log("LevelManager: StopGame called");
        StopAllCoroutines();
        currentWave = 0;
        waveInProgress = false;
        gameStarted = false;
    }

    public void NotifyEnemyReachedEnd()
    {
        enemiesReachedEnd++;
        Debug.Log($"LevelManager: Enemy reached end, total: {enemiesReachedEnd} of {enemiesInCurrentWave}");

        if (enemiesReachedEnd >= enemiesInCurrentWave && villageHealth != null)
        {
            ApplyWaveDamage();
        }
    }

    IEnumerator StartWaves()
    {
        Debug.Log($"LevelManager: StartWaves started, total waves: {waves.Count}, currentWave: {currentWave}");
        while (currentWave < waves.Count && gameStarted)
        {
            if (!waveInProgress)
            {
                Debug.Log($"LevelManager: Spawning wave {currentWave + 1}");
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
                Debug.Log($"LevelManager: Waiting {waveDelay} seconds before next wave");
                yield return new WaitForSeconds(waveDelay);
            }
        }
        if (currentWave >= waves.Count && villageHealth != null && villageHealth.GetCurrentHealth() > 0)
        {
            if (GameState.Instance != null)
            {
                GameState.Instance.CompleteLevel(1);
                Debug.Log("LevelManager: Level 1 completed, notified GameState");
            }
        }
    }

    IEnumerator SpawnWave(Wave wave)
    {
        Debug.Log($"LevelManager: SpawnWave started, enemyCount={wave.enemyCount}");
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
            Debug.LogWarning("LevelManager: SpawnEnemy skipped, game not started");
            return;
        }

        if (enemyPrefab == null) Debug.LogError("LevelManager: enemyPrefab is null!");
        if (spawnPoint == null) Debug.LogError("LevelManager: spawnPoint is null!");
        if (path == null || path.Length == 0) Debug.LogError("LevelManager: path is null or empty!");

        if (enemyPrefab != null && spawnPoint != null && path != null && path.Length > 0)
        {
            Debug.Log($"LevelManager: Spawning enemy at position {spawnPoint.position}");
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
            if (enemyMovement != null)
            {
                enemyMovement.path = path;
                Debug.Log($"LevelManager: Enemy created: {enemy.name}");
            }
            else
            {
                Debug.LogError("LevelManager: EnemyMovement component missing on prefab!");
            }
        }
    }

    void ApplyWaveDamage()
    {
        if (villageHealth != null)
        {
            if (currentWave == 1)
            {
                Debug.Log("LevelManager: First wave passed, no damage to Village.");
            }
            else if (currentWave == 2)
            {
                int halfHealth = villageHealth.GetCurrentHealth() / 2;
                villageHealth.TakeDamage(halfHealth);
                Debug.Log("LevelManager: Second wave passed, Village health halved.");
            }
            else if (currentWave == 3)
            {
                villageHealth.TakeDamage(villageHealth.GetCurrentHealth());
                Debug.Log("LevelManager: Third wave passed, Village destroyed.");
            }
        }
    }
}

[System.Serializable]
public class Wave
{
    public int enemyCount;
    public float spawnDelay = 1f;
}