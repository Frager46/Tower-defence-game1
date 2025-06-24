using UnityEngine;
using UnityEngine.UI;

public class VillageHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100; // ������������ ��������
    private int currentHealth;

    [Header("Village Sprite Settings")]
    public Sprite fullHPSprite; // ������ ��� Village (FullHP)
    public Sprite halfHPSprite; // ������ ��� Village (HalfHP)
    public Sprite zeroHPSprite; // ������ ��� Village (0HP)

    [Header("Healthbar Sprite Settings")]
    public Sprite healthBarFullHP; // ������ healthbar ��� FullHP
    public Sprite healthBarHalfHP; // ������ healthbar ��� HalfHP
    public Sprite healthBarZeroHP; // ������ healthbar ��� 0HP

    [Header("UI References")]
    public Image healthBarImage; // ������ �� Image ��� healthbar

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) Debug.LogError("SpriteRenderer �� ������ �� Village!");
        if (healthBarImage == null) Debug.LogError("HealthBarImage �� ������!");
        UpdateHealthBar();
        UpdateSprite();
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
                healthBarImage.sprite = healthBarFullHP;
                Debug.Log("Healthbar ������� �� FullHP");
            }
            else if (currentHealth > 0)
            {
                healthBarImage.sprite = healthBarHalfHP;
                Debug.Log("Healthbar ������� �� HalfHP");
            }
            else
            {
                healthBarImage.sprite = healthBarZeroHP;
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
                spriteRenderer.sprite = fullHPSprite;
                Debug.Log("Village ������ ������� �� FullHP");
            }
            else if (currentHealth > 0)
            {
                spriteRenderer.sprite = halfHPSprite;
                Debug.Log("Village ������ ������� �� HalfHP");
            }
            else
            {
                spriteRenderer.sprite = zeroHPSprite;
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