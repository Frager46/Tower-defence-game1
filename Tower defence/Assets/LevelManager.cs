using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    [System.Serializable]
    public class Wave
    {
        public int enemyCount;
        public float spawnDelay;
    }

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
    public float loseSceneTransitionDelay = 2f; // Delay before showing result panel

    private int currentWave = 0;
    private bool waveInProgress = false;
    private bool gameStarted = false;
    private int enemiesInCurrentWave = 0;
    private int enemiesReachedEnd = 0;
    private int enemiesProcessed = 0;
    private List<GameObject> activeEnemies = new List<GameObject>();

    private void Awake()
    {
        if (main == null)
        {
            main = this;
            Debug.Log($"LevelManager: Awake called, object active: {gameObject.activeInHierarchy}, main set to: {gameObject.name}, scene: {SceneManager.GetActiveScene().name}");
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            Debug.Log($"LevelManager: Found {allObjects.Length} GameObjects in scene");
            foreach (var obj in allObjects)
            {
                if (obj.activeInHierarchy) Debug.Log($"LevelManager: Active GameObject: {obj.name}");
            }
        }
        else if (main != this)
        {
            Debug.LogWarning($"LevelManager: Duplicate instance detected, destroying self: {gameObject.name}");
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("LevelManager: SceneManager.sceneLoaded subscribed");
    }

    private void Start()
    {
        Debug.Log($"LevelManager: Start called, waves count: {(waves != null ? waves.Count : 0)}, gameObject: {gameObject.name}, scene: {SceneManager.GetActiveScene().name}");
        if (enemyPrefab == null) Debug.LogError("LevelManager: enemyPrefab is null!");
        if (spawnPoint == null) Debug.LogError("LevelManager: spawnPoint is null!");
        if (path == null || path.Length == 0) Debug.LogError("LevelManager: path is null or empty! Length: " + (path != null ? path.Length : 0));
        if (villageHealth == null) Debug.LogError("LevelManager: villageHealth is null in Start!");
        if (StartPoint != spawnPoint) Debug.LogWarning("LevelManager: StartPoint and spawnPoint are different! Ensure path[0] matches spawnPoint.");
        foreach (Wave wave in waves)
        {
            Debug.Log($"LevelManager: Wave: enemyCount={wave.enemyCount}, spawnDelay={wave.spawnDelay}");
        }
        ResetManager();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("LevelManager: OnDestroy called, unsubscribed from SceneManager.sceneLoaded");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level1Scene")
        {
            Debug.Log($"LevelManager: Level1Scene loaded, mode: {mode}, main: {(main != null ? main.gameObject.name : "null")}");
            ResetManager();
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            Debug.Log($"LevelManager: Found {enemies.Length} enemies with tag 'Enemy'");
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

    private void ResetManager()
    {
        Debug.Log("LevelManager: ResetManager called");
        StopAllCoroutines();
        currentWave = 0;
        waveInProgress = false;
        gameStarted = false;
        enemiesInCurrentWave = 0;
        enemiesReachedEnd = 0;
        enemiesProcessed = 0;
        activeEnemies.Clear();
        if (villageHealth != null)
        {
            villageHealth.SetLevelIndex(0);
            if (!villageHealth.IsVillageDestroyed())
            {
                villageHealth.ResetHealth();
                Debug.Log("LevelManager: villageHealth reset and SetLevelIndex(0) called");
            }
            else
            {
                Debug.Log("LevelManager: villageHealth not reset because village is destroyed");
            }
        }
        else
        {
            Debug.LogError("LevelManager: villageHealth is null in ResetManager!");
        }
        if (GameState.Instance != null)
        {
            GameState.Instance.ResetLevel(1);
            Debug.Log("LevelManager: GameState.ResetLevel(1) called");
        }
        else
        {
            Debug.LogError("LevelManager: GameState.Instance is null in ResetManager!");
        }
    }

    public void StartGame()
    {
        Debug.Log($"LevelManager: StartGame called, currentWave: {currentWave}, scene: {SceneManager.GetActiveScene().name}");
        ResetManager();
        if (villageHealth != null)
        {
            villageHealth.SetLevelIndex(0);
            villageHealth.ResetVillageDestroyed();
            villageHealth.ResetHealth();
            Debug.Log("LevelManager: villageHealth.SetLevelIndex(0) and ResetVillageDestroyed called");
        }
        else
        {
            Debug.LogError("LevelManager: villageHealth is null in StartGame!");
        }
        gameStarted = true;
        if (waves != null && waves.Count > 0)
        {
            StartCoroutine(StartWaves());
        }
        else
        {
            Debug.LogError("LevelManager: waves is null or empty, cannot start waves!");
        }
    }

    public void StopGame()
    {
        Debug.Log("LevelManager: StopGame called");
        gameStarted = false;
        StopAllCoroutines();
        Debug.Log("LevelManager: Game stopped, all coroutines halted");
    }

    public void NotifyEnemyReachedEnd(GameObject enemy)
    {
        if (enemy != null && activeEnemies.Contains(enemy))
        {
            enemiesReachedEnd++;
            enemiesProcessed++;
            activeEnemies.Remove(enemy);
            Debug.Log($"LevelManager: NotifyEnemyReachedEnd called, enemiesReachedEnd={enemiesReachedEnd}, enemiesProcessed={enemiesProcessed}, enemiesInCurrentWave={enemiesInCurrentWave}, currentWave={currentWave}, activeEnemies={activeEnemies.Count}");
            if (villageHealth != null)
            {
                villageHealth.TakeDamage(10);
            }
            else
            {
                Debug.LogError("LevelManager: villageHealth is null in NotifyEnemyReachedEnd!");
            }
            if (LevelsManager.Instance != null)
            {
                LevelsManager.Instance.NotifyEnemyReachedEnd();
                Debug.Log("LevelManager: Notified LevelsManager of enemy reaching end");
            }
            else
            {
                Debug.LogError("LevelManager: LevelsManager.Instance is null in NotifyEnemyReachedEnd!");
            }
        }
        else
        {
            Debug.LogWarning($"LevelManager: NotifyEnemyReachedEnd called for invalid enemy: {enemy?.name}, activeEnemies contains: {activeEnemies.Contains(enemy)}");
        }
    }

    public void NotifyEnemyDestroyed(GameObject enemy)
    {
        if (enemy != null && activeEnemies.Contains(enemy))
        {
            enemiesProcessed++;
            activeEnemies.Remove(enemy);
            Debug.Log($"LevelManager: NotifyEnemyDestroyed called, enemiesProcessed={enemiesProcessed}, enemiesInCurrentWave={enemiesInCurrentWave}, currentWave={currentWave}, activeEnemies={activeEnemies.Count}");
        }
        else
        {
            Debug.LogWarning($"LevelManager: NotifyEnemyDestroyed called for invalid enemy: {enemy?.name}, activeEnemies contains: {activeEnemies.Contains(enemy)}");
        }
    }

    private IEnumerator StartWaves()
    {
        Debug.Log($"LevelManager: StartWaves started, total waves: {(waves != null ? waves.Count : 0)}, currentWave: {currentWave}, scene: {SceneManager.GetActiveScene().name}");
        while (currentWave < (waves != null ? waves.Count : 0) && gameStarted)
        {
            if (!waveInProgress)
            {
                Debug.Log($"LevelManager: Spawning wave {currentWave + 1}");
                enemiesInCurrentWave = waves[currentWave].enemyCount;
                enemiesReachedEnd = 0;
                enemiesProcessed = 0;
                activeEnemies.Clear();
                yield return StartCoroutine(SpawnWave(waves[currentWave]));
                float waveTimeout = Time.time + 60f; // Timeout after 60 seconds per wave
                while (enemiesProcessed < enemiesInCurrentWave && gameStarted)
                {
                    // Check for destroyed or null enemies
                    for (int i = activeEnemies.Count - 1; i >= 0; i--)
                    {
                        if (activeEnemies[i] == null)
                        {
                            enemiesProcessed++;
                            activeEnemies.RemoveAt(i);
                            Debug.Log($"LevelManager: Detected null enemy in activeEnemies, enemiesProcessed={enemiesProcessed}, activeEnemies={activeEnemies.Count}");
                        }
                    }
                    // Log details of remaining enemies to diagnose stuck enemies
                    if (activeEnemies.Count > 0)
                    {
                        foreach (GameObject enemy in activeEnemies)
                        {
                            if (enemy != null)
                            {
                                EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
                                Vector3 position = enemy.transform.position;
                                Debug.Log($"LevelManager: Active enemy {enemy.name} at position {position}, has EnemyMovement: {(movement != null)}");
                            }
                        }
                    }
                    // Timeout check for stuck enemies
                    if (Time.time > waveTimeout && activeEnemies.Count > 0)
                    {
                        Debug.LogWarning($"LevelManager: Wave {currentWave + 1} timed out, forcing processing of {activeEnemies.Count} remaining enemies");
                        for (int i = activeEnemies.Count - 1; i >= 0; i--)
                        {
                            if (activeEnemies[i] != null)
                            {
                                Debug.Log($"LevelManager: Force-processing stuck enemy {activeEnemies[i].name}");
                                enemiesProcessed++;
                                Destroy(activeEnemies[i]); // Destroy stuck enemies
                            }
                            else
                            {
                                enemiesProcessed++;
                            }
                            activeEnemies.RemoveAt(i);
                        }
                    }
                    Debug.Log($"LevelManager: Waiting for wave {currentWave + 1} to complete, enemiesProcessed={enemiesProcessed}, enemiesInCurrentWave={enemiesInCurrentWave}, activeEnemies={activeEnemies.Count}");
                    yield return null;
                }
                Debug.Log($"LevelManager: Wave {currentWave + 1} completed, applying damage");
                if (villageHealth != null)
                {
                    ApplyWaveDamage();
                    if (villageHealth.GetCurrentHealth() <= 0)
                    {
                        Debug.Log($"LevelManager: Village destroyed, waiting {loseSceneTransitionDelay} seconds before showing result panel");
                        StopGame();
                        yield return new WaitForSeconds(loseSceneTransitionDelay);
                        if (LevelsManager.Instance != null)
                        {
                            LevelsManager.Instance.NotifyLevelCompleted(villageHealth.IsVillageDestroyed());
                            Debug.Log("LevelManager: Notified LevelsManager of level completion (village destroyed)");
                        }
                        else
                        {
                            Debug.LogError("LevelManager: LevelsManager.Instance is null, cannot notify level completion");
                            SceneManager.LoadScene("MapScene");
                        }
                        gameStarted = false; // Ensure game stops
                        yield break;
                    }
                }
                currentWave++;
                if (currentWave < (waves != null ? waves.Count : 0))
                {
                    Debug.Log($"LevelManager: Waiting {waveDelay} seconds before next wave");
                    yield return new WaitForSeconds(waveDelay);
                }
            }
            else
            {
                Debug.Log("LevelManager: Wave in progress, waiting...");
                yield return null;
            }
        }
        if (currentWave >= (waves != null ? waves.Count : 0) && villageHealth != null && villageHealth.GetCurrentHealth() > 0)
        {
            if (GameState.Instance != null)
            {
                GameState.Instance.CompleteLevel(1);
                Debug.Log("LevelManager: Level 1 completed, notified GameState");
            }
            else
            {
                Debug.LogError("LevelManager: GameState.Instance is null, cannot complete level");
            }
            if (LevelsManager.Instance != null)
            {
                LevelsManager.Instance.NotifyLevelCompleted(villageHealth.IsVillageDestroyed());
                Debug.Log("LevelManager: Notified LevelsManager of level completion (village not destroyed)");
            }
            else
            {
                Debug.LogError("LevelManager: LevelsManager.Instance is null, cannot notify level completion");
                SceneManager.LoadScene("MapScene");
            }
            gameStarted = false; // Ensure game stops
        }
        else
        {
            Debug.Log("LevelManager: No more waves or village health <= 0, ending waves");
            gameStarted = false; // Ensure game stops
        }
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        Debug.Log($"LevelManager: SpawnWave started, enemyCount={wave.enemyCount}, spawnDelay={wave.spawnDelay}, wave index={currentWave}");
        waveInProgress = true;

        yield return StartCoroutine(SpawnEnemies(wave.enemyCount, wave.spawnDelay));

        waveInProgress = false;
        Debug.Log("LevelManager: SpawnWave completed, waveInProgress set to false");
    }

    private IEnumerator SpawnEnemies(int enemyCount, float spawnDelay)
    {
        Debug.Log($"LevelManager: SpawnEnemies started, enemyCount={enemyCount}, spawnDelay={spawnDelay}");
        for (int i = 0; i < enemyCount; i++)
        {
            if (gameStarted)
            {
                SpawnEnemy();
                Debug.Log($"LevelManager: Spawned enemy {i + 1} of {enemyCount}");
                yield return new WaitForSeconds(spawnDelay);
            }
        }
        Debug.Log("LevelManager: SpawnEnemies completed");
    }

    private void SpawnEnemy()
    {
        if (!gameStarted)
        {
            Debug.LogWarning("LevelManager: SpawnEnemy skipped, game not started");
            return;
        }

        if (enemyPrefab == null)
        {
            Debug.LogError("LevelManager: enemyPrefab is null!");
            return;
        }
        if (spawnPoint == null)
        {
            Debug.LogError("LevelManager: spawnPoint is null!");
            return;
        }
        if (path == null || path.Length == 0)
        {
            Debug.LogError("LevelManager: path is null or empty! Length: " + (path != null ? path.Length : 0));
            return;
        }

        Debug.Log($"LevelManager: Spawning enemy at position {spawnPoint.position}, path[0]: {path[0].position}");
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
        if (enemyMovement != null)
        {
            enemyMovement.path = path;
            activeEnemies.Add(enemy);
            // Ensure enemy notifies LevelManager on destruction
            Health enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.OnEnemyDestroyed += NotifyEnemyDestroyed;
                Debug.Log($"LevelManager: Enemy {enemy.name} created with Health, OnEnemyDestroyed subscribed");
            }
            else
            {
                Debug.LogWarning($"LevelManager: Enemy {enemy.name} missing Health component, destruction may not be tracked properly");
            }
            Debug.Log($"LevelManager: Enemy created: {enemy.name}, path assigned, activeEnemies={activeEnemies.Count}");
        }
        else
        {
            Debug.LogError($"LevelManager: EnemyMovement component missing on prefab: {enemy?.name}");
            Destroy(enemy);
        }
    }

    private void ApplyWaveDamage()
    {
        if (villageHealth != null)
        {
            Debug.Log($"LevelManager: ApplyWaveDamage for wave {currentWave + 1}, currentHealth={villageHealth.GetCurrentHealth()}");
            if (currentWave == 0)
            {
                Debug.Log("LevelManager: First wave passed, no damage to Village.");
            }
            else if (currentWave == 1)
            {
                int halfHealth = villageHealth.GetCurrentHealth() / 2;
                Debug.Log($"LevelManager: Second wave passed, applying damage={halfHealth}");
                villageHealth.TakeDamage(halfHealth);
                Debug.Log("LevelManager: Village health halved.");
            }
            else if (currentWave == 2)
            {
                int currentHealth = villageHealth.GetCurrentHealth();
                Debug.Log($"LevelManager: Third wave passed, applying damage={currentHealth}");
                villageHealth.TakeDamage(currentHealth);
                Debug.Log("LevelManager: Village destroyed.");
            }
        }
        else
        {
            Debug.LogError("LevelManager: villageHealth is null in ApplyWaveDamage!");
        }
    }
}