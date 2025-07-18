using UnityEngine;
using UnityEngine.UI;

public class VillageHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100; // ������������ ��������
    private int currentHealth;
    private bool isVillageDestroyed = false; // ���� ���������� �������

    [Header("Village Sprite Settings")]
    public Sprite[] fullHPSprites; // ������ �������� ��� Village (FullHP) ��� ������ �������
    public Sprite[] halfHPSprites; // ������ �������� ��� Village (HalfHP) ��� ������ �������
    public Sprite[] zeroHPSprites; // ������ �������� ��� Village (0HP) ��� ������ �������

    [Header("Healthbar Sprite Settings")]
    public Sprite[] healthBarFullHP; // ������ �������� healthbar ��� FullHP
    public Sprite[] healthBarHalfHP; // ������ �������� healthbar ��� HalfHP
    public Sprite[] healthBarZeroHP; // ������ �������� healthbar ��� 0HP

    [Header("UI References")]
    public Image healthBarImage; // ������ �� Image ��� healthbar

    private SpriteRenderer spriteRenderer;
    private int levelIndex = 0; // ������ ������ (0 ��� Level1, 1 ��� Level2 � �.�.)

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) Debug.LogError("VillageHealth: SpriteRenderer �� ������ �� Village!");
        if (healthBarImage == null) Debug.LogError("VillageHealth: HealthBarImage �� ������!");
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

    // ��������� ������� ������ ��� ������ ����������� �������
    public void SetLevelIndex(int index)
    {
        levelIndex = Mathf.Clamp(index, 0, Mathf.Max(fullHPSprites.Length - 1, halfHPSprites.Length - 1, zeroHPSprites.Length - 1,
                                                    healthBarFullHP.Length - 1, healthBarHalfHP.Length - 1, healthBarZeroHP.Length - 1));
        Debug.Log($"VillageHealth: Set level index to {levelIndex}, currentHealth={currentHealth}");
        UpdateSprite();
        UpdateHealthBar();
    }

    // ��������� �����
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

    // ����� �������� �� �������������
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

    // ����� ����� ����������
    public void ResetVillageDestroyed()
    {
        isVillageDestroyed = false;
        Debug.Log($"VillageHealth: isVillageDestroyed reset to false");
    }

    // ����� ��� ��������� �������� ��������
    public int GetCurrentHealth()
    {
        Debug.Log($"VillageHealth: GetCurrentHealth called, returning {currentHealth}");
        return currentHealth;
    }

    // �������� ��������� ����������
    public bool IsVillageDestroyed()
    {
        return isVillageDestroyed;
    }

    // ���������� healthbar
    void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            if (currentHealth > maxHealth / 2)
            {
                healthBarImage.sprite = healthBarFullHP.Length > levelIndex ? healthBarFullHP[levelIndex] : (healthBarFullHP.Length > 0 ? healthBarFullHP[0] : null);
                Debug.Log($"VillageHealth: Healthbar ������� �� FullHP, sprite={(healthBarImage.sprite != null ? healthBarImage.sprite.name : "null")}");
            }
            else if (currentHealth > 0)
            {
                healthBarImage.sprite = healthBarHalfHP.Length > levelIndex ? healthBarHalfHP[levelIndex] : (healthBarHalfHP.Length > 0 ? healthBarHalfHP[0] : null);
                Debug.Log($"VillageHealth: Healthbar ������� �� HalfHP, sprite={(healthBarImage.sprite != null ? healthBarImage.sprite.name : "null")}");
            }
            else
            {
                healthBarImage.sprite = healthBarZeroHP.Length > levelIndex ? healthBarZeroHP[levelIndex] : (healthBarZeroHP.Length > 0 ? healthBarZeroHP[0] : null);
                Debug.Log($"VillageHealth: Healthbar ������� �� ZeroHP, sprite={(healthBarImage.sprite != null ? healthBarImage.sprite.name : "null")}");
            }
            if (healthBarImage.sprite == null)
                Debug.LogError("VillageHealth: Healthbar sprite is null, check sprite arrays!");
        }
        else
        {
            Debug.LogError("VillageHealth: healthBarImage is null in UpdateHealthBar!");
        }
    }

    // ���������� ������� Village
    void UpdateSprite()
    {
        if (spriteRenderer != null)
        {
            if (currentHealth > maxHealth / 2)
            {
                spriteRenderer.sprite = fullHPSprites.Length > levelIndex ? fullHPSprites[levelIndex] : (fullHPSprites.Length > 0 ? fullHPSprites[0] : null);
                Debug.Log($"VillageHealth: Village ������ ������� �� FullHP, sprite={(spriteRenderer.sprite != null ? spriteRenderer.sprite.name : "null")}");
            }
            else if (currentHealth > 0)
            {
                spriteRenderer.sprite = halfHPSprites.Length > levelIndex ? halfHPSprites[levelIndex] : (halfHPSprites.Length > 0 ? halfHPSprites[0] : null);
                Debug.Log($"VillageHealth: Village ������ ������� �� HalfHP, sprite={(spriteRenderer.sprite != null ? spriteRenderer.sprite.name : "null")}");
            }
            else
            {
                spriteRenderer.sprite = zeroHPSprites.Length > levelIndex ? zeroHPSprites[levelIndex] : (zeroHPSprites.Length > 0 ? zeroHPSprites[0] : null);
                Debug.Log($"VillageHealth: Village ������ ������� �� ZeroHP, sprite={(spriteRenderer.sprite != null ? spriteRenderer.sprite.name : "null")}");
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