using UnityEngine;
using UnityEngine.UI;

public class VillageHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100; // Максимальное здоровье
    private int currentHealth;

    [Header("Village Sprite Settings")]
    public Sprite[] fullHPSprites; // Массив спрайтов для Village (FullHP) для разных уровней
    public Sprite[] halfHPSprites; // Массив спрайтов для Village (HalfHP) для разных уровней
    public Sprite[] zeroHPSprites; // Массив спрайтов для Village (0HP) для разных уровней

    [Header("Healthbar Sprite Settings")]
    public Sprite[] healthBarFullHP; // Массив спрайтов healthbar для FullHP
    public Sprite[] healthBarHalfHP; // Массив спрайтов healthbar для HalfHP
    public Sprite[] healthBarZeroHP; // Массив спрайтов healthbar для 0HP

    [Header("UI References")]
    public Image healthBarImage; // Ссылка на Image для healthbar

    private SpriteRenderer spriteRenderer;
    private int levelIndex = 0; // Индекс уровня (0 для Level1, 1 для Level2 и т.д.)

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) Debug.LogError("SpriteRenderer не найден на Village!");
        if (healthBarImage == null) Debug.LogError("HealthBarImage не найден!");
        UpdateHealthBar();
        UpdateSprite();
    }

    // Установка индекса уровня для выбора правильного спрайта
    public void SetLevelIndex(int index)
    {
        levelIndex = Mathf.Clamp(index, 0, Mathf.Max(fullHPSprites.Length - 1, halfHPSprites.Length - 1, zeroHPSprites.Length - 1, 
                                                    healthBarFullHP.Length - 1, healthBarHalfHP.Length - 1, healthBarZeroHP.Length - 1));
        Debug.Log($"VillageHealth: Set level index to {levelIndex}");
        UpdateSprite();
        UpdateHealthBar();
    }

    // Получение урона
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth); // Не допускаем отрицательного здоровья
        Debug.Log($"Village получил урон, текущее здоровье: {currentHealth}");
        UpdateHealthBar();
        UpdateSprite();

        if (currentHealth <= 0)
        {
            Debug.Log("Village разрушен!");
        }
    }

    // Обновление healthbar
    void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            if (currentHealth > maxHealth / 2)
            {
                healthBarImage.sprite = healthBarFullHP.Length > levelIndex ? healthBarFullHP[levelIndex] : healthBarFullHP[0];
                Debug.Log("Healthbar обновлён на FullHP");
            }
            else if (currentHealth > 0)
            {
                healthBarImage.sprite = healthBarHalfHP.Length > levelIndex ? healthBarHalfHP[levelIndex] : healthBarHalfHP[0];
                Debug.Log("Healthbar обновлён на HalfHP");
            }
            else
            {
                healthBarImage.sprite = healthBarZeroHP.Length > levelIndex ? healthBarZeroHP[levelIndex] : healthBarZeroHP[0];
                Debug.Log("Healthbar обновлён на ZeroHP");
            }
        }
    }

    // Обновление спрайта Village
    void UpdateSprite()
    {
        if (spriteRenderer != null)
        {
            if (currentHealth > maxHealth / 2)
            {
                spriteRenderer.sprite = fullHPSprites.Length > levelIndex ? fullHPSprites[levelIndex] : fullHPSprites[0];
                Debug.Log("Village спрайт обновлён на FullHP");
            }
            else if (currentHealth > 0)
            {
                spriteRenderer.sprite = halfHPSprites.Length > levelIndex ? halfHPSprites[levelIndex] : halfHPSprites[0];
                Debug.Log("Village спрайт обновлён на HalfHP");
            }
            else
            {
                spriteRenderer.sprite = zeroHPSprites.Length > levelIndex ? zeroHPSprites[levelIndex] : zeroHPSprites[0];
                Debug.Log("Village спрайт обновлён на ZeroHP");
            }
        }
    }

    // Метод для получения текущего здоровья
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}