using UnityEngine;
using UnityEngine.UI;

public class PlacementManager : MonoBehaviour
{
    [SerializeField] private Button placeUnit1Button;
    [SerializeField] private Button placeUnit2Button;
    [SerializeField] private Button placeUnit3Button;
    [SerializeField] private GameObject placeUnit1Prefab;
    [SerializeField] private GameObject placeUnit2Prefab;
    [SerializeField] private GameObject placeUnit3Prefab;
    private const int PLACE_UNIT1_COST = 25;
    private const int PLACE_UNIT2_COST = 45;
    private const int PLACE_UNIT3_COST = 75;

    private GameObject previewObject;
    private bool isPlacing = false;
    private GameObject currentPrefab;
    private GameState gameState;
    private LevelsManager levelsManager;

    private void Start()
    {
        gameState = GameState.Instance;
        levelsManager = LevelsManager.Instance;
        if (gameState == null)
        {
            Debug.LogError("PlacementManager: GameState не найден!");
            return;
        }
        if (levelsManager == null)
        {
            Debug.LogError("PlacementManager: LevelsManager не найден!");
            return;
        }

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            Button[] buttons = canvas.GetComponentsInChildren<Button>(true);
            foreach (Button btn in buttons)
            {
                if (btn.name.Contains("PlaceUnit1")) placeUnit1Button = btn;
                if (btn.name.Contains("PlaceUnit2")) placeUnit2Button = btn;
                if (btn.name.Contains("PlaceUnit3")) placeUnit3Button = btn;
            }
        }

        if (placeUnit1Button != null)
        {
            placeUnit1Button.onClick.RemoveAllListeners();
            placeUnit1Button.onClick.AddListener(() => StartPlacement(placeUnit1Prefab, PLACE_UNIT1_COST));
            Debug.Log("PlacementManager: Назначен слушатель для placeUnit1Button");
        }
        else
        {
            Debug.LogError("PlacementManager: placeUnit1Button не найден!");
        }

        if (placeUnit2Button != null)
        {
            placeUnit2Button.onClick.RemoveAllListeners();
            placeUnit2Button.onClick.AddListener(() => StartPlacement(placeUnit2Prefab, PLACE_UNIT2_COST));
            Debug.Log("PlacementManager: Назначен слушатель для placeUnit2Button");
        }
        else
        {
            Debug.LogError("PlacementManager: placeUnit2Button не найден!");
        }

        if (placeUnit3Button != null)
        {
            placeUnit3Button.onClick.RemoveAllListeners();
            placeUnit3Button.onClick.AddListener(() => StartPlacement(placeUnit3Prefab, PLACE_UNIT3_COST));
            Debug.Log("PlacementManager: Назначен слушатель для placeUnit3Button");
        }
        else
        {
            Debug.LogError("PlacementManager: placeUnit3Button не найден!");
        }

        UpdateButtonStates();
    }

    private void Update()
    {
        UpdateButtonStates();

        if (isPlacing && previewObject != null && levelsManager.IsGameActive())
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            previewObject.transform.position = mousePos;
            Debug.Log($"PlacementManager: Позиция мыши: {mousePos}, Позиция previewObject: {previewObject.transform.position}, Родитель previewObject: {(previewObject.transform.parent != null ? previewObject.transform.parent.name : "None")}");

            if (Input.GetMouseButtonDown(0))
            {
                PlaceObject(mousePos);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                CancelPlacement();
            }
        }
    }

    private void UpdateButtonStates()
    {
        int currentGold = gameState.GetGold();
        if (placeUnit1Button != null)
            placeUnit1Button.interactable = currentGold >= PLACE_UNIT1_COST && levelsManager.IsGameActive();
        if (placeUnit2Button != null)
            placeUnit2Button.interactable = currentGold >= PLACE_UNIT2_COST && levelsManager.IsGameActive();
        if (placeUnit3Button != null)
            placeUnit3Button.interactable = currentGold >= PLACE_UNIT3_COST && levelsManager.IsGameActive();
    }

    private void StartPlacement(GameObject prefab, int cost)
    {
        if (gameState == null || levelsManager == null || !levelsManager.IsGameActive()) return;

        if (gameState.SpendGold(cost))
        {
            currentPrefab = prefab;
            // Создаём previewObject с отключённым PlaceForUnit
            previewObject = Instantiate(prefab, Vector2.zero, Quaternion.identity);
            previewObject.transform.SetParent(null);
            PlaceForUnit placeForUnit = previewObject.GetComponent<PlaceForUnit>();
            if (placeForUnit != null)
            {
                previewObject.name = "Preview_" + prefab.name;
                placeForUnit.enabled = false;
                Debug.Log($"PlacementManager: Создан previewObject {previewObject.name}, PlaceForUnit отключён");
            }
            SpriteRenderer renderer = previewObject.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                Color color = renderer.color;
                color.a = 0.5f;
                renderer.color = color;
            }
            isPlacing = true;
            Debug.Log($"PlacementManager: Начато размещение PlaceForUnit за {cost} золота");
        }
    }

    private void PlaceObject(Vector3 position)
    {
        GameObject placedObject = Instantiate(currentPrefab, position, Quaternion.identity);
        placedObject.transform.SetParent(null);
        placedObject.name = "Placed_" + currentPrefab.name;
        Debug.Log($"PlacementManager: PlaceForUnit размещён в позиции {position}, Родитель: {(placedObject.transform.parent != null ? placedObject.transform.parent.name : "None")}");

        if (previewObject != null)
        {
            Destroy(previewObject);
        }

        isPlacing = false;
        previewObject = null;
    }

    private void CancelPlacement()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
        }
        isPlacing = false;
        Debug.Log("PlacementManager: Размещение PlaceForUnit отменено");
    }
}