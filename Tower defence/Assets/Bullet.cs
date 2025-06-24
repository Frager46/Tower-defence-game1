using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f;
    public float lifetime = 5f;
    public int damage = 1;

    [HideInInspector] public Transform target;

    void Start()
    {
        Destroy(gameObject, lifetime); // уничтожаем пулю через lifetime
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); // если враг исчез Ч уничтожаем пулю
            return;
        }

        // ƒвижение к цели
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // ќбращаемс€ к компоненту Health вместо EnemyMovement
            Health enemyHealth = collision.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            Destroy(gameObject); // уничтожаем пулю при попадании
        }
    }
}