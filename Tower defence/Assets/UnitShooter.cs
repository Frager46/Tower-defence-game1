using UnityEngine;
using System.Collections.Generic;

public class UnitShooter : MonoBehaviour
{
    public float attackRadius = 5f; // ������ �����
    public float attackCooldown = 1f; // ����������� �����
    public GameObject bulletPrefab; // ������ ����
    public float animationSpeed = 0.1f; // �������� �������� (������ �� ����)
    public float yOffset = 0.5f; // �������� ����� ����� �� ��� Y

    private Dictionary<string, Sprite[]> idleAnimations; // ������ �������� ��� ������� �����������
    private SpriteRenderer spriteRenderer;
    private string currentDirection = "Down"; // ������� �����������
    private int currentFrame = 0; // ������� ���� ��������
    private float animationTimer = 0f; // ������ ��� ����� ������
    private GameObject targetEnemy; // ������� ����-����
    private float attackTimer = 0f; // ������ �����������

    void Start()
    {
        // ��������� ������� SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer ����������� �� �����! �������� ��������� SpriteRenderer � ������� " + gameObject.name);
            return;
        }

        // ������� ����� �����
        transform.position += new Vector3(0, yOffset, 0);

        LoadAnimations();
        Debug.Log($"���� ������ � ������� {transform.position}");
    }

    void Update()
    {
        if (spriteRenderer == null) return; // ���� SpriteRenderer �����������, �� ��������� ����������

        FindTarget();
        UpdateDirection();
        Attack();
        UpdateAnimation();
    }

    void LoadAnimations()
    {
        idleAnimations = new Dictionary<string, Sprite[]>();

        // ��������� ������� ��� ������� �����������
        idleAnimations["Up"] = Resources.LoadAll<Sprite>("Sprites/Unit1/Idle/Up");
        idleAnimations["Down"] = Resources.LoadAll<Sprite>("Sprites/Unit1/Idle/Down");
        idleAnimations["Left"] = Resources.LoadAll<Sprite>("Sprites/Unit1/Idle/Left");
        idleAnimations["Right"] = Resources.LoadAll<Sprite>("Sprites/Unit1/Idle/Right");
        idleAnimations["UpRight"] = Resources.LoadAll<Sprite>("Sprites/Unit1/Idle/UpRight");
        idleAnimations["UpLeft"] = Resources.LoadAll<Sprite>("Sprites/Unit1/Idle/UpLeft");
        idleAnimations["DownRight"] = Resources.LoadAll<Sprite>("Sprites/Unit1/Idle/DownRight");
        idleAnimations["DownLeft"] = Resources.LoadAll<Sprite>("Sprites/Unit1/Idle/DownLeft");

        // ���������, ����������� �� �������
        foreach (var direction in idleAnimations.Keys)
        {
            if (idleAnimations[direction].Length == 0)
            {
                Debug.LogError($"�� ������� ��������� ������� ��� ����������� {direction}! ��������� ����: Resources/Sprites/Unit1/Idle/{direction}");
            }
        }

        // ��������� ��������� ������
        if (idleAnimations.ContainsKey("Down") && idleAnimations["Down"].Length > 0)
        {
            spriteRenderer.sprite = idleAnimations["Down"][0];
            Debug.Log("��������� ������ ����������: Down[0]");
        }
        else
        {
            Debug.LogError("�� ������� ���������� ��������� ������! ���������, ��� ����� Resources/Sprites/Unit1/Idle/Down �������� �������.");
        }
    }

    void FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRadius);
        float minDistance = float.MaxValue;
        targetEnemy = null;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    targetEnemy = hit.gameObject;
                }
            }
        }
    }

    void UpdateDirection()
    {
        if (targetEnemy != null)
        {
            Vector2 direction = (targetEnemy.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            currentDirection = GetClosestDirection(angle);
        }
        else
        {
            currentDirection = "Down"; // ������������ � ����������� �� ���������, ���� ������ ���
        }
    }

    string GetClosestDirection(float angle)
    {
        if (angle >= -22.5f && angle < 22.5f) return "Right";
        if (angle >= 22.5f && angle < 67.5f) return "UpRight";
        if (angle >= 67.5f && angle < 112.5f) return "Up";
        if (angle >= 112.5f && angle < 157.5f) return "UpLeft";
        if (angle >= 157.5f || angle < -157.5f) return "Left";
        if (angle >= -157.5f && angle < -112.5f) return "DownLeft";
        if (angle >= -112.5f && angle < -67.5f) return "Down";
        if (angle >= -67.5f && angle < -22.5f) return "DownRight";
        return "Down";
    }

    void Attack()
    {
        if (targetEnemy != null && attackTimer <= 0f)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.target = targetEnemy.transform;
            }
            else
            {
                Debug.LogError("Bullet �� �������� ��������� Bullet! ���������, ��� ������ Bullet �������� ���������.");
            }
            attackTimer = attackCooldown;
        }
        attackTimer -= Time.deltaTime;
    }

    void UpdateAnimation()
    {
        if (idleAnimations.ContainsKey(currentDirection) && idleAnimations[currentDirection].Length > 0)
        {
            animationTimer += Time.deltaTime;
            if (animationTimer >= animationSpeed)
            {
                animationTimer = 0f;
                currentFrame = (currentFrame + 1) % idleAnimations[currentDirection].Length;
                spriteRenderer.sprite = idleAnimations[currentDirection][currentFrame];
            }
        }
        else
        {
            Debug.LogWarning($"�� ������� ������������� �������� ��� ����������� {currentDirection}. ������� �� ���������.");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}