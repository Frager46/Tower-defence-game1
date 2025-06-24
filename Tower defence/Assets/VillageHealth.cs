using UnityEngine;
using UnityEngine.UI;

public class VillageHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100; // Максимальное здоровье
    private int currentHealth;

    [Header("Village Sprite Settings")]
    public Sprite fullHPSprite; // Спрайт для Village (FullHP)
    public Sprite halfHPSprite; // Спрайт для Village (HalfHP)
    public Sprite zeroHPSprite; // Спрайт для Village (0HP)

    [Header("Healthbar Sprite Settings")]
    public Sprite healthBarFullHP; // Спрайт healthbar для FullHP
    public Sprite healthBarHalfHP; // Спрайт healthbar для HalfHP
    public Sprite healthBarZeroHP; // Спрайт healthbar для 0HP

    [Header("UI References")]
    public Image healthBarImage; // Ссылка на Image для healthbar

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) Debug.LogError("SpriteRenderer не найден на Village!");
        if (healthBarImage == null) Debug.LogError("HealthBarImage не найден!");
        UpdateHealthBar();
        UpdateSprite();
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
                healthBarImage.sprite = healthBarFullHP;
                Debug.Log("Healthbar обновлён на FullHP");
            }
            else if (currentHealth > 0)
            {
                healthBarImage.sprite = healthBarHalfHP;
                Debug.Log("Healthbar обновлён на HalfHP");
            }
            else
            {
                healthBarImage.sprite = healthBarZeroHP;
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
                spriteRenderer.sprite = fullHPSprite;
                Debug.Log("Village спрайт обновлён на FullHP");
            }
            else if (currentHealth > 0)
            {
                spriteRenderer.sprite = halfHPSprite;
                Debug.Log("Village спрайт обновлён на HalfHP");
            }
            else
            {
                spriteRenderer.sprite = zeroHPSprite;
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