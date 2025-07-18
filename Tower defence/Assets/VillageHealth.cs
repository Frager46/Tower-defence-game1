using UnityEngine;
using UnityEngine.UI;

public class VillageHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100; // Максимальное здоровье
    private int currentHealth;
    private bool isVillageDestroyed = false; // Флаг разрушения деревни

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

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) Debug.LogError("VillageHealth: SpriteRenderer не найден на Village!");
        if (healthBarImage == null) Debug.LogError("VillageHealth: HealthBarImage не найден!");
        if (fullHPSprites.Length == 0 || halfHPSprites.Length == 0 || zeroHPSprites.Length == 0)
            Debug.LogError("VillageHealth: Village sprite arrays are empty!");
        if (healthBarFullHP.Length == 0 || healthBarHalfHP.Length == 0 || healthBarZeroHP.Length == 0)
            Debug.LogError("VillageHealth: HealthBar sprite arrays are empty!");
    }

    void Start()
    {
        currentHealth = maxHealth;
        isVillageDestroyed = false;
        Debug.Log($"VillageHealth: Start called, initialized currentHealth={currentHealth}, isVillageDestroyed={isVillageDestroyed}");
        UpdateHealthBar();
        UpdateSprite();
    }

    // Установка индекса уровня для выбора правильного спрайта
    public void SetLevelIndex(int index)
    {
        levelIndex = Mathf.Clamp(index, 0, Mathf.Max(fullHPSprites.Length - 1, halfHPSprites.Length - 1, zeroHPSprites.Length - 1,
                                                    healthBarFullHP.Length - 1, healthBarHalfHP.Length - 1, healthBarZeroHP.Length - 1));
        Debug.Log($"VillageHealth: Set level index to {levelIndex}, currentHealth={currentHealth}");
        UpdateSprite();
        UpdateHealthBar();
    }

    // Получение урона
    public void TakeDamage(int damage)
    {
        if (damage < 0)
        {
            Debug.LogError($"VillageHealth: Attempted to apply negative damage: {damage}");
            return;
        }

        if (isVillageDestroyed)
        {
            Debug.Log($"VillageHealth: Damage ignored because village is already destroyed");
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - damage);
        Debug.Log($"VillageHealth: TakeDamage called with damage={damage}, currentHealth={currentHealth}");

        if (currentHealth <= 0)
        {
            isVillageDestroyed = true;
            Debug.Log($"VillageHealth: Village destroyed, currentHealth={currentHealth}");
        }

        UpdateHealthBar();
        UpdateSprite();
    }

    // Сброс здоровья до максимального
    public void ResetHealth()
    {
        if (isVillageDestroyed)
        {
            Debug.Log($"VillageHealth: ResetHealth skipped because village is destroyed");
            return;
        }

        currentHealth = maxHealth;
        Debug.Log($"VillageHealth: ResetHealth called, currentHealth={currentHealth}");
        UpdateHealthBar();
        UpdateSprite();
    }

    // Сброс флага разрушения
    public void ResetVillageDestroyed()
    {
        isVillageDestroyed = false;
        Debug.Log($"VillageHealth: isVillageDestroyed reset to false");
    }

    // Метод для получения текущего здоровья
    public int GetCurrentHealth()
    {
        Debug.Log($"VillageHealth: GetCurrentHealth called, returning {currentHealth}");
        return currentHealth;
    }

    // Проверка состояния разрушения
    public bool IsVillageDestroyed()
    {
        return isVillageDestroyed;
    }

    // Обновление healthbar
    void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            if (currentHealth > maxHealth / 2)
            {
                healthBarImage.sprite = healthBarFullHP.Length > levelIndex ? healthBarFullHP[levelIndex] : (healthBarFullHP.Length > 0 ? healthBarFullHP[0] : null);
                Debug.Log($"VillageHealth: Healthbar обновлён на FullHP, sprite={(healthBarImage.sprite != null ? healthBarImage.sprite.name : "null")}");
            }
            else if (currentHealth > 0)
            {
                healthBarImage.sprite = healthBarHalfHP.Length > levelIndex ? healthBarHalfHP[levelIndex] : (healthBarHalfHP.Length > 0 ? healthBarHalfHP[0] : null);
                Debug.Log($"VillageHealth: Healthbar обновлён на HalfHP, sprite={(healthBarImage.sprite != null ? healthBarImage.sprite.name : "null")}");
            }
            else
            {
                healthBarImage.sprite = healthBarZeroHP.Length > levelIndex ? healthBarZeroHP[levelIndex] : (healthBarZeroHP.Length > 0 ? healthBarZeroHP[0] : null);
                Debug.Log($"VillageHealth: Healthbar обновлён на ZeroHP, sprite={(healthBarImage.sprite != null ? healthBarImage.sprite.name : "null")}");
            }
            if (healthBarImage.sprite == null)
                Debug.LogError("VillageHealth: Healthbar sprite is null, check sprite arrays!");
        }
        else
        {
            Debug.LogError("VillageHealth: healthBarImage is null in UpdateHealthBar!");
        }
    }

    // Обновление спрайта Village
    void UpdateSprite()
    {
        if (spriteRenderer != null)
        {
            if (currentHealth > maxHealth / 2)
            {
                spriteRenderer.sprite = fullHPSprites.Length > levelIndex ? fullHPSprites[levelIndex] : (fullHPSprites.Length > 0 ? fullHPSprites[0] : null);
                Debug.Log($"VillageHealth: Village спрайт обновлён на FullHP, sprite={(spriteRenderer.sprite != null ? spriteRenderer.sprite.name : "null")}");
            }
            else if (currentHealth > 0)
            {
                spriteRenderer.sprite = halfHPSprites.Length > levelIndex ? halfHPSprites[levelIndex] : (halfHPSprites.Length > 0 ? halfHPSprites[0] : null);
                Debug.Log($"VillageHealth: Village спрайт обновлён на HalfHP, sprite={(spriteRenderer.sprite != null ? spriteRenderer.sprite.name : "null")}");
            }
            else
            {
                spriteRenderer.sprite = zeroHPSprites.Length > levelIndex ? zeroHPSprites[levelIndex] : (zeroHPSprites.Length > 0 ? zeroHPSprites[0] : null);
                Debug.Log($"VillageHealth: Village спрайт обновлён на ZeroHP, sprite={(spriteRenderer.sprite != null ? spriteRenderer.sprite.name : "null")}");
            }
            if (spriteRenderer.sprite == null)
                Debug.LogError("VillageHealth: Village sprite is null, check sprite arrays!");
        }
        else
        {
            Debug.LogError("VillageHealth: spriteRenderer is null in UpdateSprite!");
        }
    }
}