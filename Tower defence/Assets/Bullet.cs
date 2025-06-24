using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f;
    public float lifetime = 5f;
    public int damage = 1;

    [HideInInspector] public Transform target;

    void Start()
    {
        Destroy(gameObject, lifetime); // ���������� ���� ����� lifetime
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); // ���� ���� ����� � ���������� ����
            return;
        }

        // �������� � ����
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // ���������� � ���������� Health ������ EnemyMovement
            Health enemyHealth = collision.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            Destroy(gameObject); // ���������� ���� ��� ���������
        }
    }
}