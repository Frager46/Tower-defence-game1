using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level2Manager : MonoBehaviour
{
    public static Level2Manager main;

    [Header("Path Settings")]
    public Transform StartPoint;
    public Transform[] path; // Путь для всех врагов

    [Header("Wave Settings")]
    public GameObject enemyPrefab; // Префаб врага
    public Transform spawnPoint; // Точка спавна для всех врагов
    public List<Wave> waves = new List<Wave>(); // Используем Wave из LevelManager.cs
    public VillageHealth villageHealth; // Ссылка на VillageHealth

    [Header("Wave Delay")]
    public float waveDelay = 2f; // Задержка между волнами

    private int currentWave = 0;
    private bool waveInProgress = false;
    private bool gameStarted = false;
    private int enemiesInCurrentWave = 0; // Количество врагов в текущей волне
    private int enemiesReachedEnd = 0; // Количество врагов, достигших конца

    private void Awake()
    {
        if (main == null)
        {
            main = this;
            Debug.Log("Level2Manager: Awake вызван, объект активен: " + gameObject.activeInHierarchy);
        }
    }

    private void Start()
    {
        gameStarted = false;
        Debug.Log("Level2Manager: Start вызван, waves count: " + waves.Count);
        if (enemyPrefab == null) Debug.LogError("Level2Manager: enemyPrefab is null!");
        if (spawnPoint == null) Debug.LogError("Level2Manager: spawnPoint is null!");
        if (path == null || path.Length == 0) Debug.LogError("Level2Manager: path is null or empty!");
        if (villageHealth == null) Debug.LogError("Level2Manager: villageHealth is null в Start!");
        foreach (Wave wave in waves)
        {
            Debug.Log($"Level2Manager: Wave: enemyCount={wave.enemyCount}, spawnDelay={wave.spawnDelay}");
        }
        SceneManager.sceneLoaded += OnSceneLoaded; // Подписываемся на событие загрузки сцены
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Отписываемся при уничтожении
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level2Scene")
        {
            Debug.Log("Level2Manager: Сцена Level2Scene загружена");
            // Очистка остаточных врагов
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                if (enemy != null && enemy != gameObject) // Исключаем сам Level2Manager
                {
                    Debug.Log($"Level2Manager: Уничтожение остаточного врага: {enemy.name}");
                    Destroy(enemy);
                }
            }
        }
    }

    public void StartGame()
    {
        Debug.Log("Level2Manager: StartGame вызван");
        gameStarted = true;
        StartCoroutine(StartWaves());
    }

    public void StopGame()
    {
        Debug.Log("Level2Manager: StopGame вызван");
        StopAllCoroutines();
        currentWave = 0;
        waveInProgress = false;
        gameStarted = false;
    }

    public void NotifyEnemyReachedEnd()
    {
        enemiesReachedEnd++;
        Debug.Log($"Level2Manager: Враг достиг конца пути, всего достигло: {enemiesReachedEnd} из {enemiesInCurrentWave}");

        if (enemiesReachedEnd >= enemiesInCurrentWave && villageHealth != null)
        {
            ApplyWaveDamage();
        }
    }

    IEnumerator StartWaves()
    {
        Debug.Log($"Level2Manager: StartWaves начат, всего волн: {waves.Count}, currentWave: {currentWave}");
        while (currentWave < waves.Count && gameStarted)
        {
            if (!waveInProgress)
            {
                Debug.Log($"Level2Manager: Спавним волну {currentWave + 1}");
                enemiesInCurrentWave = waves[currentWave].enemyCount; // Устанавливаем количество врагов в волне
                enemiesReachedEnd = 0; // Сбрасываем счётчик
                yield return StartCoroutine(SpawnWave(waves[currentWave]));
                currentWave++;
            }
            else
            {
                yield return null;
            }
            if (currentWave < waves.Count)
            {
                Debug.Log($"Level2Manager: Ожидание {waveDelay} секунд перед следующей волной");
                yield return new WaitForSeconds(waveDelay);
            }
        }
        if (currentWave >= waves.Count && villageHealth != null && villageHealth.GetCurrentHealth() > 0)
        {
            GameManager.Instance.CompleteLevel(2); // Уровень 2 пройден
            Debug.Log("Level2Manager: Уровень 2 завершён, уведомление GameManager");
        }
    }

    IEnumerator SpawnWave(Wave wave)
    {
        Debug.Log($"Level2Manager: SpawnWave начат, enemyCount={wave.enemyCount}");
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
            Debug.LogWarning("Level2Manager: SpawnEnemy пропущен, игра не начата");
            return;
        }

        if (enemyPrefab == null) Debug.LogError("Level2Manager: enemyPrefab is null!");
        if (spawnPoint == null) Debug.LogError("Level2Manager: spawnPoint is null!");
        if (path == null || path.Length == 0) Debug.LogError("Level2Manager: path is null or empty!");

        if (enemyPrefab != null && spawnPoint != null && path != null && path.Length > 0)
        {
            Debug.Log($"Level2Manager: Спавним врага на позиции {spawnPoint.position}");
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
            if (enemyMovement != null)
            {
                enemyMovement.path = path;
                Debug.Log($"Level2Manager: Enemy создан: {enemy.name}");
            }
            else
            {
                Debug.LogError("Level2Manager: EnemyMovement компонент отсутствует на префабе!");
            }
        }
    }

    void ApplyWaveDamage()
    {
        if (villageHealth != null)
        {
            if (currentWave == 1) // Первая волна — никакого урона
            {
                Debug.Log("Level2Manager: Первая волна прошла, урон Village не нанесён.");
            }
            else if (currentWave == 2) // Вторая волна — здоровье до половины
            {
                int halfHealth = villageHealth.GetCurrentHealth() / 2;
                villageHealth.TakeDamage(halfHealth);
                Debug.Log("Level2Manager: Вторая волна прошла, здоровье Village уменьшено до половины.");
            }
            else if (currentWave == 3) // Третья волна — полное разрушение
            {
                villageHealth.TakeDamage(villageHealth.GetCurrentHealth());
                Debug.Log("Level2Manager: Третья волна прошла, Village разрушен.");
            }
        }
    }
}