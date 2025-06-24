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
            Debug.LogWarning($"PlaceForUnit {gameObject.name} ��������� � UI (Canvas: {canvas.name})! ����������, ����� �������� ������ �����.");
            Destroy(gameObject);
            return;
        }

        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError($"PlaceForUnit {gameObject.name}: GameManager �� ������ � �����! ���� �� ����� ������.");
            return;
        }

        if (unitPrefab == null)
        {
            Debug.LogError($"PlaceForUnit {gameObject.name}: unitPrefab �� ��������! ���� �� ����� ������.");
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
            Debug.LogWarning($"PlaceForUnit {gameObject.name}: SpriteRenderer �� ������!");
        }

        isPlaced = true;
        Debug.Log($"PlaceForUnit {gameObject.name}: ��������������� � ������� {transform.position}, ��������: {(transform.parent != null ? transform.parent.name : "None")}");
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
                Debug.Log($"���� ��������� ��� {gameObject.name}, ��� ��� ���� ���������");
            }
        }
        else
        {
            if (placedUnit == null && !hasAttemptedSpawn)
            {
                Debug.Log($"PlaceForUnit {gameObject.name}: �������� ������� ����, ��� ��� ���� �������");
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
        Debug.Log($"���� ������ � ������� {spawnPosition} ��� {gameObject.name} (������� PlaceForUnit: {transform.position})");
    }
}