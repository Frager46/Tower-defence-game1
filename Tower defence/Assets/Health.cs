using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    public int goldReward = 10; // Награда за убийство
    private int currentHealth;

    private bool isDying = false; // Флаг, чтобы избежать повторного вызова Die()

    private void Start()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Наносит урон объекту.
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (isDying) return; // Не наносить урон, если уже умирает

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDying) return; // Избегаем повторного вызова

        isDying = true;
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            gameManager.AddGold(goldReward);
        }
        Debug.Log($"{gameObject.name} уничтожен, награда: {goldReward} золота");

        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(0.6f); // Задержка в 1 секунду (настрой под длительность анимации Die)
        Destroy(gameObject);
    }

    /// <summary>
    /// Проверяет, мёртв ли объект.
    /// </summary>
    /// <returns>Возвращает true, если здоровье <= 0</returns>
    public bool IsDead()
    {
        return currentHealth <= 0;
    }
}