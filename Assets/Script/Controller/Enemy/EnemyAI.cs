using UnityEngine;

public class EnemyAI : BaseEnemy
{
    [Header("Configurations")]
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private Weapon rangedWeapon; // Dùng riêng cho Boss nếu Boss dùng 2 vũ khí

    private float lastContactTime;

    private void Update()
    {
        if (playerTransform == null || enemyData == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;

        // Xoay vũ khí nhắm thẳng về phía Player
        if (weaponHandler != null)
        {
            weaponHandler.AimAtTargetOrDirection(directionToPlayer);
        }

        // Xử lý Logic AI theo từng loại Quái
        switch (enemyData.enemyType)
        {
            case EnemyType.ContactOnly:
                HandleContactEnemyLogic(distanceToPlayer);
                break;

            case EnemyType.Melee:
                HandleMeleeEnemyLogic(distanceToPlayer);
                break;

            case EnemyType.Ranged:
                HandleRangedEnemyLogic(distanceToPlayer);
                break;

            case EnemyType.Boss:
                HandleBossEnemyLogic(distanceToPlayer);
                break;
        }
    }

    #region AI Behaviors

    // 1. LOẠI SLIME (Chỉ bò đuổi và gây sát thương va chạm)
    private void HandleContactEnemyLogic(float distance)
    {
        if (distance <= enemyData.detectionRange)
        {
            MoveTowards(playerTransform.position);
            animHandler?.PlayMove(true);
        }
        else
        {
            animHandler?.PlayMove(false);
        }
    }

    // 2. LOẠI CẬN CHIẾN (Đuổi theo -> Đến tầm đánh thì dừng lại đánh)
    private void HandleMeleeEnemyLogic(float distance)
    {
        if (distance <= enemyData.attackRange)
        {
            animHandler?.PlayMove(false);
            if (weaponHandler != null) weaponHandler.TryAttack();
        }
        else if (distance <= enemyData.detectionRange)
        {
            MoveTowards(playerTransform.position);
            animHandler?.PlayMove(true);
        }
        else
        {
            animHandler?.PlayMove(false);
        }
    }

    // 3. LOẠI ĐÁNH XA (Áp sát thì chạy lùi, dừng chạy mới bắn)
    private void HandleRangedEnemyLogic(float distance)
    {
        if (distance < enemyData.fleeRange)
        {
            FleeFrom(playerTransform.position);
            animHandler?.PlayMove(true);
        }
        else if (distance <= enemyData.detectionRange)
        {
            animHandler?.PlayMove(false);
            if (weaponHandler != null) weaponHandler.TryAttack();
        }
        else
        {
            animHandler?.PlayMove(false);
        }
    }

    // 4. LOẠI BOSS (Thay đổi linh hoạt cận chiến & đánh xa)
    private void HandleBossEnemyLogic(float distance)
    {
        if (distance <= enemyData.attackRange)
        {
            animHandler?.PlayMove(false);
            if (weaponHandler != null) weaponHandler.TryAttack();
        }
        else if (distance <= enemyData.detectionRange)
        {
            if (rangedWeapon != null && weaponHandler != null)
            {
                rangedWeapon.TryAttack(stats, weaponHandler.AttackPoint);
            }
            MoveTowards(playerTransform.position);
            animHandler?.PlayMove(true);
        }
        else
        {
            animHandler?.PlayMove(false);
        }
    }

    #endregion

    #region Movement Helpers

    private void MoveTowards(Vector3 target)
    {
        float speed = 2f;
        if (stats != null && stats.StatsData != null)
        {
            speed = stats.StatsData.MoveSpeed;
        }

        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    private void FleeFrom(Vector3 target)
    {
        float speed = 2f;
        if (stats != null && stats.StatsData != null)
        {
            speed = stats.StatsData.MoveSpeed;
        }

        Vector3 fleeDirection = (transform.position - target).normalized;
        transform.position += fleeDirection * speed * Time.deltaTime;
    }

    #endregion

    #region Contact Damage (Slime / Boss)

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (enemyData == null || !enemyData.dealsContactDamage) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= lastContactTime + enemyData.contactCooldown)
            {
                if (collision.gameObject.TryGetComponent<BaseCharacter>(out var player))
                {
                    player.TakeDamage(enemyData.contactDamage);
                    lastContactTime = Time.time;
                }
            }
        }
    }

    #endregion
}