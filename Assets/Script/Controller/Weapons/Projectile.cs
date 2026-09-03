using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    private float damage;
    private float speed;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(float damageValue, float speedValue)
    {
        damage = damageValue;
        speed = speedValue;

        // Bắn đạn theo hướng xoay hiện tại
        rb.linearVelocity = transform.right * speed;

        // Tự hủy sau 3 giây để tránh rác bộ nhớ
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<BaseCharacter>(out var target))
        {
            target.TakeDamage(damage);
            Destroy(gameObject); // Tiêu hủy đạn khi va chạm
        }
    }
}