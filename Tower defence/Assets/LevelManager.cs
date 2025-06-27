using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    [Header("Path Settings")]
    public Transform StartPoint;
    public Transform[] path; // ���� ��� ���� ������

    [Header("Wave Settings")]
    public GameObject enemyPrefab; // ������ �����
    public Transform spawnPoint; // ����� ������ ��� ���� ������
    public List<Wave> waves = new List<Wave>();
    public VillageHealth villageHealth; // ������ �� VillageHealth

    [Header("Wave Delay")]
    public float waveDelay = 2f; // �������� ����� �������

    private int currentWave = 0;
    private bool waveInProgress = false;
    private bool gameStarted = false;
    private int enemiesInCurrentWave = 0; // ���������� ������ � ������� �����
    private int enemiesReachedEnd = 0; // ���������� ������, ��������� �����

    private void Awake()
    {
        if (main == null)
        {
            main = this;
            Debug.Log("LevelManager: Awake ������, ������ �������: " + gameObject.activeInHierarchy);
        }
    }

    private void Start()
    {
        gameStarted = false;
        Debug.Log("LevelManager: Start ������, waves count: " + waves.Count);
        if (enemyPrefab == null) Debug.LogError("enemyPrefab is null!");
        if (spawnPoint == null) Debug.LogError("spawnPoint is null!");
        if (path == null || path.Length == 0) Debug.LogError("path is null or empty!");
        if (villageHealth == null) Debug.LogError("villageHealth is null � Start!");
        foreach (Wave wave in waves)
        {
            Debug.Log($"Wave: enemyCount={wave.enemyCount}, spawnDelay={wave.spawnDelay}");
        }
        SceneManager.sceneLoaded += OnSceneLoaded; // ������������� �� ������� �������� �����
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // ������������ ��� �����������
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level1Scene")
        {
            Debug.Log("LevelManager: ����� Level1Scene ���������");
            // ������� ���������� ������
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                if (enemy != null && enemy != gameObject) // ��������� ��� LevelManager
                {
                    Debug.Log($"LevelManager: ����������� ����������� �����: {enemy.name}");
                    Destroy(enemy);
                }
            }
        }
    }

    public void StartGame()
    {
        Debug.Log("LevelManager: StartGame ������");
        gameStarted = true;
        StartCoroutine(StartWaves());
    }

    public void StopGame()
    {
        Debug.Log("LevelManager: StopGame ������");
        StopAllCoroutines();
        currentWave = 0;
        waveInProgress = false;
        gameStarted = false;
    }

    public void NotifyEnemyReachedEnd()
    {
        enemiesReachedEnd++;
        Debug.Log($"LevelManager: ���� ������ ����� ����, ����� ��������: {enemiesReachedEnd} �� {enemiesInCurrentWave}");

        if (enemiesReachedEnd >= enemiesInCurrentWave && villageHealth != null)
        {
            ApplyWaveDamage();
        }
    }

    IEnumerator StartWaves()
    {
        Debug.Log($"LevelManager: StartWaves �����, ����� ����: {waves.Count}, currentWave: {currentWave}");
        while (currentWave < waves.Count && gameStarted)
        {
            if (!waveInProgress)
            {
                Debug.Log($"LevelManager: ������� ����� {currentWave + 1}");
                enemiesInCurrentWave = waves[currentWave].enemyCount; // ������������� ���������� ������ � �����
                enemiesReachedEnd = 0; // ���������� �������
                yield return StartCoroutine(SpawnWave(waves[currentWave]));
                currentWave++;
            }
            else
            {
                yield return null;
            }
            if (currentWave < waves.Count)
            {
                Debug.Log($"LevelManager: �������� {waveDelay} ������ ����� ��������� ������");
                yield return new WaitForSeconds(waveDelay);
            }
        }
        if (currentWave >= waves.Count && villageHealth != null && villageHealth.GetCurrentHealth() > 0)
        {
            GameManager.Instance.CompleteLevel(1); // ������� 1 �������
            Debug.Log("LevelManager: ������� 1 ��������, ����������� GameManager");
        }
    }

    IEnumerator SpawnWave(Wave wave)
    {
        Debug.Log($"LevelManager: SpawnWave �����, enemyCount={wave.enemyCount}");
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
            Debug.LogWarning("LevelManager: SpawnEnemy ��������, ���� �� ������");
            return;
        }

        if (enemyPrefab == null) Debug.LogError("LevelManager: enemyPrefab is null!");
        if (spawnPoint == null) Debug.LogError("LevelManager: spawnPoint is null!");
        if (path == null || path.Length == 0) Debug.LogError("LevelManager: path is null or empty!");

        if (enemyPrefab != null && spawnPoint != null && path != null && path.Length > 0)
        {
            Debug.Log($"LevelManager: ������� ����� �� ������� {spawnPoint.position}");
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
            if (enemyMovement != null)
            {
                enemyMovement.path = path;
                Debug.Log($"LevelManager: Enemy ������: {enemy.name}");
            }
            else
            {
                Debug.LogError("LevelManager: EnemyMovement ��������� ����������� �� �������!");
            }
        }
    }

    void ApplyWaveDamage()
    {
        if (villageHealth != null)
        {
            if (currentWave == 1) // ������ ����� � �������� �����
            {
                Debug.Log("������ ����� ������, ���� Village �� ������.");
            }
            else if (currentWave == 2) // ������ ����� � �������� �� ��������
            {
                int halfHealth = villageHealth.GetCurrentHealth() / 2;
                villageHealth.TakeDamage(halfHealth);
                Debug.Log("������ ����� ������, �������� Village ��������� �� ��������.");
            }
            else if (currentWave == 3) // ������ ����� � ������ ����������
            {
                villageHealth.TakeDamage(villageHealth.GetCurrentHealth());
                Debug.Log("������ ����� ������, Village ��������.");
            }
        }
    }
}

[System.Serializable]
public class Wave
{
    public int enemyCount; // ���������� ������ � �����
    public float spawnDelay = 1f; // �������� ����� �������� ������
}