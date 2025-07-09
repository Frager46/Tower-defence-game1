using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;
using UnityEngine.EventSystems;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    [Header("UI Buttons")]
    public Button level1Button;
    public Button level2Button;
    public Button level3Button; // Добавлена кнопка для 3-го уровня
    public Button level4Button; // Добавлена кнопка для 4-го уровня
    public Button level5Button; // Добавлена кнопка для 5-го уровня
    public Button shopButton;
    public Button mainMenuButton; // Can be manually assigned in Inspector as fallback
    public Button placeForUnit1Button;
    public Button placeForUnit2Button;
    public Button placeForUnit3Button;
    public GameObject shopPanel;
    public Sprite lockSprite;
    public GameObject[] sceneObjectsToHide;
    public GameObject gameplayUI;
    public GameObject[] gameplayObjects;

    private bool isShopOpen = false;

    private void Awake()
    {
        Debug.Log("MapManager: Awake called, Instance: " + (Instance != null));
        Debug.Log($"MapManager: Initial references - level1Button: {level1Button != null}, level2Button: {level2Button != null}, level3Button: {level3Button != null}, level4Button: {level4Button != null}, level5Button: {level5Button != null}, shopButton: {shopButton != null}, mainMenuButton: {mainMenuButton != null}, placeForUnit1Button: {placeForUnit1Button != null}, placeForUnit2Button: {placeForUnit2Button != null}, placeForUnit3Button: {placeForUnit3Button != null}, shopPanel: {shopPanel != null}, sceneObjectsToHide: {sceneObjectsToHide != null && sceneObjectsToHide.Length > 0}, gameplayUI: {gameplayUI != null}, gameplayObjects: {gameplayObjects != null && gameplayObjects.Length > 0}");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("MapManager: Instance set and DontDestroyOnLoad applied");
        }
        else if (Instance != this)
        {
            Debug.Log("MapManager: Duplicate instance found, destroying self");
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("MapManager: SceneManager.sceneLoaded event subscribed");
    }

    private void Start()
    {
        Debug.Log("MapManager: Start called in scene: " + SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Debug.Log("MapManager: OnDestroy called, unsubscribed from SceneManager.sceneLoaded");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"MapManager: OnSceneLoaded called for scene: {scene.name}, Canvas count: {FindObjectsOfType<Canvas>().Length}, Instance: {Instance != null}");
        if (scene.name == "MapScene")
        {
            isShopOpen = false;
            Debug.Log($"MapManager: isShopOpen set to {isShopOpen}");
            if (shopPanel != null)
            {
                shopPanel.SetActive(false);
                Debug.Log($"MapManager: shopPanel '{shopPanel.name}' set to inactive");
            }
            else
            {
                Debug.LogWarning("MapManager: shopPanel is null before reset");
            }
            foreach (GameObject obj in sceneObjectsToHide ?? new GameObject[0])
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                    Debug.Log($"MapManager: Reset hidden object '{obj.name}' to true on load");
                }
                else
                {
                    Debug.LogWarning("MapManager: Found null reference in sceneObjectsToHide during reset");
                }
            }
            SetupUIForMapScene();
            Debug.Log("MapManager: SetupUIForMapScene called");
            EnsureEventSystem();
            Debug.Log("MapManager: EnsureEventSystem completed");
        }
        else
        {
            if (shopPanel != null)
            {
                shopPanel.SetActive(false);
                Debug.Log($"MapManager: shopPanel '{shopPanel.name}' deactivated in other scene");
            }
            else
            {
                Debug.LogWarning("MapManager: shopPanel is null in other scene");
            }
            if (gameplayUI != null)
            {
                gameplayUI.SetActive(false);
                Debug.Log($"MapManager: gameplayUI '{gameplayUI.name}' deactivated in other scene");
            }
            else
            {
                Debug.LogWarning("MapManager: gameplayUI is null in other scene");
            }
            foreach (GameObject obj in gameplayObjects ?? new GameObject[0])
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                    Debug.Log($"MapManager: gameplayObject '{obj.name}' deactivated in other scene");
                }
                else
                {
                    Debug.LogWarning("MapManager: Found null reference in gameplayObjects in other scene");
                }
            }
        }
    }

    private void SetupUIForMapScene()
    {
        Debug.Log("MapManager: SetupUIForMapScene called");
        // Reset all UI references
        level1Button = null;
        level2Button = null;
        level3Button = null; // Сброс для новой кнопки
        level4Button = null; // Сброс для новой кнопки
        level5Button = null; // Сброс для новой кнопки
        shopButton = null;
        mainMenuButton = null;
        placeForUnit1Button = null;
        placeForUnit2Button = null;
        placeForUnit3Button = null;
        shopPanel = null;
        gameplayUI = null;
        Debug.Log("MapManager: All UI references reset to null");

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            Debug.Log($"MapManager: Canvas found, name: {canvas.name}");
            Button[] buttons = canvas.GetComponentsInChildren<Button>(true);
            Debug.Log($"MapManager: Found {buttons.Length} buttons under Canvas");
            foreach (Button btn in buttons)
            {
                Debug.Log($"MapManager: Checking button '{btn.name}', active: {btn.gameObject.activeSelf}, has Button: {btn != null}");
                if (btn.name.Contains("Shop") && shopButton == null)
                {
                    shopButton = btn;
                    Debug.Log($"MapManager: shopButton assigned to '{btn.name}'");
                }
                if (btn.name.Contains("MainMenu") && mainMenuButton == null)
                {
                    mainMenuButton = btn;
                    Debug.Log($"MapManager: mainMenuButton assigned to '{btn.name}'");
                }
                else if (btn.name == "start" && mainMenuButton == null && btn.GetComponent<Button>() != null)
                {
                    mainMenuButton = btn;
                    Debug.Log($"MapManager: mainMenuButton assigned to 'start' as '{btn.name}'");
                }
                else if (btn.name.Contains("BackToMenu") && mainMenuButton == null)
                {
                    mainMenuButton = btn;
                    Debug.Log($"MapManager: mainMenuButton assigned to 'BackToMenu' as '{btn.name}'");
                }
                else if (btn.name.Contains("Menu") && mainMenuButton == null)
                {
                    mainMenuButton = btn;
                    Debug.Log($"MapManager: mainMenuButton assigned to 'Menu' as '{btn.name}'");
                }
                if (btn.name.Contains("Unit1") && placeForUnit1Button == null)
                {
                    placeForUnit1Button = btn;
                    Debug.Log($"MapManager: placeForUnit1Button assigned to '{btn.name}'");
                }
                if (btn.name.Contains("Unit2") && placeForUnit2Button == null)
                {
                    placeForUnit2Button = btn;
                    Debug.Log($"MapManager: placeForUnit2Button assigned to '{btn.name}'");
                }
                if (btn.name.Contains("Unit3") && placeForUnit3Button == null)
                {
                    placeForUnit3Button = btn;
                    Debug.Log($"MapManager: placeForUnit3Button assigned to '{btn.name}'");
                }
                if (btn.name.Contains("Level2") && level2Button == null)
                {
                    level2Button = btn;
                    Debug.Log($"MapManager: level2Button assigned to '{btn.name}'");
                }
                if (btn.name.Contains("Level1") && level1Button == null)
                {
                    level1Button = btn;
                    Debug.Log($"MapManager: level1Button assigned to '{btn.name}'");
                }
                if (btn.name.Contains("Level4") && level4Button == null)
                {
                    level4Button = btn;
                    Debug.Log($"MapManager: level4Button assigned to '{btn.name}'");
                }
                if (btn.name.Contains("Level3") && level3Button == null)
                {
                    level3Button = btn;
                    Debug.Log($"MapManager: level3Button assigned to '{btn.name}'");
                }
                if (btn.name.Contains("Level5") && level5Button == null)
                {
                    level5Button = btn;
                    Debug.Log($"MapManager: level5Button assigned to '{btn.name}'");
                }
            }

            // Enhanced search for shopPanel including inactive objects
            if (shopPanel == null)
            {
                shopPanel = GameObject.Find("ShopPanel");
                if (shopPanel == null)
                {
                    shopPanel = GameObject.FindObjectsOfType<GameObject>(true)
                        .FirstOrDefault(go => go.name == "ShopPanel");
                }
                if (shopPanel != null)
                {
                    Debug.Log($"MapManager: shopPanel found dynamically as '{shopPanel.name}'");
                }
                else
                {
                    Debug.LogWarning("MapManager: shopPanel not found even with inactive search!");
                }
            }

            if (gameplayUI == null)
            {
                gameplayUI = GameObject.Find("GameplayUI");
                if (gameplayUI != null)
                {
                    Debug.Log($"MapManager: gameplayUI found dynamically as '{gameplayUI.name}'");
                }
                else
                {
                    Debug.LogWarning("MapManager: gameplayUI not found! Creating placeholder.");
                    gameplayUI = new GameObject("GameplayUI");
                    gameplayUI.transform.SetParent(canvas.transform);
                    Debug.Log($"MapManager: Created new gameplayUI placeholder '{gameplayUI.name}'");
                }
            }

            if (GameState.Instance != null)
            {
                TextMeshProUGUI[] tmpTexts = canvas.GetComponentsInChildren<TextMeshProUGUI>(true);
                Debug.Log($"MapManager: Found {tmpTexts.Length} TextMeshProUGUI components");
                foreach (TextMeshProUGUI tmpTxt in tmpTexts)
                {
                    if (tmpTxt.name == "GoldText")
                    {
                        GameState.Instance.SetGoldText(tmpTxt);
                        Debug.Log($"MapManager: GoldText assigned to '{tmpTxt.name}'");
                        break;
                    }
                }
            }
        }
        else
        {
            Debug.LogError("MapManager: Canvas not found in MapScene!");
        }

        // Configure buttons
        if (mainMenuButton != null)
        {
            mainMenuButton.gameObject.SetActive(true);
            Debug.Log($"MapManager: mainMenuButton '{mainMenuButton.name}' set active: {mainMenuButton.gameObject.activeSelf}");
            mainMenuButton.interactable = true;
            Debug.Log($"MapManager: mainMenuButton '{mainMenuButton.name}' set interactable: {mainMenuButton.interactable}");
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(ShowMainMenu);
            Debug.Log($"MapManager: mainMenuButton '{mainMenuButton.name}' listener set");
        }
        else
        {
            Debug.LogError("MapManager: mainMenuButton not found after dynamic search!");
            if (mainMenuButton == null && this.mainMenuButton != null)
            {
                mainMenuButton = this.mainMenuButton;
                mainMenuButton.gameObject.SetActive(true);
                Debug.Log($"MapManager: mainMenuButton set active from Inspector to '{mainMenuButton.name}': {mainMenuButton.gameObject.activeSelf}");
                mainMenuButton.interactable = true;
                Debug.Log($"MapManager: mainMenuButton set interactable from Inspector to '{mainMenuButton.name}': {mainMenuButton.interactable}");
                mainMenuButton.onClick.RemoveAllListeners();
                mainMenuButton.onClick.AddListener(ShowMainMenu);
                Debug.Log($"MapManager: mainMenuButton listener set from Inspector for '{mainMenuButton.name}'");
            }
        }

        if (shopButton != null)
        {
            shopButton.gameObject.SetActive(true);
            Debug.Log($"MapManager: shopButton '{shopButton.name}' set active: {shopButton.gameObject.activeSelf}");
            shopButton.interactable = true;
            Debug.Log($"MapManager: shopButton '{shopButton.name}' set interactable: {shopButton.interactable}");
            shopButton.onClick.RemoveAllListeners();
            shopButton.onClick.AddListener(OpenShop);
            Debug.Log($"MapManager: shopButton '{shopButton.name}' listener set");
        }
        else
        {
            Debug.LogError("MapManager: shopButton not found after dynamic search!");
        }

        if (level1Button != null)
        {
            level1Button.gameObject.SetActive(true);
            Debug.Log($"MapManager: level1Button '{level1Button.name}' set active: {level1Button.gameObject.activeSelf}");
            level1Button.interactable = true;
            Debug.Log($"MapManager: level1Button '{level1Button.name}' set interactable: {level1Button.interactable}");
            level1Button.onClick.RemoveAllListeners();
            level1Button.onClick.AddListener(LoadLevel1);
            Debug.Log($"MapManager: level1Button '{level1Button.name}' listener set");
        }
        else
        {
            Debug.LogError("MapManager: level1Button not found after dynamic search!");
        }

        if (level2Button != null)
        {
            level2Button.gameObject.SetActive(true);
            Debug.Log($"MapManager: level2Button '{level2Button.name}' set active: {level2Button.gameObject.activeSelf}");
            level2Button.interactable = true; // Убрано условие блокировки
            Debug.Log($"MapManager: level2Button '{level2Button.name}' set interactable: {level2Button.interactable}");
            level2Button.onClick.RemoveAllListeners();
            level2Button.onClick.AddListener(LoadLevel2); // Предполагаем, что level2Button для 2-го уровня
            Debug.Log($"MapManager: level2Button '{level2Button.name}' listener set");
        }
        else
        {
            Debug.LogError("MapManager: level2Button not found after dynamic search!");
        }

        if (level4Button != null)
        {
            level4Button.gameObject.SetActive(true);
            Debug.Log($"MapManager: level4Button '{level4Button.name}' set active: {level4Button.gameObject.activeSelf}");
            level4Button.interactable = true; // Убрано условие IsLevel3Completed()
            Debug.Log($"MapManager: level4Button '{level4Button.name}' set interactable: {level4Button.interactable}");
            level4Button.onClick.RemoveAllListeners();
            level4Button.onClick.AddListener(LoadLevel4);
            Debug.Log($"MapManager: level4Button '{level4Button.name}' listener set");
        }
        else
        {
            Debug.LogError("MapManager: level4Button not found after dynamic search!");
        }

        if (level3Button != null)
        {
            level3Button.gameObject.SetActive(true);
            Debug.Log($"MapManager: level3Button '{level3Button.name}' set active: {level3Button.gameObject.activeSelf}");
            level3Button.interactable = true; // Убрано условие блокировки
            Debug.Log($"MapManager: level3Button '{level3Button.name}' set interactable: {level3Button.interactable}");
            level3Button.onClick.RemoveAllListeners();
            level3Button.onClick.AddListener(LoadLevel3);
            Debug.Log($"MapManager: level3Button '{level3Button.name}' listener set");
        }
        else
        {
            Debug.LogError("MapManager: level3Button not found after dynamic search!");
        }

        if (level5Button != null)
        {
            level5Button.gameObject.SetActive(true);
            Debug.Log($"MapManager: level5Button '{level5Button.name}' set active: {level5Button.gameObject.activeSelf}");
            level5Button.interactable = true; // Убрано условие блокировки
            Debug.Log($"MapManager: level5Button '{level5Button.name}' set interactable: {level5Button.interactable}");
            level5Button.onClick.RemoveAllListeners();
            level5Button.onClick.AddListener(LoadLevel5);
            Debug.Log($"MapManager: level5Button '{level5Button.name}' listener set");
        }
        else
        {
            Debug.LogError("MapManager: level5Button not found after dynamic search!");
        }

        if (placeForUnit1Button != null)
        {
            placeForUnit1Button.gameObject.SetActive(true);
            Debug.Log($"MapManager: placeForUnit1Button '{placeForUnit1Button.name}' set active: {placeForUnit1Button.gameObject.activeSelf}");
            placeForUnit1Button.interactable = true;
            Debug.Log($"MapManager: placeForUnit1Button '{placeForUnit1Button.name}' set interactable: {placeForUnit1Button.interactable}");
            placeForUnit1Button.onClick.RemoveAllListeners();
            placeForUnit1Button.onClick.AddListener(() => BuyPlaceForUnit(1));
            Debug.Log($"MapManager: placeForUnit1Button '{placeForUnit1Button.name}' listener set");
        }
        else
        {
            Debug.LogError("MapManager: placeForUnit1Button not found after dynamic search!");
        }

        if (placeForUnit2Button != null)
        {
            placeForUnit2Button.gameObject.SetActive(true);
            Debug.Log($"MapManager: placeForUnit2Button '{placeForUnit2Button.name}' set active: {placeForUnit2Button.gameObject.activeSelf}");
            placeForUnit2Button.interactable = true; // Убрано условие IsLevel1Completed()
            Debug.Log($"MapManager: placeForUnit2Button '{placeForUnit2Button.name}' set interactable: {placeForUnit2Button.interactable}");
            placeForUnit2Button.onClick.RemoveAllListeners();
            placeForUnit2Button.onClick.AddListener(() => BuyPlaceForUnit(2));
            Debug.Log($"MapManager: placeForUnit2Button '{placeForUnit2Button.name}' listener set");
        }
        else
        {
            Debug.LogError("MapManager: placeForUnit2Button not found after dynamic search!");
        }

        if (placeForUnit3Button != null)
        {
            placeForUnit3Button.gameObject.SetActive(true);
            Debug.Log($"MapManager: placeForUnit3Button '{placeForUnit3Button.name}' set active: {placeForUnit3Button.gameObject.activeSelf}");
            placeForUnit3Button.interactable = true; // Убрано условие IsLevel2Completed()
            Debug.Log($"MapManager: placeForUnit3Button '{placeForUnit3Button.name}' set interactable: {placeForUnit3Button.interactable}");
            placeForUnit3Button.onClick.RemoveAllListeners();
            placeForUnit3Button.onClick.AddListener(() => BuyPlaceForUnit(3));
            Debug.Log($"MapManager: placeForUnit3Button '{placeForUnit3Button.name}' listener set");
        }
        else
        {
            Debug.LogError("MapManager: placeForUnit3Button not found after dynamic search!");
        }

        if (gameplayUI != null)
        {
            gameplayUI.SetActive(true);
            Debug.Log($"MapManager: gameplayUI '{gameplayUI.name}' activated");
        }

        foreach (GameObject obj in gameplayObjects ?? new GameObject[0])
        {
            if (obj != null)
            {
                obj.SetActive(true);
                Debug.Log($"MapManager: gameplayObject '{obj.name}' activated");
            }
            else
            {
                Debug.LogWarning("MapManager: Found null reference in gameplayObjects during activation");
            }
        }

        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
            Debug.Log($"MapManager: shopPanel '{shopPanel.name}' deactivated");
        }
        Debug.Log("MapManager: SetupUIForMapScene completed");
    }

    private void EnsureEventSystem()
    {
        Debug.Log("MapManager: EnsureEventSystem called");
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();
            Debug.Log("MapManager: Created new EventSystem");
        }
        else
        {
            Debug.Log($"MapManager: EventSystem already exists, name: {eventSystem.name}");
        }
        Debug.Log("MapManager: EnsureEventSystem completed");
    }

    public void LoadLevel1()
    {
        Debug.Log("MapManager: LoadLevel1 called");
        Debug.Log("MapManager: Loading Level1Scene");
        SceneManager.LoadScene("Level1Scene");
    }

    public void LoadLevel2()
    {
        Debug.Log("MapManager: LoadLevel2 called");
        Debug.Log("MapManager: Loading Level2Scene");
        SceneManager.LoadScene("Level2Scene");
    }

    public void LoadLevel4()
    {
        Debug.Log("MapManager: LoadLevel4 called");
        Debug.Log("MapManager: Loading Level4Scene");
        SceneManager.LoadScene("Level4Scene");
    }

    public void LoadLevel3()
    {
        Debug.Log("MapManager: LoadLevel3 called");
        Debug.Log("MapManager: Loading Level3Scene");
        SceneManager.LoadScene("Level3Scene");
    }

    public void LoadLevel5()
    {
        Debug.Log("MapManager: LoadLevel5 called");
        Debug.Log("MapManager: Loading Level5Scene");
        SceneManager.LoadScene("Level5Scene");
    }

    public void ShowMainMenu()
    {
        Debug.Log("MapManager: ShowMainMenu called");
        if (GameState.Instance != null)
        {
            GameState.Instance.ResetGold();
            Debug.Log("MapManager: Gold reset");
        }
        Debug.Log("MapManager: Loading MainMenuScene");
        SceneManager.LoadScene("MainMenuScene");
    }

    public void OpenShop()
    {
        Debug.Log($"MapManager: OpenShop called, isShopOpen: {isShopOpen}, shopPanel: {(shopPanel != null)}, sceneObjectsToHide array length: {(sceneObjectsToHide != null ? sceneObjectsToHide.Length : 0)}");
        if (shopPanel != null)
        {
            if (isShopOpen)
            {
                CloseShop();
            }
            else
            {
                // Проверка и динамическое заполнение sceneObjectsToHide, если оно пустое или содержит null
                if (sceneObjectsToHide == null || sceneObjectsToHide.Length == 0 || sceneObjectsToHide.Any(obj => obj == null))
                {
                    Debug.Log("MapManager: sceneObjectsToHide is null, empty, or contains null, searching dynamically");
                    sceneObjectsToHide = GameObject.FindGameObjectsWithTag("HideOnShop")
                        .Where(obj => obj != null && obj != shopPanel)
                        .ToArray();
                    Debug.Log($"MapManager: Dynamically found {(sceneObjectsToHide != null ? sceneObjectsToHide.Length : 0)} objects with tag 'HideOnShop'");
                }
                else
                {
                    Debug.Log($"MapManager: sceneObjectsToHide array valid, length: {sceneObjectsToHide.Length}");
                }

                foreach (GameObject obj in sceneObjectsToHide ?? new GameObject[0])
                {
                    if (obj != null)
                    {
                        obj.SetActive(false);
                        Debug.Log($"MapManager: Hidden object '{obj.name}'");
                    }
                    else
                    {
                        Debug.LogWarning("MapManager: Found null reference in sceneObjectsToHide during hide");
                    }
                }
                shopPanel.SetActive(true);
                isShopOpen = true;
                Debug.Log($"MapManager: Shop opened, shopPanel '{shopPanel.name}' activated");
            }
        }
        else
        {
            Debug.LogError("MapManager: shopPanel is null, cannot open shop!");
        }
        Debug.Log("MapManager: OpenShop completed");
    }

    public void CloseShop()
    {
        Debug.Log("MapManager: CloseShop called");
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
            Debug.Log($"MapManager: shopPanel '{shopPanel.name}' deactivated");
            foreach (GameObject obj in sceneObjectsToHide ?? new GameObject[0])
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                    Debug.Log($"MapManager: Restored object '{obj.name}'");
                }
                else
                {
                    Debug.LogWarning("MapManager: Found null reference in sceneObjectsToHide during restore");
                }
            }
            isShopOpen = false;
            Debug.Log($"MapManager: isShopOpen set to {isShopOpen}");
        }
        else
        {
            Debug.LogError("MapManager: shopPanel is null, cannot close shop!");
        }
        Debug.Log("MapManager: CloseShop completed");
    }

    public void BuyPlaceForUnit(int type)
    {
        Debug.Log($"MapManager: BuyPlaceForUnit called for type {type}");
        int cost = type * 50; // Cost: 50, 100, 150
        if (GameState.Instance != null && GameState.Instance.SpendGold(cost))
        {
            Debug.Log($"MapManager: PlaceForUnit {type} unlocked, cost: {cost}");
            if (type == 2 && GameState.Instance != null && !GameState.Instance.IsLevel1Completed())
            {
                GameState.Instance.CompleteLevel(1);
                Debug.Log("MapManager: Level 1 completed");
            }
            if (type == 3 && GameState.Instance != null && !GameState.Instance.IsLevel2Completed())
            {
                GameState.Instance.CompleteLevel(2);
                Debug.Log("MapManager: Level 2 completed");
            }
            SetupUIForMapScene();
            Debug.Log("MapManager: SetupUIForMapScene called after purchase");
            CloseShop();
            Debug.Log("MapManager: CloseShop called after purchase");
        }
        else
        {
            Debug.LogWarning($"MapManager: Insufficient gold for PlaceForUnit {type}, required: {cost}");
        }
        Debug.Log("MapManager: BuyPlaceForUnit completed");
    }
}