using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level3Manager : MonoBehaviour
{
    public static Level3Manager main;

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
            Debug.Log("Level3Manager: Awake called, object active: " + gameObject.activeInHierarchy + ", main set to: " + main.name);
        }
        else if (main != this)
        {
            Debug.LogWarning("Level3Manager: Duplicate instance detected, destroying self: " + gameObject.name);
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gameStarted = false;
        Debug.Log("Level3Manager: Start called, waves count: " + (waves != null ? waves.Count : 0) + ", gameObject: " + gameObject.name);
        if (enemyPrefab == null) Debug.LogError("Level3Manager: enemyPrefab is null in Start!");
        if (spawnPoint == null) Debug.LogError("Level3Manager: spawnPoint is null in Start!");
        if (path == null || path.Length == 0) Debug.LogError("Level3Manager: path is null or empty in Start! Length: " + (path != null ? path.Length : 0));
        if (villageHealth == null) Debug.LogError("Level3Manager: villageHealth is null in Start!");
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("Level3Manager: SceneManager.sceneLoaded subscribed");
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("Level3Manager: OnDestroy called, unsubscribed from SceneManager.sceneLoaded");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level3Scene")
        {
            Debug.Log("Level3Manager: Level3Scene loaded, mode: " + mode + ", gameObject: " + gameObject.name);
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            Debug.Log("Level3Manager: Found " + (enemies != null ? enemies.Length : 0) + " enemies with tag 'Enemy'");
            foreach (GameObject enemy in enemies)
            {
                if (enemy != null && enemy != gameObject)
                {
                    Destroy(enemy);
                    Debug.Log("Level3Manager: Destroyed existing enemy: " + (enemy != null ? enemy.name : "null"));
                }
            }
        }
    }

    public void StartGame()
    {
        Debug.Log("Level3Manager: StartGame called, gameStarted set to true, currentWave: " + currentWave);
        gameStarted = true;
        StartCoroutine(StartWaves());
    }

    public void StopGame()
    {
        Debug.Log("Level3Manager: StopGame called, currentWave: " + currentWave + ", waveInProgress: " + waveInProgress);
        StopAllCoroutines();
        currentWave = 0;
        waveInProgress = false;
        gameStarted = false;
        Debug.Log("Level3Manager: Game stopped, all coroutines halted");
    }

    public void NotifyEnemyReachedEnd()
    {
        enemiesReachedEnd++;
        Debug.Log($"Level3Manager: NotifyEnemyReachedEnd called, enemiesReachedEnd: {enemiesReachedEnd}, enemiesInCurrentWave: {enemiesInCurrentWave}, villageHealth: {(villageHealth != null ? "not null" : "null")}");

        if (enemiesReachedEnd >= enemiesInCurrentWave && villageHealth != null)
        {
            Debug.Log("Level3Manager: All enemies reached end, applying wave damage");
            ApplyWaveDamage();
        }
    }

    IEnumerator StartWaves()
    {
        Debug.Log($"Level3Manager: StartWaves started, total waves: {(waves != null ? waves.Count : 0)}, currentWave: {currentWave}, gameStarted: {gameStarted}");
        while (currentWave < (waves != null ? waves.Count : 0) && gameStarted)
        {
            if (!waveInProgress)
            {
                string waveData = (waves != null && currentWave < waves.Count)
                    ? (waves[currentWave] != null ? waves[currentWave].ToString() : "null wave")
                    : "null or out of range";

                Debug.Log($"Level3Manager: Preparing to spawn wave {currentWave + 1}, wave data: {waveData}");

                if (waves != null && currentWave < waves.Count)
                {
                    enemiesInCurrentWave = waves[currentWave].enemyCount;
                    enemiesReachedEnd = 0;
                    Debug.Log($"Level3Manager: Spawning wave {currentWave + 1}, enemyCount: {enemiesInCurrentWave}");
                    yield return StartCoroutine(SpawnWave(waves[currentWave]));
                    currentWave++;
                    Debug.Log($"Level3Manager: Wave {currentWave} completed, moving to next wave");
                }
                else
                {
                    Debug.LogError("Level3Manager: Wave data is invalid or currentWave out of range");
                    yield break;
                }
            }
            else
            {
                Debug.Log("Level3Manager: Wave in progress, waiting...");
                yield return null;
            }

            if (currentWave < (waves != null ? waves.Count : 0))
            {
                Debug.Log($"Level3Manager: Waiting {waveDelay} seconds before next wave, currentWave: {currentWave}");
                yield return new WaitForSeconds(waveDelay);
            }
        }

        if (currentWave >= (waves != null ? waves.Count : 0) && villageHealth != null && villageHealth.GetCurrentHealth() > 0)
        {
            if (GameState.Instance != null)
            {
                GameState.Instance.CompleteLevel(3);
                Debug.Log("Level3Manager: Level 3 completed, notified GameState");
            }
            else
            {
                Debug.LogError("Level3Manager: GameState.Instance is null, cannot complete level");
            }
        }
        else
        {
            Debug.Log("Level3Manager: No more waves or village health <= 0, ending waves");
        }
    }

    IEnumerator SpawnWave(Wave wave)
    {
        Debug.Log($"Level3Manager: SpawnWave started, wave: {wave}, enemyCount={wave.enemyCount}, spawnDelay={wave.spawnDelay}");
        waveInProgress = true;

        Coroutine enemyCoroutine = StartCoroutine(SpawnEnemies(wave.enemyCount, wave.spawnDelay));
        yield return enemyCoroutine;

        waveInProgress = false;
        Debug.Log("Level3Manager: SpawnWave completed, waveInProgress set to false");
    }

    IEnumerator SpawnEnemies(int enemyCount, float spawnDelay)
    {
        Debug.Log($"Level3Manager: SpawnEnemies started, enemyCount={enemyCount}, spawnDelay={spawnDelay}");
        for (int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy();
            Debug.Log($"Level3Manager: Spawned enemy {i + 1} of {enemyCount}");
            yield return new WaitForSeconds(spawnDelay);
        }
        Debug.Log("Level3Manager: SpawnEnemies completed");
    }

    void SpawnEnemy()
    {
        if (!gameStarted)
        {
            Debug.LogWarning("Level3Manager: SpawnEnemy skipped, game not started");
            return;
        }

        if (enemyPrefab == null) Debug.LogError("Level3Manager: enemyPrefab is null in SpawnEnemy!");
        if (spawnPoint == null) Debug.LogError("Level3Manager: spawnPoint is null in SpawnEnemy!");
        if (path == null || path.Length == 0) Debug.LogError("Level3Manager: path is null or empty in SpawnEnemy! Length: " + (path != null ? path.Length : 0));

        if (enemyPrefab != null && spawnPoint != null && path != null && path.Length > 0)
        {
            Debug.Log($"Level3Manager: Spawning enemy at position {spawnPoint.position}, path length: {path.Length}");
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
            if (enemyMovement != null)
            {
                enemyMovement.path = path;
                Debug.Log($"Level3Manager: Enemy created: {enemy.name}, path assigned");
            }
            else
            {
                Debug.LogError("Level3Manager: EnemyMovement component missing on prefab: " + (enemy != null ? enemy.name : "null"));
            }
        }
        else
        {
            Debug.LogError("Level3Manager: Failed to spawn enemy due to null references");
        }
    }

    void ApplyWaveDamage()
    {
        if (villageHealth != null)
        {
            Debug.Log("Level3Manager: ApplyWaveDamage called, currentWave: " + currentWave + ", villageHealth: " + villageHealth.GetCurrentHealth());
            if (currentWave == 1)
            {
                Debug.Log("Level3Manager: First wave passed, no damage to Village.");
            }
            else if (currentWave == 2)
            {
                int halfHealth = villageHealth.GetCurrentHealth() / 2;
                villageHealth.TakeDamage(halfHealth);
                Debug.Log("Level3Manager: Second wave passed, Village health halved to: " + villageHealth.GetCurrentHealth());
            }
            else if (currentWave == 3)
            {
                villageHealth.TakeDamage(villageHealth.GetCurrentHealth());
                Debug.Log("Level3Manager: Third wave passed, Village destroyed, health: " + villageHealth.GetCurrentHealth());
            }
        }
        else
        {
            Debug.LogError("Level3Manager: villageHealth is null, cannot apply wave damage");
        }
    }
}