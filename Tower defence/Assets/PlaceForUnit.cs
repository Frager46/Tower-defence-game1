using UnityEngine;

public class PlaceForUnit : MonoBehaviour
{
    public GameObject unitPrefab;
    public float yOffset = 0.5f;
    private GameObject placedUnit;
    private GameManager gameManager;
    private bool isPlaced = false;
    private bool hasAttemptedSpawn = false;

    void Start()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            Debug.LogWarning($"PlaceForUnit {gameObject.name} находится в UI (Canvas: {canvas.name})! Уничтожаем, чтобы избежать спавна юнита.");
            Destroy(gameObject);
            return;
        }

        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError($"PlaceForUnit {gameObject.name}: GameManager не найден в сцене! Юнит не будет создан.");
            return;
        }

        if (unitPrefab == null)
        {
            Debug.LogError($"PlaceForUnit {gameObject.name}: unitPrefab не назначен! Юнит не будет создан.");
            return;
        }

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            Color color = renderer.color;
            color.a = 1f;
            renderer.color = color;
        }
        else
        {
            Debug.LogWarning($"PlaceForUnit {gameObject.name}: SpriteRenderer не найден!");
        }

        isPlaced = true;
        Debug.Log($"PlaceForUnit {gameObject.name}: Инициализирован в позиции {transform.position}, Родитель: {(transform.parent != null ? transform.parent.name : "None")}");
    }

    void Update()
    {
        if (!isPlaced) return;

        if (!gameManager.IsGameActive())
        {
            if (placedUnit != null)
            {
                Destroy(placedUnit);
                placedUnit = null;
                hasAttemptedSpawn = false;
                Debug.Log($"Юнит уничтожен для {gameObject.name}, так как игра неактивна");
            }
        }
        else
        {
            if (placedUnit == null && !hasAttemptedSpawn)
            {
                Debug.Log($"PlaceForUnit {gameObject.name}: Пытаемся создать юнит, так как игра активна");
                SpawnUnit();
                hasAttemptedSpawn = true;
            }
        }
    }

    private void SpawnUnit()
    {
        Vector3 spawnPosition = transform.position + new Vector3(0, yOffset, 0);
        placedUnit = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
        placedUnit.tag = "Unit";
        Debug.Log($"Юнит создан в позиции {spawnPosition} для {gameObject.name} (позиция PlaceForUnit: {transform.position})");
    }
}