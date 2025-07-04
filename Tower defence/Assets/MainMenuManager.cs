using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public Button playButton;

    private void Awake()
    {
        Debug.Log("MainMenuManager: Awake called");
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("MainMenuManager: SceneManager.sceneLoaded event subscribed");
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("MainMenuManager: OnDestroy called, unsubscribed from SceneManager.sceneLoaded");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"MainMenuManager: OnSceneLoaded called for scene: {scene.name}");
        if (scene.name == "MainMenuScene")
        {
            InitializeUI();
            EnsureEventSystem();
            Debug.Log("MainMenuManager: InitializeUI and EnsureEventSystem completed");
        }
        else
        {
            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(false);
                Debug.Log("MainMenuManager: mainMenuPanel deactivated in other scene");
            }
        }
    }

    private void InitializeUI()
    {
        Debug.Log("MainMenuManager: InitializeUI called");

        // Reset UI references
        mainMenuPanel = null;
        playButton = null;
        Debug.Log("MainMenuManager: UI references reset");

        mainMenuPanel = GameObject.Find("StartPanel");
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
            Debug.Log("MainMenuManager: mainMenuPanel activated");
        }
        else
        {
            Debug.LogWarning("MainMenuManager: StartPanel not found!");
        }

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            Debug.Log($"MainMenuManager: Canvas found, name: {canvas.name}");
            Button[] buttons = canvas.GetComponentsInChildren<Button>(true);
            Debug.Log($"MainMenuManager: Found {buttons.Length} buttons under Canvas");
            foreach (Button btn in buttons)
            {
                Debug.Log($"MainMenuManager: Checking button: {btn.name}, active: {btn.gameObject.activeSelf}, has Button: {btn != null}");
                if (btn.name.Contains("Play") && playButton == null)
                {
                    playButton = btn;
                    Debug.Log($"MainMenuManager: playButton found dynamically, name: {btn.name}");
                }
            }
        }
        else
        {
            Debug.LogError("MainMenuManager: Canvas not found in MainMenuScene!");
        }

        if (playButton != null)
        {
            playButton.gameObject.SetActive(true);
            Debug.Log($"MainMenuManager: playButton set active: {playButton.gameObject.activeSelf}");
            playButton.interactable = true;
            Debug.Log($"MainMenuManager: playButton set interactable: {playButton.interactable}");
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(StartGame);
            Debug.Log($"MainMenuManager: playButton listener set");
        }
        else
        {
            Debug.LogError("MainMenuManager: playButton not found!");
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
            Debug.Log("MainMenuManager: Created new EventSystem");
        }
        else
        {
            Debug.Log("MainMenuManager: EventSystem already exists, name: " + eventSystem.name);
        }
    }

    public void StartGame()
    {
        Debug.Log("MainMenuManager: StartGame called, loading MapScene");
        SceneManager.LoadScene("MapScene");
    }

    public void ExitGame()
    {
        Debug.Log("MainMenuManager: ExitGame called");
        Application.Quit();
    }
}