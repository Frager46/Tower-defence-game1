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

    [Header("Currency")]
    public int initialGold = 75;
    private int gold;
    private TextMeshProUGUI goldText; // Это уже настроено для GoldText

    [Header("Upgrades")]
    private float damageMultiplier = 1f;

    [Header("Shop Settings")]
    public GameObject shopPanel;
    public Button shopButton; // Кнопка открытия магазина в MapScene
    public Button placeForUnit1Button;
    public Button placeForUnit2Button;
    public Button placeForUnit3Button;
    public Sprite lockSprite; // Спрайт замка для заблокированных мест
    public GameObject[] sceneObjectsToHide; // Объекты сцены, которые нужно скрыть при открытии магазина

    private bool isPaused = false;
    private int currentSpeed = 1;
    private bool isGameActive = false;
    private bool isShopOpen = false; // Флаг для отслеживания состояния магазина

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
        Debug.Log("GameManager: Start вызван в сцене: " + SceneManager.GetActiveScene().name);
        gold = initialGold;
        UpdateGoldText();
        InitializeUI();
        EnsureEventSystem();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"GameManager: Сцена {scene.name} загружена");
        if (scene.name == "Level1Scene")
        {
            StartCoroutine(DelayedStartLevelGameplay());
        }
        else if (scene.name == "MapScene")
        {
            SetupShop();
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void InitializeUI()
    {
        Debug.Log("GameManager: InitializeUI вызван");

        if (gameplayUI != null)
        {
            gameplayUI.SetActive(false);
            Debug.Log("GameManager: gameplayUI деактивирован");
        }

        foreach (GameObject obj in gameplayObjects)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        if (playButton != null)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(StartGame);
            playButton.gameObject.SetActive(true);
            Debug.Log($"GameManager: Назначен слушатель для playButton, интерактивен: {playButton.interactable}");
        }
        else
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                Button[] buttons = canvas.GetComponentsInChildren<Button>(true);
                foreach (Button btn in buttons)
                {
                    if (btn.name.Contains("Play"))
                    {
                        playButton = btn;
                        playButton.onClick.RemoveAllListeners();
                        playButton.onClick.AddListener(StartGame);
                        playButton.gameObject.SetActive(true);
                        Debug.Log($"GameManager: playButton найден динамически, интерактивен: {playButton.interactable}");
                        break;
                    }
                }
            }
            if (playButton == null)
            {
                Debug.LogError("GameManager: playButton не назначен и не найден в MainMenuScene!");
            }
        }

        if (SceneManager.GetActiveScene().name == "MainMenuScene")
        {
            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(true);
                Debug.Log("GameManager: mainMenuPanel активирован в MainMenuScene");
            }
            else
            {
                Debug.LogWarning("GameManager: mainMenuPanel не назначен в инспекторе!");
            }
        }
        else if (SceneManager.GetActiveScene().name != "Level1Scene")
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
            Debug.Log("GameManager: Создан новый EventSystem");
        }
        else
        {
            Debug.Log("GameManager: EventSystem уже существует");
        }
    }

    public void StartGame()
    {
        Debug.Log("GameManager: StartGame вызван, переход на MapScene");
        isGameActive = true;
        SceneManager.LoadScene("MapScene");
    }

    public void StartLevelGameplay()
    {
        StartCoroutine(DelayedStartLevelGameplay());
    }

    private IEnumerator DelayedStartLevelGameplay()
    {
        Debug.Log("GameManager: DelayedStartLevelGameplay начат");
        yield return new WaitForSeconds(0.5f);

        isGameActive = true;

        if (pauseButton == null || speedButton == null || backToMenuButton == null || speedText == null || goldText == null)
        {
            Canvas levelCanvas = FindObjectOfType<Canvas>();
            if (levelCanvas != null)
            {
                Debug.Log($"GameManager: Canvas найден, имя: {levelCanvas.gameObject.name}");
                Button[] buttons = levelCanvas.GetComponentsInChildren<Button>(true);
                Text[] texts = levelCanvas.GetComponentsInChildren<Text>(true);
                TextMeshProUGUI[] tmpTexts = levelCanvas.GetComponentsInChildren<TextMeshProUGUI>(true);
                Debug.Log($"GameManager: Найдено {texts.Length} Text компонентов и {tmpTexts.Length} TextMeshProUGUI компонентов в Canvas");

                foreach (Text txt in texts)
                {
                    Debug.Log($"GameManager: Обнаружен Text объект: {txt.gameObject.name}");
                    if (txt.name.Contains("Speed") && speedText == null) speedText = txt;
                }

                foreach (TextMeshProUGUI tmpTxt in tmpTexts)
                {
                    Debug.Log($"GameManager: Обнаружен TextMeshProUGUI объект: {tmpTxt.gameObject.name}");
                    if (tmpTxt.name == "GoldText" && goldText == null)
                    {
                        goldText = tmpTxt;
                        Debug.Log($"GameManager: GoldText найден, имя объекта: {goldText.gameObject.name}");
                    }
                }

                foreach (Button btn in buttons)
                {
                    if (btn.name.Contains("Pause") && pauseButton == null) pauseButton = btn;
                    if (btn.name.Contains("Speed") && speedButton == null) speedButton = btn;
                    if (btn.name.Contains("Back") && backToMenuButton == null) backToMenuButton = btn;
                }
                Debug.Log("GameManager: UI-объекты найдены динамически в Level1Scene");
            }
            else
            {
                Debug.LogError("GameManager: Canvas не найден в Level1Scene!");
            }
        }

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
            Debug.Log("GameManager: mainMenuPanel деактивирован");
        }

        if (gameplayUI != null)
        {
            gameplayUI.SetActive(true);
            Debug.Log("GameManager: gameplayUI активирован");
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
            pauseButton.onClick.RemoveAllListeners();
            pauseButton.onClick.AddListener(TogglePause);
            Debug.Log($"GameManager: pauseButton активирован, интерактивен: {pauseButton.interactable}");
        }
        else
        {
            Debug.LogError("GameManager: pauseButton не назначен или не найден в Level1Scene!");
        }

        if (speedButton != null)
        {
            speedButton.gameObject.SetActive(true);
            speedButton.onClick.RemoveAllListeners();
            speedButton.onClick.AddListener(ChangeSpeed);
            Debug.Log($"GameManager: speedButton активирован, интерактивен: {speedButton.interactable}");
        }
        else
        {
            Debug.LogError("GameManager: speedButton не назначен или не найден в Level1Scene!");
        }

        if (backToMenuButton != null)
        {
            backToMenuButton.gameObject.SetActive(true);
            backToMenuButton.onClick.RemoveAllListeners();
            backToMenuButton.onClick.AddListener(ShowMenu);
            Debug.Log($"GameManager: backToMenuButton активирован, интерактивен: {backToMenuButton.interactable}");
        }
        else
        {
            Debug.LogError("GameManager: backToMenuButton не назначен или не найден в Level1Scene!");
        }

        if (speedText != null)
        {
            speedText.text = currentSpeed + "x";
            Debug.Log("GameManager: speedText обновлён");
        }
        else
        {
            Debug.LogError("GameManager: speedText не назначен или не найден в Level1Scene!");
        }

        if (goldText != null)
        {
            goldText.gameObject.SetActive(true);
            UpdateGoldText();
            Debug.Log($"GameManager: goldText активирован, текущий текст: {goldText.text}");
        }
        else
        {
            Debug.LogError("GameManager: GoldText не найден в Level1Scene!");
        }

        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager == null)
        {
            Debug.LogError("GameManager: LevelManager не найден в сцене Level1Scene!");
            yield break;
        }

        LevelManager.main = levelManager;
        Debug.Log("GameManager: LevelManager найден, вызываем StartGame");
        levelManager.StartGame();

        isPaused = false;
        Time.timeScale = 1;
        currentSpeed = 1;
        UpdateSpeedText();
    }

    public void ShowMenu()
    {
        Debug.Log("GameManager: ShowMenu вызван");
        isGameActive = false;

        if (LevelManager.main != null)
        {
            LevelManager.main.StopGame();
            LevelManager.main = null;
            Debug.Log("GameManager: LevelManager.main сброшен");
        }

        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(false);
            pauseButton = null;
            Debug.Log("GameManager: pauseButton сброшен");
        }
        if (speedButton != null)
        {
            speedButton.gameObject.SetActive(false);
            speedButton = null;
            Debug.Log("GameManager: speedButton сброшен");
        }
        if (backToMenuButton != null)
        {
            backToMenuButton.gameObject.SetActive(false);
            backToMenuButton = null;
            Debug.Log("GameManager: backToMenuButton сброшен");
        }
        if (speedText != null)
        {
            speedText = null;
            Debug.Log("GameManager: speedText сброшен");
        }
        if (goldText != null)
        {
            goldText = null;
            Debug.Log("GameManager: goldText сброшен");
        }

        gold = initialGold;
        Debug.Log($"GameManager: Золото сброшено до {gold}");

        SceneManager.LoadScene("MainMenuScene");
        SceneManager.sceneLoaded += OnMainMenuSceneLoaded;
    }

    private void OnMainMenuSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenuScene")
        {
            Debug.Log("GameManager: MainMenuScene загружена, инициализация UI");
            InitializeUI();

            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(true);
                Debug.Log("GameManager: mainMenuPanel активирован в MainMenuScene");
            }

            if (gameplayUI != null)
            {
                gameplayUI.SetActive(false);
                Debug.Log("GameManager: gameplayUI деактивирован");
            }

            foreach (GameObject obj in gameplayObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }

            DestroyAllEnemies();
            DestroyAllUnits();

            isPaused = false;
            Time.timeScale = 1;
            currentSpeed = 1;
            UpdateSpeedText();

            SceneManager.sceneLoaded -= OnMainMenuSceneLoaded;
        }
    }

    private void DestroyAllEnemies()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
        Debug.Log("GameManager: Все враги уничтожены");
    }

    private void DestroyAllUnits()
    {
        GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
        foreach (GameObject unit in units)
        {
            Destroy(unit);
        }
        Debug.Log("GameManager: Все юниты уничтожены");
    }

    private void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : currentSpeed;
        Debug.Log($"GameManager: Пауза: {isPaused}");
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
        Debug.Log($"GameManager: Скорость изменена на {currentSpeed}x");
    }

    private void UpdateSpeedText()
    {
        if (speedText != null)
        {
            speedText.text = currentSpeed + "x";
        }
        else
        {
            Debug.LogWarning("GameManager: speedText не назначен в инспекторе!");
        }
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateGoldText();
        Debug.Log($"GameManager: Добавлено {amount} золота, теперь: {gold}");
    }

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            UpdateGoldText();
            Debug.Log($"GameManager: Потрачено {amount} золота, осталось: {gold}");
            return true;
        }
        Debug.Log($"GameManager: Недостаточно золота! Требуется: {amount}, есть: {gold}");
        return false;
    }

    private void UpdateGoldText()
    {
        if (goldText != null)
        {
            goldText.text = "" + gold; // Убрали "Gold: ", так как иконка заменяет текст
            Debug.Log($"GameManager: goldText обновлён на '{goldText.text}'");
        }
        else
        {
            Debug.LogWarning("GameManager: goldText не найден для обновления!");
        }
    }

    public int GetGold()
    {
        return gold;
    }

    public void ApplyDamageUpgrade(float increase)
    {
        damageMultiplier += increase;
        Debug.Log($"GameManager: Улучшение урона применено, множитель: {damageMultiplier}");
    }

    public float GetDamageMultiplier()
    {
        return damageMultiplier;
    }

    public void ExitGame()
    {
        Debug.Log("GameManager: ExitGame вызван");
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
                // Скрываем все объекты сцены
                foreach (GameObject obj in sceneObjectsToHide)
                {
                    if (obj != null)
                    {
                        obj.SetActive(false);
                    }
                }
                shopPanel.SetActive(true);
                isShopOpen = true;
                Debug.Log("GameManager: Магазин открыт, остальные объекты скрыты");
            }
            else
            {
                // Закрываем магазин при повторном нажатии
                CloseShop();
            }
        }
    }

    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
            // Восстанавливаем видимость объектов сцены
            foreach (GameObject obj in sceneObjectsToHide)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }
            isShopOpen = false;
            Debug.Log("GameManager: Магазин закрыт, объекты сцены восстановлены");
        }
    }

    public void BuyPlaceForUnit(int type)
    {
        int cost = type * 50; // Стоимость: 50, 100, 150
        if (SpendGold(cost))
        {
            Debug.Log($"PlaceForUnit {type} разблокировано!");
            // Здесь можно добавить логику активации PlaceForUnit в игре
            if (type == 2 && !level1Completed) level1Completed = true;
            if (type == 3 && !level2Completed) level2Completed = true;
            SetupShop();
            CloseShop(); // Закрываем магазин после успешной покупки
        }
    }

    private void SetupShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false); // По умолчанию закрыт

            if (shopButton != null)
            {
                shopButton.onClick.RemoveAllListeners();
                shopButton.onClick.AddListener(OpenShop);
                Debug.Log("GameManager: Слушатель для shopButton назначен");
            }

            if (placeForUnit1Button != null)
            {
                placeForUnit1Button.interactable = true;
                placeForUnit1Button.onClick.RemoveAllListeners();
                placeForUnit1Button.onClick.AddListener(() => BuyPlaceForUnit(1));
                Debug.Log("GameManager: placeForUnit1Button настроен");
            }

            if (placeForUnit2Button != null)
            {
                placeForUnit2Button.interactable = level1Completed;
                if (!level1Completed && lockSprite != null)
                {
                    placeForUnit2Button.GetComponent<Image>().sprite = lockSprite;
                }
                placeForUnit2Button.onClick.RemoveAllListeners();
                placeForUnit2Button.onClick.AddListener(() => BuyPlaceForUnit(2));
                Debug.Log("GameManager: placeForUnit2Button настроен");
            }

            if (placeForUnit3Button != null)
            {
                placeForUnit3Button.interactable = level2Completed;
                if (!level2Completed && lockSprite != null)
                {
                    placeForUnit3Button.GetComponent<Image>().sprite = lockSprite;
                }
                placeForUnit3Button.onClick.RemoveAllListeners();
                placeForUnit3Button.onClick.AddListener(() => BuyPlaceForUnit(3));
                Debug.Log("GameManager: placeForUnit3Button настроен");
            }
        }
    }

    public void CompleteLevel(int level)
    {
        if (level == 1) level1Completed = true;
        if (level == 2) level2Completed = true;
        SetupShop(); // Обновляем состояние кнопок
        Debug.Log($"GameManager: Уровень {level} пройден, обновлён магазин");
    }
}