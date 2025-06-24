using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button level1Button;
    public Button shopButton; // Добавлена кнопка магазина

    void Start()
    {
        Debug.Log("MapManager Start вызван в сцене: " + SceneManager.GetActiveScene().name);
        if (level1Button != null)
        {
            level1Button.onClick.AddListener(LoadLevel1);
            Debug.Log("Событие LoadLevel1 назначено на кнопку level1Button");
        }
        else
        {
            Debug.LogWarning("Кнопка первого уровня не назначена в MapManager!");
        }

        if (shopButton != null)
        {
            shopButton.onClick.AddListener(OpenShop);
            Debug.Log("Событие OpenShop назначено на кнопку shopButton");
        }
        else
        {
            Debug.LogWarning("Кнопка магазина не назначена в MapManager!");
        }
    }

    public void LoadLevel1()
    {
        Debug.Log("Метод LoadLevel1 вызван, пытаемся перейти на Level1Scene...");
        SceneManager.sceneLoaded += OnSceneLoaded; // Подписываемся на событие загрузки сцены
        SceneManager.LoadScene("Level1Scene");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level1Scene")
        {
            Debug.Log("Level1Scene полностью загружена");
            if (GameManager.Instance != null)
            {
                Debug.Log("GameManager найден, вызываем StartLevelGameplay...");
                GameManager.Instance.StartLevelGameplay();
            }
            else
            {
                Debug.LogError("GameManager не найден!");
            }
            SceneManager.sceneLoaded -= OnSceneLoaded; // Отписываемся после вызова
        }
    }

    private void OpenShop()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OpenShop();
            Debug.Log("Магазин открыт через MapManager");
        }
        else
        {
            Debug.LogError("GameManager не найден для открытия магазина!");
        }
    }
}