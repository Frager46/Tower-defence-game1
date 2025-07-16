using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;

public class LevelsManager : MonoBehaviour
{
    public static LevelsManager Instance;

    public GameObject gameplayUI;
    public GameObject[] gameplayObjects;
    [Tooltip("Must be assigned in Inspector to the Back to Map button")]
    public Button backToMenuButton; // Required Inspector reference
    public Button pauseButton;
    public Button speedButton;
    public Text speedText;

    public GameObject pausedPanel; // Панель паузы
    public Button resumeButton; // Кнопка возобновления
    public Button closePausedButton; // Кнопка закрытия паузы
    public Button exitButton; // Кнопка выхода
    public GameObject questionPanel; // Панель подтверждения выхода
    public Button yesButton; // Кнопка "Да" для выхода
    public Button noButton; // Кнопка "Нет" для возврата

    private bool isPaused = false;
    private int currentSpeed = 1;
    private bool isGameActive = false;

    // Static backups to preserve references across scenes
    private static Button _backupBackToMenuButton;
    private static Button _backupPauseButton;
    private static Button _backupSpeedButton;
    private static Button _backupResumeButton;
    private static Button _backupClosePausedButton;
    private static Button _backupExitButton;
    private static Button _backupYesButton;
    private static Button _backupNoButton;

    private void Awake()
    {
        Debug.Log("LevelsManager: Awake called, Current Scene: " + SceneManager.GetActiveScene().name);
        Debug.Log("LevelsManager: Instance: " + (Instance != null));
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("LevelsManager: Instance set as new");
            DontDestroyOnLoad(gameObject);
            Debug.Log("LevelsManager: DontDestroyOnLoad applied");
            // Restore backups if available
            backToMenuButton = _backupBackToMenuButton != null ? _backupBackToMenuButton : backToMenuButton;
            pauseButton = _backupPauseButton != null ? _backupPauseButton : pauseButton;
            speedButton = _backupSpeedButton != null ? _backupSpeedButton : speedButton;
            resumeButton = _backupResumeButton != null ? _backupResumeButton : resumeButton;
            closePausedButton = _backupClosePausedButton != null ? _backupClosePausedButton : closePausedButton;
            exitButton = _backupExitButton != null ? _backupExitButton : exitButton;
            yesButton = _backupYesButton != null ? _backupYesButton : yesButton;
            noButton = _backupNoButton != null ? _backupNoButton : noButton;
            Debug.Log("LevelsManager: Restored button references from backups: pausedPanel=" + (pausedPanel != null) + ", questionPanel=" + (questionPanel != null));
        }
        else if (Instance != this)
        {
            Debug.Log("LevelsManager: Duplicate instance found, Current Scene: " + SceneManager.GetActiveScene().name);
            if (Instance.gameObject.scene != gameObject.scene && Instance.gameObject.activeInHierarchy)
            {
                Debug.Log("LevelsManager: Destroying duplicate from different scene");
                // Backup new instance's buttons
                _backupBackToMenuButton = backToMenuButton != null ? backToMenuButton : _backupBackToMenuButton;
                _backupPauseButton = pauseButton != null ? pauseButton : _backupPauseButton;
                _backupSpeedButton = speedButton != null ? speedButton : _backupSpeedButton;
                _backupResumeButton = resumeButton != null ? resumeButton : _backupResumeButton;
                _backupClosePausedButton = closePausedButton != null ? closePausedButton : _backupClosePausedButton;
                _backupExitButton = exitButton != null ? exitButton : _backupExitButton;
                _backupYesButton = yesButton != null ? yesButton : _backupYesButton;
                _backupNoButton = noButton != null ? noButton : _backupNoButton;
                Debug.Log("LevelsManager: Backed up new instance's buttons: pausedPanel=" + (pausedPanel != null) + ", questionPanel=" + (questionPanel != null));
                Destroy(gameObject);
                Debug.Log("LevelsManager: Destroyed self");
            }
            else
            {
                Debug.Log("LevelsManager: Preserving original instance");
            }
            return;
        }
        Debug.Log("LevelsManager: SceneManager.sceneLoaded += OnSceneLoaded");
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("LevelsManager: SceneManager.sceneLoaded event subscribed");
    }

    private void Start()
    {
        Debug.Log("LevelsManager: Start called, Current Scene: " + SceneManager.GetActiveScene().name);
        if (gameplayUI != null)
        {
            Debug.Log("LevelsManager: gameplayUI not null");
            gameplayUI.SetActive(false);
            Debug.Log("LevelsManager: gameplayUI deactivated");
        }
        foreach (GameObject obj in gameplayObjects)
        {
            Debug.Log("LevelsManager: Checking gameplayObject");
            if (obj != null)
            {
                Debug.Log("LevelsManager: gameplayObject not null");
                obj.SetActive(false);
                Debug.Log("LevelsManager: gameplayObject deactivated");
            }
        }
        if (pausedPanel != null)
        {
            pausedPanel.SetActive(false); // Панель паузы не видна по умолчанию
            Debug.Log("LevelsManager: pausedPanel deactivated by default, name: " + (pausedPanel != null ? pausedPanel.name : "null"));
        }
        else
        {
            Debug.Log("LevelsManager: pausedPanel is null at Start");
        }
        if (questionPanel != null)
        {
            questionPanel.SetActive(false); // Панель вопроса не видна по умолчанию
            Debug.Log("LevelsManager: questionPanel deactivated by default, name: " + (questionPanel != null ? questionPanel.name : "null"));
        }
        else
        {
            Debug.Log("LevelsManager: questionPanel is null at Start");
        }
    }

    private void OnDestroy()
    {
        Debug.Log("LevelsManager: OnDestroy called, Current Scene: " + SceneManager.GetActiveScene().name);
        if (Instance == this)
        {
            Debug.Log("LevelsManager: Instance == this");
            Debug.Log("LevelsManager: SceneManager.sceneLoaded -= OnSceneLoaded");
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Debug.Log("LevelsManager: Unsubscribed from SceneManager.sceneLoaded");
            // Backup all button references before destruction
            _backupBackToMenuButton = backToMenuButton;
            _backupPauseButton = pauseButton;
            _backupSpeedButton = speedButton;
            _backupResumeButton = resumeButton;
            _backupClosePausedButton = closePausedButton;
            _backupExitButton = exitButton;
            _backupYesButton = yesButton;
            _backupNoButton = noButton;
            Debug.Log("LevelsManager: Backed up all button references: pausedPanel=" + (pausedPanel != null) + ", questionPanel=" + (questionPanel != null));
            Instance = null;
            Debug.Log("LevelsManager: Instance reset to null");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("LevelsManager: OnSceneLoaded called, Scene: " + scene.name + ", Mode: " + mode);
        Debug.Log("LevelsManager: Canvas count: " + FindObjectsOfType<Canvas>().Length);
        Debug.Log("LevelsManager: Instance: " + (Instance != null));
        Debug.Log("LevelsManager: Initial panel states - pausedPanel: " + (pausedPanel != null ? pausedPanel.name : "null") + ", questionPanel: " + (questionPanel != null ? questionPanel.name : "null"));
        if (scene.name == "Level1Scene" || scene.name == "Level2Scene" || scene.name == "Level4Scene" || scene.name == "Level3Scene" || scene.name == "Level5Scene")
        {
            Debug.Log("LevelsManager: Scene is a level scene");
            Debug.Log("LevelsManager: Starting DelayedStartLevelGameplay");
            StartCoroutine(DelayedStartLevelGameplay());
        }
        else
        {
            Debug.Log("LevelsManager: Scene is not a level scene");
            if (gameplayUI != null)
            {
                Debug.Log("LevelsManager: gameplayUI not null");
                gameplayUI.SetActive(false);
                Debug.Log("LevelsManager: gameplayUI deactivated");
            }
            foreach (GameObject obj in gameplayObjects)
            {
                Debug.Log("LevelsManager: Checking gameplayObject");
                if (obj != null)
                {
                    Debug.Log("LevelsManager: gameplayObject not null");
                    obj.SetActive(false);
                    Debug.Log("LevelsManager: gameplayObject deactivated");
                }
            }
            if (pausedPanel != null)
            {
                pausedPanel.SetActive(false);
                Debug.Log("LevelsManager: pausedPanel deactivated, name: " + pausedPanel.name);
            }
            else
            {
                Debug.Log("LevelsManager: pausedPanel is null, cannot deactivate");
            }
            if (questionPanel != null)
            {
                questionPanel.SetActive(false);
                Debug.Log("LevelsManager: questionPanel deactivated, name: " + questionPanel.name);
            }
            else
            {
                Debug.Log("LevelsManager: questionPanel is null, cannot deactivate");
            }
            Debug.Log("LevelsManager: Resetting state");
            isGameActive = false;
            isPaused = false;
            currentSpeed = 1;
            Debug.Log("LevelsManager: State reset (isGameActive: " + isGameActive + ", isPaused: " + isPaused + ", currentSpeed: " + currentSpeed + ")");
        }
    }

    private IEnumerator DelayedStartLevelGameplay()
    {
        Debug.Log("LevelsManager: DelayedStartLevelGameplay started");
        yield return new WaitForSeconds(0.5f);
        Debug.Log("LevelsManager: WaitForSeconds completed");
        isGameActive = true;
        Debug.Log("LevelsManager: isGameActive set to " + isGameActive);
        Canvas levelCanvas = FindObjectOfType<Canvas>();
        Debug.Log("LevelsManager: levelCanvas assigned, Canvas: " + (levelCanvas != null ? levelCanvas.name : "null"));
        if (levelCanvas != null)
        {
            Debug.Log("LevelsManager: levelCanvas not null");
            Debug.Log("LevelsManager: Getting buttons");
            Button[] buttons = levelCanvas.GetComponentsInChildren<Button>(true);
            Debug.Log("LevelsManager: Buttons count: " + buttons.Length);
            Text[] texts = levelCanvas.GetComponentsInChildren<Text>(true);
            Debug.Log("LevelsManager: Texts count: " + texts.Length);
            TextMeshProUGUI[] tmpTexts = levelCanvas.GetComponentsInChildren<TextMeshProUGUI>(true);
            Debug.Log("LevelsManager: TMPTexts count: " + tmpTexts.Length);
            pauseButton = null;
            Debug.Log("LevelsManager: pauseButton reset to null");
            speedButton = null;
            Debug.Log("LevelsManager: speedButton reset to null");
            // Start with backup or Inspector reference
            backToMenuButton = _backupBackToMenuButton != null ? _backupBackToMenuButton : backToMenuButton;
            resumeButton = _backupResumeButton != null ? _backupResumeButton : resumeButton;
            closePausedButton = _backupClosePausedButton != null ? _backupClosePausedButton : closePausedButton;
            exitButton = _backupExitButton != null ? _backupExitButton : exitButton;
            yesButton = _backupYesButton != null ? _backupYesButton : yesButton;
            noButton = _backupNoButton != null ? _backupNoButton : noButton;
            speedText = null;
            Debug.Log("LevelsManager: UI references updated with backups: pausedPanel=" + (pausedPanel != null ? pausedPanel.name : "null") + ", questionPanel=" + (questionPanel != null ? questionPanel.name : "null"));
            if (backToMenuButton == null)
            {
                Debug.LogError("LevelsManager: backToMenuButton not assigned! Check Inspector or scene hierarchy.");
            }
            if (pauseButton == null)
            {
                Debug.Log("LevelsManager: Searching for pauseButton");
                foreach (Button btn in buttons)
                {
                    Debug.Log("LevelsManager: Checking button: " + btn.name + ", Active: " + btn.gameObject.activeInHierarchy);
                    if (btn.name.Contains("Pause"))
                    {
                        Debug.Log("LevelsManager: pauseButton name contains Pause");
                        pauseButton = btn;
                        _backupPauseButton = pauseButton;
                        Debug.Log("LevelsManager: pauseButton assigned: " + (pauseButton != null));
                        break;
                    }
                }
                if (pauseButton == null)
                {
                    Debug.Log("LevelsManager: pauseButton not found");
                }
            }
            if (speedButton == null)
            {
                Debug.Log("LevelsManager: Searching for speedButton");
                foreach (Button btn in buttons)
                {
                    Debug.Log("LevelsManager: Checking button: " + btn.name + ", Active: " + btn.gameObject.activeInHierarchy);
                    if (btn.name.Contains("Speed"))
                    {
                        Debug.Log("LevelsManager: speedButton name contains Speed");
                        speedButton = btn;
                        _backupSpeedButton = speedButton;
                        Debug.Log("LevelsManager: speedButton assigned: " + (speedButton != null));
                        break;
                    }
                }
                if (speedButton == null)
                {
                    Debug.Log("LevelsManager: speedButton not found");
                }
            }
            if (backToMenuButton == null)
            {
                Debug.Log("LevelsManager: Searching for backToMenuButton");
                foreach (Button btn in buttons)
                {
                    Debug.Log("LevelsManager: Checking button: " + btn.name + ", Active: " + btn.gameObject.activeInHierarchy);
                    if (btn.name.Contains("Back") || btn.name.Contains("BackToMap"))
                    {
                        Debug.Log("LevelsManager: backToMenuButton name matches: " + btn.name);
                        backToMenuButton = btn;
                        _backupBackToMenuButton = backToMenuButton;
                        Debug.Log("LevelsManager: backToMenuButton assigned: " + (backToMenuButton != null));
                        break;
                    }
                }
                if (backToMenuButton == null)
                {
                    Debug.LogError("LevelsManager: backToMenuButton not found in scene hierarchy");
                }
            }
            else
            {
                Debug.Log("LevelsManager: Using existing backToMenuButton: " + backToMenuButton.name);
            }
            if (speedText == null)
            {
                Debug.Log("LevelsManager: Searching for speedText");
                foreach (Text txt in texts)
                {
                    Debug.Log("LevelsManager: Checking text: " + txt.name + ", Active: " + txt.gameObject.activeInHierarchy);
                    if (txt.name.Contains("Speed"))
                    {
                        Debug.Log("LevelsManager: speedText name contains Speed");
                        speedText = txt;
                        Debug.Log("LevelsManager: speedText assigned: " + (speedText != null));
                        break;
                    }
                }
                if (speedText == null)
                {
                    Debug.Log("LevelsManager: speedText not found");
                }
            }
            if (GameState.Instance != null)
            {
                Debug.Log("LevelsManager: GameState.Instance not null");
                foreach (TextMeshProUGUI tmpTxt in tmpTexts)
                {
                    Debug.Log("LevelsManager: Checking TMPText: " + tmpTxt.name + ", Active: " + tmpTxt.gameObject.activeInHierarchy);
                    if (tmpTxt.name == "GoldText")
                    {
                        Debug.Log("LevelsManager: TMPText name is GoldText");
                        GameState.Instance.SetGoldText(tmpTxt);
                        Debug.Log("LevelsManager: GoldText assigned");
                        break;
                    }
                }
            }
            // Enhanced panel restoration with named button assignment
            Debug.Log("LevelsManager: Checking panel restoration - pausedPanel: " + (pausedPanel != null ? pausedPanel.name : "null") + ", questionPanel: " + (questionPanel != null ? questionPanel.name : "null"));
            if (pausedPanel == null)
            {
                pausedPanel = GameObject.Find("/Canvas/PausedPanel") ?? GameObject.Find("PausedPanel");
                if (pausedPanel == null)
                {
                    Debug.LogWarning("LevelsManager: pausedPanel not found with Find, searching all inactive objects");
                    pausedPanel = FindInActiveObjectByName("PausedPanel");
                }
                if (pausedPanel != null)
                {
                    pausedPanel.SetActive(false);
                    Debug.Log("LevelsManager: Restored and deactivated pausedPanel: " + pausedPanel.name);
                    Button[] pausedButtons = pausedPanel.GetComponentsInChildren<Button>(true);
                    foreach (Button btn in pausedButtons)
                    {
                        if (btn.name.Contains("Resume")) { resumeButton = btn; _backupResumeButton = resumeButton; Debug.Log("LevelsManager: Assigned resumeButton: " + btn.name); }
                        else if (btn.name.Contains("ClosePaused")) { closePausedButton = btn; _backupClosePausedButton = closePausedButton; Debug.Log("LevelsManager: Assigned closePausedButton: " + btn.name); }
                        else if (btn.name.Contains("Exit")) { exitButton = btn; _backupExitButton = exitButton; Debug.Log("LevelsManager: Assigned exitButton: " + btn.name); }
                    }
                    Debug.Log("LevelsManager: Restored buttons from pausedPanel: resume=" + (resumeButton != null) + ", close=" + (closePausedButton != null) + ", exit=" + (exitButton != null));
                }
                else
                {
                    Debug.LogError("LevelsManager: Failed to restore pausedPanel, check scene hierarchy!");
                }
            }
            if (questionPanel == null)
            {
                questionPanel = GameObject.Find("/Canvas/QuestionPanel") ?? GameObject.Find("QuestionPanel");
                if (questionPanel == null)
                {
                    Debug.LogWarning("LevelsManager: questionPanel not found with Find, searching all inactive objects");
                    questionPanel = FindInActiveObjectByName("QuestionPanel");
                }
                if (questionPanel != null)
                {
                    questionPanel.SetActive(false);
                    Debug.Log("LevelsManager: Restored and deactivated questionPanel: " + questionPanel.name);
                    Button[] questionButtons = questionPanel.GetComponentsInChildren<Button>(true);
                    foreach (Button btn in questionButtons)
                    {
                        if (btn.name.Contains("Yes")) { yesButton = btn; _backupYesButton = yesButton; Debug.Log("LevelsManager: Assigned yesButton: " + btn.name); }
                        else if (btn.name.Contains("No")) { noButton = btn; _backupNoButton = noButton; Debug.Log("LevelsManager: Assigned noButton: " + btn.name); }
                    }
                    Debug.Log("LevelsManager: Restored buttons from questionPanel: yes=" + (yesButton != null) + ", no=" + (noButton != null));
                }
                else
                {
                    Debug.LogError("LevelsManager: Failed to restore questionPanel, check scene hierarchy!");
                }
            }
            Debug.Log("LevelsManager: After restoration - pausedPanel: " + (pausedPanel != null ? pausedPanel.name : "null") + ", questionPanel: " + (questionPanel != null ? questionPanel.name : "null"));
        }
        else
        {
            Debug.LogError("LevelsManager: levelCanvas is null, check scene hierarchy!");
        }
        if (gameplayUI != null)
        {
            Debug.Log("LevelsManager: gameplayUI not null");
            gameplayUI.SetActive(true);
            Debug.Log("LevelsManager: gameplayUI activated");
        }
        foreach (GameObject obj in gameplayObjects)
        {
            Debug.Log("LevelsManager: Checking gameplayObject");
            if (obj != null)
            {
                Debug.Log("LevelsManager: gameplayObject not null");
                obj.SetActive(true);
                Debug.Log("LevelsManager: gameplayObject activated");
            }
        }
        if (pauseButton != null)
        {
            Debug.Log("LevelsManager: pauseButton not null, name: " + pauseButton.name);
            pauseButton.gameObject.SetActive(true);
            Debug.Log("LevelsManager: pauseButton set active");
            pauseButton.interactable = true;
            Debug.Log("LevelsManager: pauseButton set interactable: " + pauseButton.interactable);
            pauseButton.onClick.RemoveAllListeners();
            Debug.Log("LevelsManager: pauseButton listeners removed");
            pauseButton.onClick.AddListener(TogglePause);
            Debug.Log("LevelsManager: pauseButton listener added: " + (pauseButton.onClick.GetPersistentEventCount() > 0));
        }
        if (speedButton != null)
        {
            Debug.Log("LevelsManager: speedButton not null, name: " + speedButton.name);
            speedButton.gameObject.SetActive(true);
            Debug.Log("LevelsManager: speedButton set active");
            speedButton.interactable = true;
            Debug.Log("LevelsManager: speedButton set interactable: " + speedButton.interactable);
            speedButton.onClick.RemoveAllListeners();
            Debug.Log("LevelsManager: speedButton listeners removed");
            speedButton.onClick.AddListener(ChangeSpeed);
            Debug.Log("LevelsManager: speedButton listener added: " + (speedButton.onClick.GetPersistentEventCount() > 0));
        }
        if (backToMenuButton != null)
        {
            Debug.Log("LevelsManager: backToMenuButton not null, name: " + backToMenuButton.name);
            backToMenuButton.gameObject.SetActive(true);
            Debug.Log("LevelsManager: backToMenuButton set active");
            backToMenuButton.interactable = true;
            Debug.Log("LevelsManager: backToMenuButton set interactable: " + backToMenuButton.interactable);
            backToMenuButton.onClick.RemoveAllListeners();
            Debug.Log("LevelsManager: backToMenuButton listeners removed");
            backToMenuButton.onClick.AddListener(BackToMap);
            Debug.Log("LevelsManager: backToMenuButton listener added: " + (backToMenuButton.onClick.GetPersistentEventCount() > 0));
        }
        if (resumeButton != null)
        {
            Debug.Log("LevelsManager: resumeButton not null, name: " + resumeButton.name);
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(ResumeGame);
            Debug.Log("LevelsManager: resumeButton listener added");
        }
        if (closePausedButton != null)
        {
            Debug.Log("LevelsManager: closePausedButton not null, name: " + closePausedButton.name);
            closePausedButton.onClick.RemoveAllListeners();
            closePausedButton.onClick.AddListener(ResumeGame);
            Debug.Log("LevelsManager: closePausedButton listener added");
        }
        if (exitButton != null)
        {
            Debug.Log("LevelsManager: exitButton not null, name: " + exitButton.name);
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(ShowQuestionPanel);
            Debug.Log("LevelsManager: exitButton listener added");
        }
        if (yesButton != null)
        {
            Debug.Log("LevelsManager: yesButton not null, name: " + yesButton.name);
            yesButton.onClick.RemoveAllListeners();
            yesButton.onClick.AddListener(ExitToMap);
            Debug.Log("LevelsManager: yesButton listener added");
        }
        if (noButton != null)
        {
            Debug.Log("LevelsManager: noButton not null, name: " + noButton.name);
            noButton.onClick.RemoveAllListeners();
            noButton.onClick.AddListener(ShowPausedPanel);
            Debug.Log("LevelsManager: noButton listener added");
        }
        EnsureEventSystem();
        Debug.Log("LevelsManager: EnsureEventSystem called");
        if (speedText != null)
        {
            Debug.Log("LevelsManager: speedText not null, name: " + speedText.name);
            speedText.text = currentSpeed + "x";
            Debug.Log("LevelsManager: speedText updated to " + currentSpeed + "x");
        }
        if (SceneManager.GetActiveScene().name == "Level1Scene")
        {
            Debug.Log("LevelsManager: Scene is Level1Scene");
            LevelManager levelManager = FindObjectOfType<LevelManager>();
            Debug.Log("LevelsManager: levelManager assigned: " + (levelManager != null));
            if (levelManager == null)
            {
                Debug.Log("LevelsManager: levelManager is null");
                Debug.Log("LevelsManager: LevelManager not found in Level1Scene!");
                yield break;
            }
            LevelManager.main = levelManager;
            Debug.Log("LevelsManager: LevelManager.main set");
            Debug.Log("LevelsManager: Calling StartGame");
            levelManager.StartGame();
        }
        else if (SceneManager.GetActiveScene().name == "Level2Scene")
        {
            Debug.Log("LevelsManager: Scene is Level2Scene");
            Level2Manager level2Manager = FindObjectOfType<Level2Manager>();
            Debug.Log("LevelsManager: level2Manager assigned: " + (level2Manager != null));
            if (level2Manager == null)
            {
                Debug.Log("LevelsManager: level2Manager is null");
                Debug.Log("LevelsManager: Level2Manager not found in Level2Scene!");
                yield break;
            }
            Level2Manager.main = level2Manager;
            Debug.Log("LevelsManager: Level2Manager.main set");
            Debug.Log("LevelsManager: Calling StartGame");
            level2Manager.StartGame();
        }
        else if (SceneManager.GetActiveScene().name == "Level4Scene")
        {
            Debug.Log("LevelsManager: Scene is Level4Scene");
            Level4Manager level4Manager = FindObjectOfType<Level4Manager>();
            Debug.Log("LevelsManager: level4Manager assigned: " + (level4Manager != null));
            if (level4Manager == null)
            {
                Debug.Log("LevelsManager: level4Manager is null");
                Debug.Log("LevelsManager: Level4Manager not found in Level4Scene!");
                yield break;
            }
            Level4Manager.main = level4Manager;
            Debug.Log("LevelsManager: Level4Manager.main set");
            Debug.Log("LevelsManager: Calling StartGame");
            level4Manager.StartGame();
        }
        else if (SceneManager.GetActiveScene().name == "Level3Scene")
        {
            Debug.Log("LevelsManager: Scene is Level3Scene");
            Level3Manager level3Manager = FindObjectOfType<Level3Manager>();
            Debug.Log("LevelsManager: level3Manager assigned: " + (level3Manager != null));
            if (level3Manager == null)
            {
                Debug.Log("LevelsManager: level3Manager is null");
                Debug.Log("LevelsManager: Level3Manager not found in Level3Scene!");
                yield break;
            }
            Level3Manager.main = level3Manager;
            Debug.Log("LevelsManager: Level3Manager.main set");
            Debug.Log("LevelsManager: Calling StartGame");
            level3Manager.StartGame();
        }
        else if (SceneManager.GetActiveScene().name == "Level5Scene")
        {
            Debug.Log("LevelsManager: Scene is Level5Scene");
            Level5Manager level5Manager = FindObjectOfType<Level5Manager>();
            Debug.Log("LevelsManager: level5Manager assigned: " + (level5Manager != null));
            if (level5Manager == null)
            {
                Debug.Log("LevelsManager: level5Manager is null");
                Debug.Log("LevelsManager: Level5Manager not found in Level5Scene!");
                yield break;
            }
            Level5Manager.main = level5Manager;
            Debug.Log("LevelsManager: Level5Manager.main set");
            Debug.Log("LevelsManager: Calling StartGame");
            level5Manager.StartGame();
        }
        isPaused = false;
        Debug.Log("LevelsManager: isPaused set to " + isPaused);
        Time.timeScale = 1;
        Debug.Log("LevelsManager: Time.timeScale set to " + Time.timeScale);
    }

    private GameObject FindInActiveObjectByName(string name)
    {
        Debug.Log("LevelsManager: Searching for inactive object: " + name);
        Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform transform in allTransforms)
        {
            if (transform.name == name && !transform.gameObject.activeInHierarchy)
            {
                Debug.Log("LevelsManager: Found inactive object: " + transform.name);
                return transform.gameObject;
            }
        }
        Debug.Log("LevelsManager: No inactive object found with name: " + name);
        return null;
    }

    private void EnsureEventSystem()
    {
        Debug.Log("LevelsManager: EnsureEventSystem called");
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        Debug.Log("LevelsManager: eventSystem assigned: " + (eventSystem != null));
        if (eventSystem == null)
        {
            Debug.Log("LevelsManager: eventSystem is null");
            GameObject eventSystemObj = new GameObject("EventSystem");
            Debug.Log("LevelsManager: eventSystemObj created");
            eventSystemObj.AddComponent<EventSystem>();
            Debug.Log("LevelsManager: EventSystem component added");
            eventSystemObj.AddComponent<StandaloneInputModule>();
            Debug.Log("LevelsManager: StandaloneInputModule component added");
        }
        else
        {
            Debug.Log("LevelsManager: eventSystem not null, name: " + eventSystem.name);
        }
    }

    private void TogglePause()
    {
        Debug.Log("LevelsManager: TogglePause called");
        isPaused = !isPaused;
        Debug.Log("LevelsManager: isPaused set to " + isPaused + ", pausedPanel: " + (pausedPanel != null ? pausedPanel.name : "null"));
        if (isPaused)
        {
            if (pausedPanel != null)
            {
                pausedPanel.SetActive(true);
                Debug.Log("LevelsManager: pausedPanel activated, name: " + pausedPanel.name);
            }
            else
            {
                Debug.LogError("LevelsManager: pausedPanel is null, cannot activate");
            }
            Time.timeScale = 0;
        }
        else
        {
            if (pausedPanel != null)
            {
                pausedPanel.SetActive(false);
                Debug.Log("LevelsManager: pausedPanel deactivated, name: " + pausedPanel.name);
            }
            else
            {
                Debug.LogError("LevelsManager: pausedPanel is null, cannot deactivate");
            }
            Time.timeScale = currentSpeed;
        }
        Debug.Log("LevelsManager: Time.timeScale set to " + Time.timeScale);
    }

    private void ResumeGame()
    {
        Debug.Log("LevelsManager: ResumeGame called, pausedPanel: " + (pausedPanel != null ? pausedPanel.name : "null"));
        isPaused = false;
        if (pausedPanel != null)
        {
            pausedPanel.SetActive(false);
            Debug.Log("LevelsManager: pausedPanel deactivated, name: " + pausedPanel.name);
        }
        else
        {
            Debug.LogError("LevelsManager: pausedPanel is null, cannot deactivate");
        }
        Time.timeScale = currentSpeed;
        Debug.Log("LevelsManager: Time.timeScale set to " + Time.timeScale);
    }

    private void ShowQuestionPanel()
    {
        Debug.Log("LevelsManager: ShowQuestionPanel called, pausedPanel: " + (pausedPanel != null ? pausedPanel.name : "null") + ", questionPanel: " + (questionPanel != null ? questionPanel.name : "null"));
        if (pausedPanel != null)
        {
            pausedPanel.SetActive(false);
            Debug.Log("LevelsManager: pausedPanel deactivated, name: " + pausedPanel.name);
        }
        else
        {
            Debug.LogError("LevelsManager: pausedPanel is null, cannot deactivate");
        }
        if (questionPanel != null)
        {
            questionPanel.SetActive(true);
            Debug.Log("LevelsManager: questionPanel activated, name: " + questionPanel.name);
        }
        else
        {
            Debug.LogError("LevelsManager: questionPanel is null, cannot activate");
        }
    }

    private void ExitToMap()
    {
        Debug.Log("LevelsManager: ExitToMap called, pausedPanel: " + (pausedPanel != null ? pausedPanel.name : "null") + ", questionPanel: " + (questionPanel != null ? questionPanel.name : "null"));
        isGameActive = false;
        isPaused = false;
        Time.timeScale = 1;
        if (SceneManager.GetActiveScene().name == "Level1Scene" && LevelManager.main != null)
        {
            LevelManager.main.StopGame();
            LevelManager.main = null;
        }
        else if (SceneManager.GetActiveScene().name == "Level2Scene" && Level2Manager.main != null)
        {
            Level2Manager.main.StopGame();
            Level2Manager.main = null;
        }
        else if (SceneManager.GetActiveScene().name == "Level4Scene" && Level4Manager.main != null)
        {
            Level4Manager.main.StopGame();
            Level4Manager.main = null;
        }
        else if (SceneManager.GetActiveScene().name == "Level3Scene" && Level3Manager.main != null)
        {
            Level3Manager.main.StopGame();
            Level3Manager.main = null;
        }
        else if (SceneManager.GetActiveScene().name == "Level5Scene" && Level5Manager.main != null)
        {
            Level5Manager.main.StopGame();
            Level5Manager.main = null;
        }
        SceneManager.LoadScene("MapScene");
    }

    private void ShowPausedPanel()
    {
        Debug.Log("LevelsManager: ShowPausedPanel called, questionPanel: " + (questionPanel != null ? questionPanel.name : "null") + ", pausedPanel: " + (pausedPanel != null ? pausedPanel.name : "null"));
        if (questionPanel != null)
        {
            questionPanel.SetActive(false);
            Debug.Log("LevelsManager: questionPanel deactivated, name: " + questionPanel.name);
        }
        else
        {
            Debug.LogError("LevelsManager: questionPanel is null, cannot deactivate");
        }
        if (pausedPanel != null)
        {
            pausedPanel.SetActive(true);
            Debug.Log("LevelsManager: pausedPanel activated, name: " + pausedPanel.name);
        }
        else
        {
            Debug.LogError("LevelsManager: pausedPanel is null, cannot activate");
        }
    }

    private void ChangeSpeed()
    {
        Debug.Log("LevelsManager: ChangeSpeed called");
        currentSpeed++;
        Debug.Log("LevelsManager: currentSpeed incremented to " + currentSpeed);
        if (currentSpeed > 3)
        {
            Debug.Log("LevelsManager: currentSpeed exceeds 3");
            currentSpeed = 1;
            Debug.Log("LevelsManager: currentSpeed reset to " + currentSpeed);
        }
        if (!isPaused)
        {
            Debug.Log("LevelsManager: isPaused is false");
            Time.timeScale = currentSpeed;
            Debug.Log("LevelsManager: Time.timeScale set to " + Time.timeScale);
        }
        UpdateSpeedText();
        Debug.Log("LevelsManager: Speed changed to " + currentSpeed + "x");
    }

    private void UpdateSpeedText()
    {
        Debug.Log("LevelsManager: UpdateSpeedText called");
        if (speedText != null)
        {
            Debug.Log("LevelsManager: speedText not null, name: " + speedText.name);
            speedText.text = currentSpeed + "x";
            Debug.Log("LevelsManager: speedText updated to " + currentSpeed + "x");
        }
        else
        {
            Debug.Log("LevelsManager: speedText is null");
            Debug.Log("LevelsManager: speedText not assigned!");
        }
    }

    public void BackToMap()
    {
        Debug.Log("LevelsManager: BackToMap called, pausedPanel: " + (pausedPanel != null ? pausedPanel.name : "null") + ", questionPanel: " + (questionPanel != null ? questionPanel.name : "null"));
        isGameActive = false;
        Debug.Log("LevelsManager: isGameActive set to " + isGameActive);
        isPaused = false;
        Debug.Log("LevelsManager: isPaused set to " + isPaused);
        Time.timeScale = 1;
        Debug.Log("LevelsManager: Time.timeScale set to " + Time.timeScale);
        if (SceneManager.GetActiveScene().name == "Level1Scene" && LevelManager.main != null)
        {
            Debug.Log("LevelsManager: Scene is Level1Scene and LevelManager.main not null");
            LevelManager.main.StopGame();
            Debug.Log("LevelsManager: LevelManager.main.StopGame called");
            LevelManager.main = null;
            Debug.Log("LevelsManager: LevelManager.main reset");
        }
        else if (SceneManager.GetActiveScene().name == "Level2Scene" && Level2Manager.main != null)
        {
            Debug.Log("LevelsManager: Scene is Level2Scene and Level2Manager.main not null");
            Level2Manager.main.StopGame();
            Debug.Log("LevelsManager: Level2Manager.main.StopGame called");
            Level2Manager.main = null;
            Debug.Log("LevelsManager: Level2Manager.main reset");
        }
        else if (SceneManager.GetActiveScene().name == "Level4Scene" && Level4Manager.main != null)
        {
            Debug.Log("LevelsManager: Scene is Level4Scene and Level4Manager.main not null");
            Level4Manager.main.StopGame();
            Debug.Log("LevelsManager: Level4Manager.main.StopGame called");
            Level4Manager.main = null;
            Debug.Log("LevelsManager: Level4Manager.main reset");
        }
        else if (SceneManager.GetActiveScene().name == "Level3Scene" && Level3Manager.main != null)
        {
            Debug.Log("LevelsManager: Scene is Level3Scene and Level3Manager.main not null");
            Level3Manager.main.StopGame();
            Debug.Log("LevelsManager: Level3Manager.main.StopGame called");
            Level3Manager.main = null;
            Debug.Log("LevelsManager: Level3Manager.main reset");
        }
        else if (SceneManager.GetActiveScene().name == "Level5Scene" && Level5Manager.main != null)
        {
            Debug.Log("LevelsManager: Scene is Level5Scene and Level5Manager.main not null");
            Level5Manager.main.StopGame();
            Debug.Log("LevelsManager: Level5Manager.main.StopGame called");
            Level5Manager.main = null;
            Debug.Log("LevelsManager: Level5Manager.main reset");
        }
        Debug.Log("LevelsManager: Loading MapScene");
        SceneManager.LoadScene("MapScene");
    }

    public bool IsGameActive()
    {
        Debug.Log("LevelsManager: IsGameActive called, returning " + isGameActive);
        return isGameActive;
    }

    private void DestroyAllEnemies()
    {
        Debug.Log("LevelsManager: DestroyAllEnemies called");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log("LevelsManager: enemies count: " + enemies.Length);
        foreach (GameObject enemy in enemies)
        {
            Debug.Log("LevelsManager: Destroying enemy: " + enemy.name);
            Destroy(enemy);
        }
        Debug.Log("LevelsManager: All enemies destroyed");
    }

    private void DestroyAllUnits()
    {
        Debug.Log("LevelsManager: DestroyAllUnits called");
        GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
        Debug.Log("LevelsManager: units count: " + units.Length);
        foreach (GameObject unit in units)
        {
            Debug.Log("LevelsManager: Destroying unit: " + unit.name);
            Destroy(unit);
        }
        Debug.Log("LevelsManager: All units destroyed");
    }
}