using UnityEngine;
using UnityEngine.UI;

public class VillageHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100; // ������������ ��������
    private int currentHealth;

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

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) Debug.LogError("SpriteRenderer �� ������ �� Village!");
        if (healthBarImage == null) Debug.LogError("HealthBarImage �� ������!");
        UpdateHealthBar();
        UpdateSprite();
    }

    // ��������� ������� ������ ��� ������ ����������� �������
    public void SetLevelIndex(int index)
    {
        levelIndex = Mathf.Clamp(index, 0, Mathf.Max(fullHPSprites.Length - 1, halfHPSprites.Length - 1, zeroHPSprites.Length - 1, 
                                                    healthBarFullHP.Length - 1, healthBarHalfHP.Length - 1, healthBarZeroHP.Length - 1));
        Debug.Log($"VillageHealth: Set level index to {levelIndex}");
        UpdateSprite();
        UpdateHealthBar();
    }

    // ��������� �����
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth); // �� ��������� �������������� ��������
        Debug.Log($"Village ������� ����, ������� ��������: {currentHealth}");
        UpdateHealthBar();
        UpdateSprite();

        if (currentHealth <= 0)
        {
            Debug.Log("Village ��������!");
        }
    }

    // ���������� healthbar
    void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            if (currentHealth > maxHealth / 2)
            {
                healthBarImage.sprite = healthBarFullHP.Length > levelIndex ? healthBarFullHP[levelIndex] : healthBarFullHP[0];
                Debug.Log("Healthbar ������� �� FullHP");
            }
            else if (currentHealth > 0)
            {
                healthBarImage.sprite = healthBarHalfHP.Length > levelIndex ? healthBarHalfHP[levelIndex] : healthBarHalfHP[0];
                Debug.Log("Healthbar ������� �� HalfHP");
            }
            else
            {
                healthBarImage.sprite = healthBarZeroHP.Length > levelIndex ? healthBarZeroHP[levelIndex] : healthBarZeroHP[0];
                Debug.Log("Healthbar ������� �� ZeroHP");
            }
        }
    }

    // ���������� ������� Village
    void UpdateSprite()
    {
        if (spriteRenderer != null)
        {
            if (currentHealth > maxHealth / 2)
            {
                spriteRenderer.sprite = fullHPSprites.Length > levelIndex ? fullHPSprites[levelIndex] : fullHPSprites[0];
                Debug.Log("Village ������ ������� �� FullHP");
            }
            else if (currentHealth > 0)
            {
                spriteRenderer.sprite = halfHPSprites.Length > levelIndex ? halfHPSprites[levelIndex] : halfHPSprites[0];
                Debug.Log("Village ������ ������� �� HalfHP");
            }
            else
            {
                spriteRenderer.sprite = zeroHPSprites.Length > levelIndex ? zeroHPSprites[levelIndex] : zeroHPSprites[0];
                Debug.Log("Village ������ ������� �� ZeroHP");
            }
        }
    }

    // ����� ��� ��������� �������� ��������
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}