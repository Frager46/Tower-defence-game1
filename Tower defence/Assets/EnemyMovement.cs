using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Rigidbody2D rb;

    public Transform[] path;
    private int pathIndex = 0;
    private Transform target;
    protected Health health;

    [SerializeField] private Animator animator; // Компонент Animator
    [SerializeField] private float moveSpeed = 2f; // Скорость движения
    [SerializeField] private bool isFlying = false; // Указывает, использует ли враг анимацию Fly

    private bool hasPlayedDeathAnimation = false; // Флаг для анимации смерти

    private LevelManager levelManager; // Ссылка на LevelManager

    private void Start()
    {
        health = GetComponent<Health>();
        levelManager = FindObjectOfType<LevelManager>(); // Находим LevelManager в сцене
        if (health == null)
        {
            Debug.LogError($"Компонент Health отсутствует на враге {gameObject.name}!");
        }
        if (levelManager == null)
        {
            Debug.LogError("LevelManager не найден в сцене!");
        }

        if (path != null && path.Length > 0)
        {
            target = path[pathIndex];
            Debug.Log($"EnemyMovement: Путь установлен для {gameObject.name}, точек в пути: {path.Length}, начальная точка: {target.position}");
        }
        else
        {
            Debug.LogError($"Путь path не задан для врага {gameObject.name}!");
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (animator != null)
        {
            animator.Play(isFlying ? "Fly" : "Walk");
            Debug.Log($"EnemyMovement: Анимация {(isFlying ? "Fly" : "Walk")} запущена для {gameObject.name}");
        }
    }

    private void Update()
    {
        if (target == null || health == null || health.IsDead())
        {
            if (health != null && health.IsDead())
            {
                Debug.Log($"EnemyMovement: {gameObject.name} мёртв, движение остановлено");
                if (animator != null && !hasPlayedDeathAnimation)
                {
                    animator.SetBool("isDead", true);
                    hasPlayedDeathAnimation = true;
                    Debug.Log($"EnemyMovement: Установлен параметр isDead = true для {gameObject.name}");
                }
            }
            if (target == null)
            {
                Debug.LogWarning($"EnemyMovement: {gameObject.name} не имеет цели для движения, pathIndex: {pathIndex}, path.Length: {path?.Length}");
            }
            return;
        }

        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        Debug.Log($"EnemyMovement: {gameObject.name} расстояние до цели {target.name}: {distanceToTarget}");

        if (distanceToTarget <= 0.1f)
        {
            pathIndex++;
            Debug.Log($"EnemyMovement: {gameObject.name} достиг точки {pathIndex - 1} в {target.position}");

            if (pathIndex >= path.Length)
            {
                Debug.Log($"EnemyMovement: {gameObject.name} достиг конца пути, pathIndex: {pathIndex}, path.Length: {path.Length}");
                if (levelManager != null)
                {
                    levelManager.NotifyEnemyReachedEnd(); // Уведомляем LevelManager
                    Debug.Log($"EnemyMovement: Уведомление LevelManager о достижении конца для {gameObject.name}");
                }
                Destroy(gameObject); // Уничтожаем врага
                return;
            }

            target = path[pathIndex];
            Debug.Log($"EnemyMovement: Новая цель для {gameObject.name}: {target.position}");
        }
    }

    private void FixedUpdate()
    {
        if (target == null || health == null || health.IsDead()) return;

        Vector2 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
        Debug.Log($"EnemyMovement: {gameObject.name} движется со скоростью {rb.velocity} к {target.position}");

        // Разворачиваем врага в сторону движения
        if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Разворот влево
        }
        else if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // Разворот вправо
        }

        // Обновляем анимацию
        if (animator != null)
        {
            animator.SetBool("isDead", health.IsDead());
        }
    }

    private void OnDestroy()
    {
        Debug.Log($"EnemyMovement: {gameObject.name} уничтожен");
    }
}