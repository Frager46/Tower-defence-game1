using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyMovement : MonoBehaviour
{
    public Transform[] path;
    private int currentPoint = 0;
    private float speed = 2f;
    private bool isMoving = true;

    private void Start()
    {
        if (path == null || path.Length == 0)
        {
            Debug.LogError($"EnemyMovement: ���� �� ����� ��� ���� ��� {gameObject.name} � ����� {SceneManager.GetActiveScene().name}");
            isMoving = false;
            return;
        }
        Debug.Log($"EnemyMovement: ������ �������� ��� {gameObject.name}, ����� � ����: {path.Length}, �����: {SceneManager.GetActiveScene().name}");
        transform.position = path[0].position;
    }

    private void Update()
    {
        if (!isMoving)
            return;

        if (currentPoint < path.Length)
        {
            Vector2 target = path[currentPoint].position;
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, target) < 0.1f)
            {
                currentPoint++;
                if (currentPoint >= path.Length)
                {
                    Debug.Log($"EnemyMovement: {gameObject.name} ������ ����� ����, ���������� ��������� ������");
                    StartCoroutine(NotifyLevelManagerReachedEndWithRetry());
                    Destroy(gameObject);
                    return;
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (currentPoint < path.Length)
        {
            Debug.Log($"EnemyMovement: {gameObject.name} ��������� �� ���������� ����� ����, ���������� ��������� ������");
            StartCoroutine(NotifyLevelManagerDestroyedWithRetry());
        }
    }

    private System.Collections.IEnumerator NotifyLevelManagerReachedEndWithRetry()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"EnemyMovement: ����������� ��������� ������ � ����� {sceneName} ��� {gameObject.name}");
        int maxRetries = 3;
        float retryDelay = 0.1f;

        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                switch (sceneName)
                {
                    case "Level1Scene":
                        if (LevelManager.main != null)
                        {
                            LevelManager.main.NotifyEnemyReachedEnd(gameObject);
                            Debug.Log($"EnemyMovement: ����������� LevelManager ������� ��� {gameObject.name}");
                            yield break;
                        }
                        else
                        {
                            Debug.LogWarning($"EnemyMovement: LevelManager.main is null ��� {gameObject.name} � ����� {sceneName}, ������� {attempt + 1}/{maxRetries}");
                        }
                        break;
                    case "Level2Scene":
                        if (Level2Manager.main != null)
                        {
                            Level2Manager.main.NotifyEnemyReachedEnd(gameObject);
                            Debug.Log($"EnemyMovement: ����������� Level2Manager ������� ��� {gameObject.name}");
                            yield break;
                        }
                        else
                        {
                            Debug.LogWarning($"EnemyMovement: Level2Manager.main is null ��� {gameObject.name} � ����� {sceneName}, ������� {attempt + 1}/{maxRetries}");
                        }
                        break;
                    case "Level3Scene":
                        if (Level3Manager.main != null)
                        {
                            Level3Manager.main.NotifyEnemyReachedEnd(gameObject);
                            Debug.Log($"EnemyMovement: ����������� Level3Manager ������� ��� {gameObject.name}");
                            yield break;
                        }
                        else
                        {
                            Debug.LogWarning($"EnemyMovement: Level3Manager.main is null ��� {gameObject.name} � ����� {sceneName}, ������� {attempt + 1}/{maxRetries}");
                        }
                        break;
                    case "Level4Scene":
                        if (Level4Manager.main != null)
                        {
                            Level4Manager.main.NotifyEnemyReachedEnd(gameObject);
                            Debug.Log($"EnemyMovement: ����������� Level4Manager ������� ��� {gameObject.name}");
                            yield break;
                        }
                        else
                        {
                            Debug.LogWarning($"EnemyMovement: Level4Manager.main is null ��� {gameObject.name} � ����� {sceneName}, ������� {attempt + 1}/{maxRetries}");
                        }
                        break;
                    case "Level5Scene":
                        if (Level5Manager.main != null)
                        {
                            Level5Manager.main.NotifyEnemyReachedEnd(gameObject);
                            Debug.Log($"EnemyMovement: ����������� Level5Manager ������� ��� {gameObject.name}");
                            yield break;
                        }
                        else
                        {
                            Debug.LogWarning($"EnemyMovement: Level5Manager.main is null ��� {gameObject.name} � ����� {sceneName}, ������� {attempt + 1}/{maxRetries}");
                        }
                        break;
                    default:
                        Debug.LogError($"EnemyMovement: ����������� ����� {sceneName}, ����������� ��������� ������ �� ��������� ��� {gameObject.name}");
                        yield break;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"EnemyMovement: ������ ��� ����������� ��������� ������ ��� {gameObject.name}: {e.Message}");
                yield break;
            }

            if (attempt < maxRetries - 1)
            {
                yield return new WaitForSeconds(retryDelay);
            }
        }
        Debug.LogError($"EnemyMovement: �� ������� ��������� �������� ������ ��� {gameObject.name} � ����� {sceneName} ����� {maxRetries} �������. ���������� ������� ���������������� ��������� � �����!");
    }

    private System.Collections.IEnumerator NotifyLevelManagerDestroyedWithRetry()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"EnemyMovement: ����������� ��������� �� ����������� {gameObject.name} � ����� {sceneName}");
        int maxRetries = 3;
        float retryDelay = 0.1f;

        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                switch (sceneName)
                {
                    case "Level1Scene":
                        if (LevelManager.main != null)
                        {
                            LevelManager.main.NotifyEnemyDestroyed(gameObject);
                            Debug.Log($"EnemyMovement: ����������� LevelManager �� ����������� ������� ��� {gameObject.name}");
                            yield break;
                        }
                        else
                        {
                            Debug.LogWarning($"EnemyMovement: LevelManager.main is null ��� ����������� �� ����������� ��� {gameObject.name} � ����� {sceneName}, ������� {attempt + 1}/{maxRetries}");
                        }
                        break;
                    case "Level2Scene":
                        if (Level2Manager.main != null)
                        {
                            Level2Manager.main.NotifyEnemyDestroyed(gameObject);
                            Debug.Log($"EnemyMovement: ����������� Level2Manager �� ����������� ������� ��� {gameObject.name}");
                            yield break;
                        }
                        else
                        {
                            Debug.LogWarning($"EnemyMovement: Level2Manager.main is null ��� ����������� �� ����������� ��� {gameObject.name} � ����� {sceneName}, ������� {attempt + 1}/{maxRetries}");
                        }
                        break;
                    case "Level3Scene":
                        if (Level3Manager.main != null)
                        {
                            Level3Manager.main.NotifyEnemyDestroyed(gameObject);
                            Debug.Log($"EnemyMovement: ����������� Level3Manager �� ����������� ������� ��� {gameObject.name}");
                            yield break;
                        }
                        else
                        {
                            Debug.LogWarning($"EnemyMovement: Level3Manager.main is null ��� ����������� �� ����������� ��� {gameObject.name} � ����� {sceneName}, ������� {attempt + 1}/{maxRetries}");
                        }
                        break;
                    case "Level4Scene":
                        if (Level4Manager.main != null)
                        {
                            Level4Manager.main.NotifyEnemyDestroyed(gameObject);
                            Debug.Log($"EnemyMovement: ����������� Level4Manager �� ����������� ������� ��� {gameObject.name}");
                            yield break;
                        }
                        else
                        {
                            Debug.LogWarning($"EnemyMovement: Level4Manager.main is null ��� ����������� �� ����������� ��� {gameObject.name} � ����� {sceneName}, ������� {attempt + 1}/{maxRetries}");
                        }
                        break;
                    case "Level5Scene":
                        if (Level5Manager.main != null)
                        {
                            Level5Manager.main.NotifyEnemyDestroyed(gameObject);
                            Debug.Log($"EnemyMovement: ����������� Level5Manager �� ����������� ������� ��� {gameObject.name}");
                            yield break;
                        }
                        else
                        {
                            Debug.LogWarning($"EnemyMovement: Level5Manager.main is null ��� ����������� �� ����������� ��� {gameObject.name} � ����� {sceneName}, ������� {attempt + 1}/{maxRetries}");
                        }
                        break;
                    default:
                        Debug.LogError($"EnemyMovement: ����������� ����� {sceneName}, ����������� �� ����������� �� ��������� ��� {gameObject.name}");
                        yield break;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"EnemyMovement: ������ ��� ����������� �� ����������� ��� {gameObject.name}: {e.Message}");
                yield break;
            }

            if (attempt < maxRetries - 1)
            {
                yield return new WaitForSeconds(retryDelay);
            }
        }
        Debug.LogError($"EnemyMovement: �� ������� ��������� �������� �� ����������� ��� {gameObject.name} � ����� {sceneName} ����� {maxRetries} �������. ���������� ������� ���������������� ��������� � �����!");
    }
}