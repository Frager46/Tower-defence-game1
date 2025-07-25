﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level2Manager : MonoBehaviour
{
    public static Level2Manager main;

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
            Debug.Log($"Level2Manager: Awake called, object active: {gameObject.activeInHierarchy}, main set to: {gameObject.name}, scene: {SceneManager.GetActiveScene().name}");
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            Debug.Log($"Level2Manager: Found {allObjects.Length} GameObjects in scene");
            foreach (var obj in allObjects)
            {
                if (obj.activeInHierarchy) Debug.Log($"Level2Manager: Active GameObject: {obj.name}");
            }
        }
        else if (main != this)
        {
            Debug.LogWarning($"Level2Manager: Duplicate instance detected, destroying self: {gameObject.name}");
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("Level2Manager: SceneManager.sceneLoaded subscribed");
    }

    private void Start()
    {
        Debug.Log($"Level2Manager: Start called, waves count: {(waves != null ? waves.Count : 0)}, gameObject: {gameObject.name}, scene: {SceneManager.GetActiveScene().name}");
        if (enemyPrefab == null) Debug.LogError("Level2Manager: enemyPrefab is null!");
        if (spawnPoint == null) Debug.LogError("Level2Manager: spawnPoint is null!");
        if (path == null || path.Length == 0) Debug.LogError("Level2Manager: path is null or empty! Length: " + (path != null ? path.Length : 0));
        if (villageHealth == null) Debug.LogError("Level2Manager: villageHealth is null in Start!");
        if (StartPoint != spawnPoint) Debug.LogWarning("Level2Manager: StartPoint and spawnPoint are different! Ensure path[0] matches spawnPoint.");
        foreach (Wave wave in waves)
        {
            Debug.Log($"Level2Manager: Wave: enemyCount={wave.enemyCount}, spawnDelay={wave.spawnDelay}");
        }
        ResetManager();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("Level2Manager: OnDestroy called, unsubscribed from SceneManager.sceneLoaded");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level2Scene")
        {
            Debug.Log($"Level2Manager: Level2Scene loaded, mode: {mode}, main: {(main != null ? main.gameObject.name : "null")}");
            ResetManager();
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            Debug.Log($"Level2Manager: Found {enemies.Length} enemies with tag 'Enemy'");
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

    private void ResetManager()
    {
        Debug.Log("Level2Manager: ResetManager called");
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
            villageHealth.SetLevelIndex(1);
            if (!villageHealth.IsVillageDestroyed())
            {
                villageHealth.ResetHealth();
                Debug.Log("Level2Manager: villageHealth reset and SetLevelIndex(1) called");
            }
            else
            {
                Debug.Log("Level2Manager: villageHealth not reset because village is destroyed");
            }
        }
        else
        {
            Debug.LogError("Level2Manager: villageHealth is null in ResetManager!");
        }
        if (GameState.Instance != null)
        {
            GameState.Instance.ResetLevel(2);
            Debug.Log("Level2Manager: GameState.ResetLevel(2) called");
        }
        else
        {
            Debug.LogError("Level2Manager: GameState.Instance is null in ResetManager!");
        }
    }

    public void StartGame()
    {
        Debug.Log($"Level2Manager: StartGame called, currentWave: {currentWave}, scene: {SceneManager.GetActiveScene().name}");
        ResetManager();
        if (villageHealth != null)
        {
            villageHealth.SetLevelIndex(1);
            villageHealth.ResetVillageDestroyed();
            villageHealth.ResetHealth();
            Debug.Log("Level2Manager: villageHealth.SetLevelIndex(1) and ResetVillageDestroyed called");
        }
        else
        {
            Debug.LogError("Level2Manager: villageHealth is null in StartGame!");
        }
        gameStarted = true;
        if (waves != null && waves.Count > 0)
        {
            StartCoroutine(StartWaves());
        }
        else
        {
            Debug.LogError("Level2Manager: waves is null or empty, cannot start waves!");
        }
    }

    public void StopGame()
    {
        Debug.Log("Level2Manager: StopGame called");
        gameStarted = false;
        StopAllCoroutines();
        Debug.Log("Level2Manager: Game stopped, all coroutines halted");
    }

    public void NotifyEnemyReachedEnd(GameObject enemy)
    {
        if (enemy != null && activeEnemies.Contains(enemy))
        {
            enemiesReachedEnd++;
            enemiesProcessed++;
            activeEnemies.Remove(enemy);
            Debug.Log($"Level2Manager: NotifyEnemyReachedEnd called, enemiesReachedEnd={enemiesReachedEnd}, enemiesProcessed={enemiesProcessed}, enemiesInCurrentWave={enemiesInCurrentWave}, currentWave={currentWave}, activeEnemies={activeEnemies.Count}");
            if (villageHealth != null)
            {
                villageHealth.TakeDamage(10);
            }
            else
            {
                Debug.LogError("Level2Manager: villageHealth is null in NotifyEnemyReachedEnd!");
            }
            if (LevelsManager.Instance != null)
            {
                LevelsManager.Instance.NotifyEnemyReachedEnd();
                Debug.Log("Level2Manager: Notified LevelsManager of enemy reaching end");
            }
            else
            {
                Debug.LogError("Level2Manager: LevelsManager.Instance is null in NotifyEnemyReachedEnd!");
            }
        }
        else
        {
            Debug.LogWarning($"Level2Manager: NotifyEnemyReachedEnd called for invalid enemy: {enemy?.name}, activeEnemies contains: {activeEnemies.Contains(enemy)}");
        }
    }

    public void NotifyEnemyDestroyed(GameObject enemy)
    {
        if (enemy != null && activeEnemies.Contains(enemy))
        {
            enemiesProcessed++;
            activeEnemies.Remove(enemy);
            Debug.Log($"Level2Manager: NotifyEnemyDestroyed called, enemiesProcessed={enemiesProcessed}, enemiesInCurrentWave={enemiesInCurrentWave}, currentWave={currentWave}, activeEnemies={activeEnemies.Count}");
        }
        else
        {
            Debug.LogWarning($"Level2Manager: NotifyEnemyDestroyed called for invalid enemy: {enemy?.name}, activeEnemies contains: {activeEnemies.Contains(enemy)}");
        }
    }

    private IEnumerator StartWaves()
    {
        Debug.Log($"Level2Manager: StartWaves started, total waves: {(waves != null ? waves.Count : 0)}, currentWave: {currentWave}, scene: {SceneManager.GetActiveScene().name}");
        while (currentWave < (waves != null ? waves.Count : 0) && gameStarted)
        {
            if (!waveInProgress)
            {
                Debug.Log($"Level2Manager: Spawning wave {currentWave + 1}");
                enemiesInCurrentWave = waves[currentWave].enemyCount;
                enemiesReachedEnd = 0;
                enemiesProcessed = 0;
                activeEnemies.Clear();
                yield return StartCoroutine(SpawnWave(waves[currentWave]));
                while (enemiesProcessed < enemiesInCurrentWave && gameStarted)
                {
                    Debug.Log($"Level2Manager: Waiting for wave {currentWave + 1} to complete, enemiesProcessed={enemiesProcessed}, enemiesInCurrentWave={enemiesInCurrentWave}, activeEnemies={activeEnemies.Count}");
                    yield return null;
                }
                Debug.Log($"Level2Manager: Wave {currentWave + 1} completed, applying damage");
                if (villageHealth != null)
                {
                    ApplyWaveDamage();
                    if (villageHealth.GetCurrentHealth() <= 0)
                    {
                        Debug.Log($"Level2Manager: Village destroyed, waiting {loseSceneTransitionDelay} seconds before showing result panel");
                        StopGame();
                        yield return new WaitForSeconds(loseSceneTransitionDelay);
                        if (LevelsManager.Instance != null)
                        {
                            LevelsManager.Instance.NotifyLevelCompleted(villageHealth.IsVillageDestroyed());
                            Debug.Log("Level2Manager: Notified LevelsManager of level completion (village destroyed)");
                        }
                        else
                        {
                            Debug.LogError("Level2Manager: LevelsManager.Instance is null, cannot notify level completion");
                            SceneManager.LoadScene("MapScene");
                        }
                        gameStarted = false; // Ensure game stops
                        yield break;
                    }
                }
                currentWave++;
                if (currentWave < (waves != null ? waves.Count : 0))
                {
                    Debug.Log($"Level2Manager: Waiting {waveDelay} seconds before next wave");
                    yield return new WaitForSeconds(waveDelay);
                }
            }
            else
            {
                Debug.Log("Level2Manager: Wave in progress, waiting...");
                yield return null;
            }
        }
        if (currentWave >= (waves != null ? waves.Count : 0) && villageHealth != null && villageHealth.GetCurrentHealth() > 0)
        {
            if (GameState.Instance != null)
            {
                GameState.Instance.CompleteLevel(2);
                Debug.Log("Level2Manager: Level 2 completed, notified GameState");
            }
            else
            {
                Debug.LogError("Level2Manager: GameState.Instance is null, cannot complete level");
            }
            if (LevelsManager.Instance != null)
            {
                LevelsManager.Instance.NotifyLevelCompleted(villageHealth.IsVillageDestroyed());
                Debug.Log("Level2Manager: Notified LevelsManager of level completion (village not destroyed)");
            }
            else
            {
                Debug.LogError("Level2Manager: LevelsManager.Instance is null, cannot notify level completion");
                SceneManager.LoadScene("MapScene");
            }
            gameStarted = false; // Ensure game stops
        }
        else
        {
            Debug.Log("Level2Manager: No more waves or village health <= 0, ending waves");
            gameStarted = false; // Ensure game stops
        }
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        Debug.Log($"Level2Manager: SpawnWave started, enemyCount={wave.enemyCount}, spawnDelay={wave.spawnDelay}, wave index={currentWave}");
        waveInProgress = true;

        yield return StartCoroutine(SpawnEnemies(wave.enemyCount, wave.spawnDelay));

        waveInProgress = false;
        Debug.Log("Level2Manager: SpawnWave completed, waveInProgress set to false");
    }

    private IEnumerator SpawnEnemies(int enemyCount, float spawnDelay)
    {
        Debug.Log($"Level2Manager: SpawnEnemies started, enemyCount={enemyCount}, spawnDelay={spawnDelay}");
        for (int i = 0; i < enemyCount; i++)
        {
            if (gameStarted)
            {
                SpawnEnemy();
                Debug.Log($"Level2Manager: Spawned enemy {i + 1} of {enemyCount}");
                yield return new WaitForSeconds(spawnDelay);
            }
        }
        Debug.Log("Level2Manager: SpawnEnemies completed");
    }

    private void SpawnEnemy()
    {
        if (!gameStarted)
        {
            Debug.LogWarning("Level2Manager: SpawnEnemy skipped, game not started");
            return;
        }

        if (enemyPrefab == null)
        {
            Debug.LogError("Level2Manager: enemyPrefab is null!");
            return;
        }
        if (spawnPoint == null)
        {
            Debug.LogError("Level2Manager: spawnPoint is null!");
            return;
        }
        if (path == null || path.Length == 0)
        {
            Debug.LogError("Level2Manager: path is null or empty! Length: " + (path != null ? path.Length : 0));
            return;
        }

        Debug.Log($"Level2Manager: Spawning enemy at position {spawnPoint.position}, path[0]: {path[0].position}");
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
        if (enemyMovement != null)
        {
            enemyMovement.path = path;
            activeEnemies.Add(enemy);
            Debug.Log($"Level2Manager: Enemy created: {enemy.name}, path assigned, activeEnemies={activeEnemies.Count}");
        }
        else
        {
            Debug.LogError($"Level2Manager: EnemyMovement component missing on prefab: {enemy?.name}");
            Destroy(enemy);
        }
    }

    private void ApplyWaveDamage()
    {
        if (villageHealth != null)
        {
            Debug.Log($"Level2Manager: ApplyWaveDamage for wave {currentWave + 1}, currentHealth={villageHealth.GetCurrentHealth()}");
            if (currentWave == 0)
            {
                Debug.Log("Level2Manager: First wave passed, no damage to Village.");
            }
            else if (currentWave == 1)
            {
                int halfHealth = villageHealth.GetCurrentHealth() / 2;
                Debug.Log($"Level2Manager: Second wave passed, applying damage={halfHealth}");
                villageHealth.TakeDamage(halfHealth);
                Debug.Log("Level2Manager: Village health halved.");
            }
            else if (currentWave == 2)
            {
                int currentHealth = villageHealth.GetCurrentHealth();
                Debug.Log($"Level2Manager: Third wave passed, applying damage={currentHealth}");
                villageHealth.TakeDamage(currentHealth);
                Debug.Log("Level2Manager: Village destroyed.");
            }
        }
        else
        {
            Debug.LogError("Level2Manager: villageHealth is null in ApplyWaveDamage!");
        }
    }
}