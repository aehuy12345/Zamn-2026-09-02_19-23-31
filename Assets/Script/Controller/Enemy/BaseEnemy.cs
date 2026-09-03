using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CharacterStats))]
public class BaseEnemy : BaseCharacter
{
    [Header("Base Enemy References")]
    protected WeaponHandler weaponHandler;
    protected Transform playerTransform;

    protected override void Awake()
    {
        base.Awake();
        weaponHandler = GetComponent<WeaponHandler>();
    }

    protected virtual void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    // Hàm hỗ trợ xoay vũ khí về một vị trí chỉ định
    public virtual void AimAtTarget(Vector3 targetPosition)
    {
        if (weaponHandler != null)
        {
            Vector2 direction = (targetPosition - transform.position).normalized;
            weaponHandler.AimAtTargetOrDirection(direction);
        }
    }
}