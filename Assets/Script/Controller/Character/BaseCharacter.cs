using UnityEngine;

[RequireComponent(typeof(CharacterAnimationHandler))]
[RequireComponent(typeof(CharacterStats))]
public abstract class BaseCharacter : MonoBehaviour
{
    protected CharacterAnimationHandler animHandler;
    protected CharacterStats stats;

    // Lấy moveSpeed trực tiếp từ SO trong CharacterStats
    protected float moveSpeed => (stats != null && stats.StatsData != null) ? stats.StatsData.MoveSpeed : 5f;

    public CharacterStats Stats => stats;

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
        animHandler.PlayHit();
        stats.TakeDamage(damageAmount);
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}