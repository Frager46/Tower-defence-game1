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

    [SerializeField] private Animator animator; // ��������� Animator
    [SerializeField] private float moveSpeed = 2f; // �������� ��������
    [SerializeField] private bool isFlying = false; // ���������, ���������� �� ���� �������� Fly

    private bool hasPlayedDeathAnimation = false; // ���� ��� �������� ������

    private LevelManager levelManager; // ������ �� LevelManager

    private void Start()
    {
        health = GetComponent<Health>();
        levelManager = FindObjectOfType<LevelManager>(); // ������� LevelManager � �����
        if (health == null)
        {
            Debug.LogError($"��������� Health ����������� �� ����� {gameObject.name}!");
        }
        if (levelManager == null)
        {
            Debug.LogError("LevelManager �� ������ � �����!");
        }

        if (path != null && path.Length > 0)
        {
            target = path[pathIndex];
            Debug.Log($"EnemyMovement: ���� ���������� ��� {gameObject.name}, ����� � ����: {path.Length}, ��������� �����: {target.position}");
        }
        else
        {
            Debug.LogError($"���� path �� ����� ��� ����� {gameObject.name}!");
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (animator != null)
        {
            animator.Play(isFlying ? "Fly" : "Walk");
            Debug.Log($"EnemyMovement: �������� {(isFlying ? "Fly" : "Walk")} �������� ��� {gameObject.name}");
        }
    }

    private void Update()
    {
        if (target == null || health == null || health.IsDead())
        {
            if (health != null && health.IsDead())
            {
                Debug.Log($"EnemyMovement: {gameObject.name} ����, �������� �����������");
                if (animator != null && !hasPlayedDeathAnimation)
                {
                    animator.SetBool("isDead", true);
                    hasPlayedDeathAnimation = true;
                    Debug.Log($"EnemyMovement: ���������� �������� isDead = true ��� {gameObject.name}");
                }
            }
            if (target == null)
            {
                Debug.LogWarning($"EnemyMovement: {gameObject.name} �� ����� ���� ��� ��������, pathIndex: {pathIndex}, path.Length: {path?.Length}");
            }
            return;
        }

        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        Debug.Log($"EnemyMovement: {gameObject.name} ���������� �� ���� {target.name}: {distanceToTarget}");

        if (distanceToTarget <= 0.1f)
        {
            pathIndex++;
            Debug.Log($"EnemyMovement: {gameObject.name} ������ ����� {pathIndex - 1} � {target.position}");

            if (pathIndex >= path.Length)
            {
                Debug.Log($"EnemyMovement: {gameObject.name} ������ ����� ����, pathIndex: {pathIndex}, path.Length: {path.Length}");
                if (levelManager != null)
                {
                    levelManager.NotifyEnemyReachedEnd(); // ���������� LevelManager
                    Debug.Log($"EnemyMovement: ����������� LevelManager � ���������� ����� ��� {gameObject.name}");
                }
                Destroy(gameObject); // ���������� �����
                return;
            }

            target = path[pathIndex];
            Debug.Log($"EnemyMovement: ����� ���� ��� {gameObject.name}: {target.position}");
        }
    }

    private void FixedUpdate()
    {
        if (target == null || health == null || health.IsDead()) return;

        Vector2 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
        Debug.Log($"EnemyMovement: {gameObject.name} �������� �� ��������� {rb.velocity} � {target.position}");

        // ������������� ����� � ������� ��������
        if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // �������� �����
        }
        else if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // �������� ������
        }

        // ��������� ��������
        if (animator != null)
        {
            animator.SetBool("isDead", health.IsDead());
        }
    }

    private void OnDestroy()
    {
        Debug.Log($"EnemyMovement: {gameObject.name} ���������");
    }
}