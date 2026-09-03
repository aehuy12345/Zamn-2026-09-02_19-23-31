using UnityEngine;

[RequireComponent(typeof(WeaponHandler))]
public class BaseEnemy : BaseCharacter
{
    [Header("Enemy AI Settings")]
    [SerializeField] private float attackRange = 2f;
    
    protected WeaponHandler weaponHandler;
    protected Transform playerTransform;

    protected override void Awake()
    {
        base.Awake();
        weaponHandler = GetComponent<WeaponHandler>();
    }

    protected virtual void Start()
    {
        // Tìm Player trong Scene
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    protected virtual void Update()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // Nhắm về phía Player
        weaponHandler.AimAt(playerTransform.position);

        // Nếu Player đi vào tầm đánh -> Tiến hành tấn công
        if (distanceToPlayer <= attackRange)
        {
            weaponHandler.TryAttack();
        }
    }
}