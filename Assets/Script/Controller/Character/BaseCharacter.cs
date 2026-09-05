using UnityEngine;

[RequireComponent(typeof(CharacterAnimationHandler))]
[RequireComponent(typeof(CharacterStats))]
public abstract class BaseCharacter : MonoBehaviour
{
    protected CharacterAnimationHandler animHandler;
    protected CharacterStats stats;

    protected float moveSpeed => (stats != null && stats.StatsData != null) ? stats.StatsData.MoveSpeed : 5f;

    public CharacterStats Stats => stats;

    [Header("Audio Config")]
    [SerializeField] protected SoundData hitSound;
    [SerializeField] protected SoundData deathSound;

    protected virtual void Awake()
    {
        animHandler = GetComponent<CharacterAnimationHandler>();
        stats = GetComponent<CharacterStats>();
    }

    protected virtual void OnEnable()
    {
        if (stats != null)
        {
            stats.OnDeath += Die;
        }
    }

    protected virtual void OnDisable()
    {
        if (stats != null)
        {
            stats.OnDeath -= Die;
        }
    }

    public virtual void TakeDamage(float damageAmount)
    {
        if (animHandler != null) animHandler.PlayHit();
        if (hitSound != null)
        {
            AudioManager.Instance.PlaySFXAtPosition(hitSound, transform.position);
        }
        if (stats != null) stats.TakeDamage(damageAmount);
    }

    protected virtual void Die()
    {
        if (deathSound != null)
        {
            AudioManager.Instance.PlaySFXAtPosition(deathSound, transform.position);
        }
        // Kiểm tra xem đây có phải là Player không
        if (CompareTag("Player"))
        {
            // Tắt va chạm (Collider2D)
            Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
            foreach (var col in colliders) col.enabled = false;

            // Dừng chuyển động vật lý
            if (TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.linearVelocity = Vector2.zero;
                rb.simulated = false; // Tắt mô phỏng vật lý tạm thời
            }

            // Tắt bớt các Script điều khiển
            enabled = false;
        }
        else
        {
            // Nếu là Monster / Enemy thì mới xóa khỏi Game
            Destroy(gameObject);
        }
    }
}