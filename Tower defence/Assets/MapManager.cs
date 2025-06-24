using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button level1Button;
    public Button shopButton; // ��������� ������ ��������

    void Start()
    {
        Debug.Log("MapManager Start ������ � �����: " + SceneManager.GetActiveScene().name);
        if (level1Button != null)
        {
            level1Button.onClick.AddListener(LoadLevel1);
            Debug.Log("������� LoadLevel1 ��������� �� ������ level1Button");
        }
        else
        {
            Debug.LogWarning("������ ������� ������ �� ��������� � MapManager!");
        }

        if (shopButton != null)
        {
            shopButton.onClick.AddListener(OpenShop);
            Debug.Log("������� OpenShop ��������� �� ������ shopButton");
        }
        else
        {
            Debug.LogWarning("������ �������� �� ��������� � MapManager!");
        }
    }

    public void LoadLevel1()
    {
        Debug.Log("����� LoadLevel1 ������, �������� ������� �� Level1Scene...");
        SceneManager.sceneLoaded += OnSceneLoaded; // ������������� �� ������� �������� �����
        SceneManager.LoadScene("Level1Scene");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level1Scene")
        {
            Debug.Log("Level1Scene ��������� ���������");
            if (GameManager.Instance != null)
            {
                Debug.Log("GameManager ������, �������� StartLevelGameplay...");
                GameManager.Instance.StartLevelGameplay();
            }
            else
            {
                Debug.LogError("GameManager �� ������!");
            }
            SceneManager.sceneLoaded -= OnSceneLoaded; // ������������ ����� ������
        }
    }

    private void OpenShop()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OpenShop();
            Debug.Log("������� ������ ����� MapManager");
        }
        else
        {
            Debug.LogError("GameManager �� ������ ��� �������� ��������!");
        }
    }
}