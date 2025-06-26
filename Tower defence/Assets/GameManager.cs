using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject mainMenuPanel;
    public GameObject gameplayUI;
    public GameObject[] gameplayObjects;

    [Header("UI Buttons")]
    public Button playButton;
    public Button pauseButton;
    public Button speedButton;
    public Text speedText;
    public Button backToMenuButton;
    public Button mainMenuButton; // ������ ��� �������� � MainMenuScene

    [Header("Currency")]
    public int initialGold = 75;
    private int gold;
    private TextMeshProUGUI goldText; // ��� ��� ��������� ��� GoldText

    [Header("Upgrades")]
    private float damageMultiplier = 1f;

    [Header("Shop Settings")]
    public GameObject shopPanel;
    public Button shopButton; // ������ �������� �������� � MapScene
    public Button placeForUnit1Button;
    public Button placeForUnit2Button;
    public Button placeForUnit3Button;
    public Sprite lockSprite; // ������ ����� ��� ��������������� ����
    public GameObject[] sceneObjectsToHide; // ������� �����, ������� ����� ������ ��� �������� ��������

    private bool isPaused = false;
    private int currentSpeed = 1;
    private bool isGameActive = false;
    private bool isShopOpen = false; // ���� ��� ������������ ��������� ��������

    private GameObject[] enemies;
    private bool level1Completed = false;
    private bool level2Completed = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager Awake, instance count: " + FindObjectsOfType<GameManager>().Length);
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        Debug.Log("GameManager: Start ������ � �����: " + SceneManager.GetActiveScene().name);
        gold = initialGold;
        UpdateGoldText();
        InitializeUI();
        EnsureEventSystem();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"GameManager: ����� {scene.name} ���������");
        if (scene.name == "Level1Scene")
        {
            StartCoroutine(DelayedStartLevelGameplay());
        }
        else if (scene.name == "MapScene")
        {
            SetupUIForMapScene();
        }
        else if (scene.name == "MainMenuScene")
        {
            InitializeUI();
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void InitializeUI()
    {
        Debug.Log("GameManager: InitializeUI ������");

        if (gameplayUI != null)
        {
            gameplayUI.SetActive(false);
            Debug.Log("GameManager: gameplayUI �������������");
        }

        foreach (GameObject obj in gameplayObjects)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        // ������ ���� playButton ������, ����� ������������� ������������
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            Button[] buttons = canvas.GetComponentsInChildren<Button>(true);
            foreach (Button btn in buttons)
            {
                if (btn.name.Contains("Play"))
                {
                    playButton = btn;
                    Debug.Log($"GameManager: playButton ������ �����������, ���: {btn.name}");
                    break;
                }
            }
        }

        if (playButton != null)
        {
            playButton.gameObject.SetActive(true);
            playButton.interactable = true;
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(StartGame);
            Debug.Log($"GameManager: �������� ��������� ��� playButton, ������������: {playButton.interactable}");
        }
        else
        {
            Debug.LogError("GameManager: playButton �� �������� � �� ������ � MainMenuScene!");
        }

        if (SceneManager.GetActiveScene().name == "MainMenuScene")
        {
            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(true);
                Debug.Log("GameManager: mainMenuPanel ����������� � MainMenuScene");
            }
            else
            {
                Debug.LogWarning("GameManager: mainMenuPanel �� �������� � ����������!");
            }
        }
        else
        {
            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(false);
            }
            if (gameplayUI != null)
            {
                gameplayUI.SetActive(true);
            }
            foreach (GameObject obj in gameplayObjects)
            {
                if (obj != null) obj.SetActive(true);
            }
        }
    }

    private void EnsureEventSystem()
    {
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();
            Debug.Log("GameManager: ������ ����� EventSystem");
        }
        else
        {
            Debug.Log("GameManager: EventSystem ��� ����������");
        }
    }

    public void StartGame()
    {
        Debug.Log("GameManager: StartGame ������, ������� �� MapScene");
        isGameActive = true;
        SceneManager.LoadScene("MapScene");
    }

    public void StartLevelGameplay()
    {
        StartCoroutine(DelayedStartLevelGameplay());
    }

    private IEnumerator DelayedStartLevelGameplay()
    {
        Debug.Log("GameManager: DelayedStartLevelGameplay �����");
        yield return new WaitForSeconds(0.5f);

        isGameActive = true;

        Canvas levelCanvas = FindObjectOfType<Canvas>();
        if (levelCanvas != null)
        {
            Debug.Log($"GameManager: Canvas ������, ���: {levelCanvas.gameObject.name}");
            Button[] buttons = levelCanvas.GetComponentsInChildren<Button>(true);
            Text[] texts = levelCanvas.GetComponentsInChildren<Text>(true);
            TextMeshProUGUI[] tmpTexts = levelCanvas.GetComponentsInChildren<TextMeshProUGUI>(true);
            Debug.Log($"GameManager: ������� {texts.Length} Text ����������� � {tmpTexts.Length} TextMeshProUGUI ����������� � Canvas");

            if (pauseButton == null)
            {
                foreach (Button btn in buttons)
                {
                    if (btn.name.Contains("Pause"))
                    {
                        pauseButton = btn;
                        Debug.Log($"GameManager: pauseButton ������ �����������, ���: {btn.name}");
                        break;
                    }
                }
            }

            if (speedButton == null)
            {
                foreach (Button btn in buttons)
                {
                    if (btn.name.Contains("Speed"))
                    {
                        speedButton = btn;
                        Debug.Log($"GameManager: speedButton ������ �����������, ���: {btn.name}");
                        break;
                    }
                }
            }

            if (backToMenuButton == null)
            {
                foreach (Button btn in buttons)
                {
                    if (btn.name.Contains("Back"))
                    {
                        backToMenuButton = btn;
                        Debug.Log($"GameManager: backToMenuButton ������ �����������, ���: {btn.name}");
                        break;
                    }
                }
            }

            if (speedText == null)
            {
                foreach (Text txt in texts)
                {
                    if (txt.name.Contains("Speed"))
                    {
                        speedText = txt;
                        Debug.Log($"GameManager: speedText ������ �����������, ���: {txt.name}");
                        break;
                    }
                }
            }

            if (goldText == null)
            {
                foreach (TextMeshProUGUI tmpTxt in tmpTexts)
                {
                    if (tmpTxt.name == "GoldText")
                    {
                        goldText = tmpTxt;
                        Debug.Log($"GameManager: GoldText ������ �����������, ���: {tmpTxt.name}");
                        break;
                    }
                }
            }
        }
        else
        {
            Debug.LogError("GameManager: Canvas �� ������ � Level1Scene!");
        }

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
            Debug.Log("GameManager: mainMenuPanel �������������");
        }

        if (gameplayUI != null)
        {
            gameplayUI.SetActive(true);
            Debug.Log("GameManager: gameplayUI �����������");
        }

        foreach (GameObject obj in gameplayObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }

        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(true);
            pauseButton.interactable = true;
            pauseButton.onClick.RemoveAllListeners();
            pauseButton.onClick.AddListener(TogglePause);
            Debug.Log($"GameManager: pauseButton �����������, ������������: {pauseButton.interactable}");
        }
        else
        {
            Debug.LogError("GameManager: pauseButton �� �������� � �� ������ � Level1Scene!");
        }

        if (speedButton != null)
        {
            speedButton.gameObject.SetActive(true);
            speedButton.interactable = true;
            speedButton.onClick.RemoveAllListeners();
            speedButton.onClick.AddListener(ChangeSpeed);
            Debug.Log($"GameManager: speedButton �����������, ������������: {speedButton.interactable}");
        }
        else
        {
            Debug.LogError("GameManager: speedButton �� �������� � �� ������ � Level1Scene!");
        }

        if (backToMenuButton != null)
        {
            backToMenuButton.gameObject.SetActive(true);
            backToMenuButton.interactable = true;
            backToMenuButton.onClick.RemoveAllListeners();
            backToMenuButton.onClick.AddListener(BackToMap);
            Debug.Log($"GameManager: backToMenuButton �����������, ������������: {backToMenuButton.interactable}");
        }
        else
        {
            Debug.LogError("GameManager: backToMenuButton �� �������� � �� ������ � Level1Scene!");
        }

        if (speedText != null)
        {
            speedText.text = currentSpeed + "x";
            Debug.Log("GameManager: speedText �������");
        }
        else
        {
            Debug.LogError("GameManager: speedText �� �������� � �� ������ � Level1Scene!");
        }

        if (goldText != null)
        {
            goldText.gameObject.SetActive(true);
            UpdateGoldText();
            Debug.Log($"GameManager: goldText �����������, ������� �����: {goldText.text}");
        }
        else
        {
            Debug.LogError("GameManager: GoldText �� ������ � Level1Scene!");
        }

        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager == null)
        {
            Debug.LogError("GameManager: LevelManager �� ������ � Level1Scene!");
            yield break;
        }

        LevelManager.main = levelManager;
        Debug.Log("GameManager: LevelManager ������, �������� StartGame");
        levelManager.StartGame();

        isPaused = false;
        Time.timeScale = 1;
        currentSpeed = 1;
        UpdateSpeedText();
    }

    public void BackToMap()
    {
        Debug.Log("GameManager: BackToMap ������");
        isGameActive = false;

        if (LevelManager.main != null)
        {
            LevelManager.main.StopGame();
            LevelManager.main = null;
            Debug.Log("GameManager: LevelManager.main �������");
        }

        SceneManager.LoadScene("MapScene");
    }

    public void ShowMainMenu()
    {
        Debug.Log("GameManager: ShowMainMenu ������");
        isGameActive = false;

        gold = initialGold;
        Debug.Log($"GameManager: ������ �������� �� {gold}");

        SceneManager.LoadScene("MainMenuScene");
    }

    private void SetupUIForMapScene()
    {
        Debug.Log("GameManager: SetupUIForMapScene ������");

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            Button[] buttons = canvas.GetComponentsInChildren<Button>(true);
            foreach (Button btn in buttons)
            {
                if (btn.name.Contains("Shop") && shopButton == null)
                {
                    shopButton = btn;
                    Debug.Log($"GameManager: shopButton ������ �����������, ���: {btn.name}");
                }
                if (btn.name.Contains("MainMenu") && mainMenuButton == null)
                {
                    mainMenuButton = btn;
                    Debug.Log($"GameManager: mainMenuButton ������ �����������, ���: {btn.name}");
                }
                if (btn.name.Contains("Unit1") && placeForUnit1Button == null)
                {
                    placeForUnit1Button = btn;
                    Debug.Log($"GameManager: placeForUnit1Button ������ �����������, ���: {btn.name}");
                }
                if (btn.name.Contains("Unit2") && placeForUnit2Button == null)
                {
                    placeForUnit2Button = btn;
                    Debug.Log($"GameManager: placeForUnit2Button ������ �����������, ���: {btn.name}");
                }
                if (btn.name.Contains("Unit3") && placeForUnit3Button == null)
                {
                    placeForUnit3Button = btn;
                    Debug.Log($"GameManager: placeForUnit3Button ������ �����������, ���: {btn.name}");
                }
            }

            if (shopPanel == null)
            {
                shopPanel = GameObject.Find("ShopPanel");
                if (shopPanel != null)
                {
                    Debug.Log($"GameManager: shopPanel ������ �����������, ���: {shopPanel.name}");
                }
            }
        }
        else
        {
            Debug.LogError("GameManager: Canvas �� ������ � MapScene!");
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.gameObject.SetActive(true);
            mainMenuButton.interactable = true;
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(ShowMainMenu);
            Debug.Log($"GameManager: mainMenuButton ��������, ������������: {mainMenuButton.interactable}");
        }
        else
        {
            Debug.LogError("GameManager: mainMenuButton �� �������� � �� ������ � MapScene!");
        }

        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
            Debug.Log("GameManager: shopPanel �������������");
        }
        else
        {
            Debug.LogError("GameManager: shopPanel �� ������ � MapScene!");
        }

        if (shopButton != null)
        {
            shopButton.gameObject.SetActive(true);
            shopButton.interactable = true;
            shopButton.onClick.RemoveAllListeners();
            shopButton.onClick.AddListener(OpenShop);
            Debug.Log($"GameManager: shopButton ��������, ������������: {shopButton.interactable}");
        }
        else
        {
            Debug.LogError("GameManager: shopButton �� �������� � �� ������ � MapScene!");
        }

        if (placeForUnit1Button != null)
        {
            placeForUnit1Button.gameObject.SetActive(true);
            placeForUnit1Button.interactable = true;
            placeForUnit1Button.onClick.RemoveAllListeners();
            placeForUnit1Button.onClick.AddListener(() => BuyPlaceForUnit(1));
            Debug.Log($"GameManager: placeForUnit1Button ��������, ������������: {placeForUnit1Button.interactable}");
        }
        else
        {
            Debug.LogWarning("GameManager: placeForUnit1Button �� ������ � MapScene!");
        }

        if (placeForUnit2Button != null)
        {
            placeForUnit2Button.gameObject.SetActive(true);
            placeForUnit2Button.interactable = level1Completed;
            if (!level1Completed && lockSprite != null)
            {
                placeForUnit2Button.GetComponent<Image>().sprite = lockSprite;
            }
            placeForUnit2Button.onClick.RemoveAllListeners();
            placeForUnit2Button.onClick.AddListener(() => BuyPlaceForUnit(2));
            Debug.Log($"GameManager: placeForUnit2Button ��������, ������������: {placeForUnit2Button.interactable}");
        }
        else
        {
            Debug.LogWarning("GameManager: placeForUnit2Button �� ������ � MapScene!");
        }

        if (placeForUnit3Button != null)
        {
            placeForUnit3Button.gameObject.SetActive(true);
            placeForUnit3Button.interactable = level2Completed;
            if (!level2Completed && lockSprite != null)
            {
                placeForUnit3Button.GetComponent<Image>().sprite = lockSprite;
            }
            placeForUnit3Button.onClick.RemoveAllListeners();
            placeForUnit3Button.onClick.AddListener(() => BuyPlaceForUnit(3));
            Debug.Log($"GameManager: placeForUnit3Button ��������, ������������: {placeForUnit3Button.interactable}");
        }
        else
        {
            Debug.LogWarning("GameManager: placeForUnit3Button �� ������ � MapScene!");
        }

        if (gameplayUI != null)
        {
            gameplayUI.SetActive(true);
            Debug.Log("GameManager: gameplayUI ����������� � MapScene");
        }

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
            Debug.Log("GameManager: mainMenuPanel ������������� � MapScene");
        }

        foreach (GameObject obj in gameplayObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }

    private void DestroyAllEnemies()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
        Debug.Log("GameManager: ��� ����� ����������");
    }

    private void DestroyAllUnits()
    {
        GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
        foreach (GameObject unit in units)
        {
            Destroy(unit);
        }
        Debug.Log("GameManager: ��� ����� ����������");
    }

    private void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : currentSpeed;
        Debug.Log($"GameManager: �����: {isPaused}");
    }

    private void ChangeSpeed()
    {
        currentSpeed++;
        if (currentSpeed > 3)
            currentSpeed = 1;

        if (!isPaused)
        {
            Time.timeScale = currentSpeed;
        }

        UpdateSpeedText();
        Debug.Log($"GameManager: �������� �������� �� {currentSpeed}x");
    }

    private void UpdateSpeedText()
    {
        if (speedText != null)
        {
            speedText.text = currentSpeed + "x";
        }
        else
        {
            Debug.LogWarning("GameManager: speedText �� �������� � ����������!");
        }
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateGoldText();
        Debug.Log($"GameManager: ��������� {amount} ������, ������: {gold}");
    }

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            UpdateGoldText();
            Debug.Log($"GameManager: ��������� {amount} ������, ��������: {gold}");
            return true;
        }
        Debug.Log($"GameManager: ������������ ������! ���������: {amount}, ����: {gold}");
        return false;
    }

    private void UpdateGoldText()
    {
        if (goldText != null)
        {
            goldText.text = "" + gold; // ������ "Gold: ", ��� ��� ������ �������� �����
            Debug.Log($"GameManager: goldText ������� �� '{goldText.text}'");
        }
        else
        {
            Debug.LogWarning("GameManager: goldText �� ������ ��� ����������!");
        }
    }

    public int GetGold()
    {
        return gold;
    }

    public void ApplyDamageUpgrade(float increase)
    {
        damageMultiplier += increase;
        Debug.Log($"GameManager: ��������� ����� ���������, ���������: {damageMultiplier}");
    }

    public float GetDamageMultiplier()
    {
        return damageMultiplier;
    }

    public void ExitGame()
    {
        Debug.Log("GameManager: ExitGame ������");
        Application.Quit();
    }

    public bool IsGameActive()
    {
        return isGameActive;
    }

    public void OpenShop()
    {
        if (shopPanel != null)
        {
            if (!isShopOpen)
            {
                foreach (GameObject obj in sceneObjectsToHide)
                {
                    if (obj != null)
                    {
                        obj.SetActive(false);
                    }
                }
                shopPanel.SetActive(true);
                isShopOpen = true;
                Debug.Log("GameManager: ������� ������, ��������� ������� ������");
            }
            else
            {
                CloseShop();
            }
        }
        else
        {
            Debug.LogError("GameManager: shopPanel �� �������� � �� ������ � MapScene!");
        }
    }

    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
            foreach (GameObject obj in sceneObjectsToHide)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }
            isShopOpen = false;
            Debug.Log("GameManager: ������� ������, ������� ����� �������������");
        }
    }

    public void BuyPlaceForUnit(int type)
    {
        int cost = type * 50; // ���������: 50, 100, 150
        if (SpendGold(cost))
        {
            Debug.Log($"PlaceForUnit {type} ��������������!");
            if (type == 2 && !level1Completed) level1Completed = true;
            if (type == 3 && !level2Completed) level2Completed = true;
            SetupUIForMapScene(); // ����������������� UI ����� �������
            CloseShop();
        }
    }

    public void CompleteLevel(int level)
    {
        if (level == 1) level1Completed = true;
        if (level == 2) level2Completed = true;
        SetupUIForMapScene(); // ����������������� UI ����� ���������� ������
        Debug.Log($"GameManager: ������� {level} �������, ������� �������");
    }
}