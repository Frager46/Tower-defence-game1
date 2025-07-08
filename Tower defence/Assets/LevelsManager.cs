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

    private bool isPaused = false;
    private int currentSpeed = 1;
    private bool isGameActive = false;

    // Static backup to preserve button reference
    private static Button _backupBackToMenuButton;

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
            // Restore backup if available
            if (_backupBackToMenuButton != null)
            {
                backToMenuButton = _backupBackToMenuButton;
                Debug.Log("LevelsManager: Restored backToMenuButton from backup: " + (backToMenuButton != null));
            }
            else if (backToMenuButton == null)
            {
                Debug.LogError("LevelsManager: backToMenuButton not assigned in Inspector! Please assign it before playing.");
                enabled = false; // Disable the script to prevent further execution
                return;
            }
            // Update backup with Inspector value
            if (backToMenuButton != null)
            {
                _backupBackToMenuButton = backToMenuButton;
                Debug.Log("LevelsManager: Updated backup with Inspector backToMenuButton: " + (_backupBackToMenuButton != null));
            }
        }
        else if (Instance != this)
        {
            Debug.Log("LevelsManager: Duplicate instance found, Current Scene: " + SceneManager.GetActiveScene().name);
            if (Instance.gameObject.scene != gameObject.scene && Instance.gameObject.activeInHierarchy)
            {
                Debug.Log("LevelsManager: Destroying duplicate from different scene");
                if (backToMenuButton != null)
                {
                    _backupBackToMenuButton = backToMenuButton;
                    Debug.Log("LevelsManager: Backed up new instance's backToMenuButton: " + (_backupBackToMenuButton != null));
                }
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
            // Backup the button reference before destruction
            if (backToMenuButton != null)
            {
                _backupBackToMenuButton = backToMenuButton;
                Debug.Log("LevelsManager: Backed up backToMenuButton: " + (_backupBackToMenuButton != null));
            }
            Instance = null;
            Debug.Log("LevelsManager: Instance reset to null");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("LevelsManager: OnSceneLoaded called, Scene: " + scene.name);
        Debug.Log("LevelsManager: LoadSceneMode: " + mode);
        Debug.Log("LevelsManager: Canvas count: " + FindObjectsOfType<Canvas>().Length);
        Debug.Log("LevelsManager: Instance: " + (Instance != null));
        if (scene.name == "Level1Scene" || scene.name == "Level2Scene" || scene.name == "Level4Scene") // Добавлен Level4Scene
        {
            Debug.Log("LevelsManager: Scene is Level1Scene or Level2Scene or Level4Scene");
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
            backToMenuButton = _backupBackToMenuButton != null ? _backupBackToMenuButton : this.backToMenuButton;
            Debug.Log("LevelsManager: backToMenuButton set to backup or Inspector: " + (backToMenuButton != null));
            if (backToMenuButton == null)
            {
                Debug.LogError("LevelsManager: backToMenuButton not assigned! Check Inspector or scene hierarchy.");
            }
            speedText = null;
            Debug.Log("LevelsManager: speedText reset to null");
            if (pauseButton == null)
            {
                Debug.Log("LevelsManager: Searching for pauseButton");
                foreach (Button btn in buttons)
                {
                    Debug.Log("LevelsManager: Checking button: " + btn.name);
                    if (btn.name.Contains("Pause"))
                    {
                        Debug.Log("LevelsManager: pauseButton name contains Pause");
                        pauseButton = btn;
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
                    Debug.Log("LevelsManager: Checking button: " + btn.name);
                    if (btn.name.Contains("Speed"))
                    {
                        Debug.Log("LevelsManager: speedButton name contains Speed");
                        speedButton = btn;
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
                        Debug.Log("LevelsManager: backToMenuButton assigned: " + (backToMenuButton != null));
                        _backupBackToMenuButton = backToMenuButton;
                        Debug.Log("LevelsManager: Updated backup with dynamically found backToMenuButton: " + (_backupBackToMenuButton != null));
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
                    Debug.Log("LevelsManager: Checking text: " + txt.name);
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
                    Debug.Log("LevelsManager: Checking TMPText: " + tmpTxt.name);
                    if (tmpTxt.name == "GoldText")
                    {
                        Debug.Log("LevelsManager: TMPText name is GoldText");
                        GameState.Instance.SetGoldText(tmpTxt);
                        Debug.Log("LevelsManager: GoldText assigned");
                        break;
                    }
                }
            }
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
            Debug.Log("LevelsManager: pauseButton not null");
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
            Debug.Log("LevelsManager: speedButton not null");
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
            Debug.Log("LevelsManager: backToMenuButton not null");
            backToMenuButton.gameObject.SetActive(true);
            Debug.Log("LevelsManager: backToMenuButton set active");
            backToMenuButton.interactable = true;
            Debug.Log("LevelsManager: backToMenuButton set interactable: " + backToMenuButton.interactable);
            backToMenuButton.onClick.RemoveAllListeners();
            Debug.Log("LevelsManager: backToMenuButton listeners removed");
            backToMenuButton.onClick.AddListener(BackToMap);
            Debug.Log("LevelsManager: backToMenuButton listener added: " + (backToMenuButton.onClick.GetPersistentEventCount() > 0));
        }
        EnsureEventSystem();
        Debug.Log("LevelsManager: EnsureEventSystem called");
        if (speedText != null)
        {
            Debug.Log("LevelsManager: speedText not null");
            speedText.text = currentSpeed + "x";
            Debug.Log("LevelsManager: speedText updated to " + currentSpeed + "x");
        }
        if (SceneManager.GetActiveScene().name == "Level1Scene")
        {
            Debug.Log("LevelsManager: Scene is Level1Scene");
            LevelManager levelManager = FindObjectOfType<LevelManager>();
            Debug.Log("LevelsManager: levelManager assigned");
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
            Debug.Log("LevelsManager: level2Manager assigned");
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
        else if (SceneManager.GetActiveScene().name == "Level4Scene") // Исправлено на Level4Manager
        {
            Debug.Log("LevelsManager: Scene is Level4Scene");
            Level4Manager level4Manager = FindObjectOfType<Level4Manager>();
            Debug.Log("LevelsManager: level4Manager assigned");
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
        isPaused = false;
        Debug.Log("LevelsManager: isPaused set to " + isPaused);
        Time.timeScale = 1;
        Debug.Log("LevelsManager: Time.timeScale set to " + Time.timeScale);
    }

    private void EnsureEventSystem()
    {
        Debug.Log("LevelsManager: EnsureEventSystem called");
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        Debug.Log("LevelsManager: eventSystem assigned");
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
            Debug.Log("LevelsManager: eventSystem not null");
            Debug.Log("LevelsManager: EventSystem already exists, name: " + eventSystem.name);
        }
    }

    private void TogglePause()
    {
        Debug.Log("LevelsManager: TogglePause called");
        isPaused = !isPaused;
        Debug.Log("LevelsManager: isPaused set to " + isPaused);
        Time.timeScale = isPaused ? 0 : currentSpeed;
        Debug.Log("LevelsManager: Time.timeScale set to " + Time.timeScale);
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
            Debug.Log("LevelsManager: speedText not null");
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
        Debug.Log("LevelsManager: BackToMap called");
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
        else if (SceneManager.GetActiveScene().name == "Level4Scene" && Level4Manager.main != null) // Исправлено на Level4Manager
        {
            Debug.Log("LevelsManager: Scene is Level4Scene and Level4Manager.main not null");
            Level4Manager.main.StopGame();
            Debug.Log("LevelsManager: Level4Manager.main.StopGame called");
            Level4Manager.main = null;
            Debug.Log("LevelsManager: Level4Manager.main reset");
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
            Debug.Log("LevelsManager: Destroying enemy");
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
            Debug.Log("LevelsManager: Destroying unit");
            Destroy(unit);
        }
        Debug.Log("LevelsManager: All units destroyed");
    }
}