using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    public int goldReward = 10; // ������� �� ��������
    private int currentHealth;

    private bool isDying = false; // ����, ����� �������� ���������� ������ Die()

    // Event to notify when the enemy is destroyed
    public event System.Action<GameObject> OnEnemyDestroyed;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// ������� ���� �������.
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (isDying) return; // �� �������� ����, ���� ��� �������

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDying) return; // �������� ���������� ������

        isDying = true;
        GameState gameState = GameState.Instance;
        if (gameState != null)
        {
            gameState.AddGold(goldReward);
        }
        Debug.Log($"{gameObject.name} ���������, �������: {goldReward} ������");

        // Notify LevelManager before destruction
        OnEnemyDestroyed?.Invoke(gameObject);

        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(0.6f); // �������� � 1 ������� (������� ��� ������������ �������� Die)
        Destroy(gameObject);
    }

    /// <summary>
    /// ���������, ���� �� ������.
    /// </summary>
    /// <returns>���������� true, ���� �������� <= 0</returns>
    public bool IsDead()
    {
        return currentHealth <= 0;
    }
}