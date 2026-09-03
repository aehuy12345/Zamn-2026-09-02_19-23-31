using UnityEngine;

[RequireComponent(typeof(CharacterAnimationHandler))]
public class EnemyAI : BaseCharacter
{
    [Header("Configurations")]
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private Weapon rangedWeapon;

    private WeaponHandler weaponHandler;
    private Transform playerTransform;
    private float lastContactTime;

    protected override void Awake()
    {
        base.Awake(); // Đã tự động lấy animHandler từ BaseCharacter
        weaponHandler = GetComponent<WeaponHandler>();
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
    }

    private void Update()
    {
        if (playerTransform == null || enemyData == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // Xoay hướng nếu có WeaponHandler
        if (weaponHandler != null)
        {
            weaponHandler.AimAt(playerTransform.position);
        }

        // Xử lý Logic theo loại Quái
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

    // 1. LOẠI SLIME (Chạy đuổi -> Bật/Tắt Animation Move)
    private void HandleContactEnemyLogic(float distance)
    {
        if (distance <= enemyData.detectionRange)
        {
            MoveTowards(playerTransform.position);
            animHandler?.PlayMove(true); // Bật animation Chạy
        }
        else
        {
            animHandler?.PlayMove(false); // Trả về Idle khi ngoài tầm
        }
    }

    // 2. LOẠI CẬN CHIẾN
    private void HandleMeleeEnemyLogic(float distance)
    {
        if (distance <= enemyData.attackRange)
        {
            animHandler?.PlayMove(false); // Dừng lại để đánh
            weaponHandler.TryAttack();
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

    // 3. LOẠI ĐÁNH XA
    private void HandleRangedEnemyLogic(float distance)
    {
        if (distance < enemyData.fleeRange)
        {
            FleeFrom(playerTransform.position);
            animHandler?.PlayMove(true);
        }
        else if (distance <= enemyData.detectionRange)
        {
            animHandler?.PlayMove(false); // Đứng yên bắn
            weaponHandler.TryAttack();
        }
        else
        {
            animHandler?.PlayMove(false);
        }
    }

    // 4. LOẠI BOSS
    private void HandleBossEnemyLogic(float distance)
    {
        if (distance <= enemyData.attackRange)
        {
            animHandler?.PlayMove(false);
            weaponHandler.TryAttack();
        }
        else if (distance <= enemyData.detectionRange)
        {
            if (rangedWeapon != null)
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
        transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    private void FleeFrom(Vector3 target)
    {
        Vector3 fleeDirection = (transform.position - target).normalized;
        transform.position += fleeDirection * moveSpeed * Time.deltaTime;
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