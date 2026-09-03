using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Weapon currentWeapon;

    [Header("Auto Tracking Settings")]
    [SerializeField] private bool enableAutoTargeting = true;
    [SerializeField] private float autoTargetRadius = 5f; // Tầm tìm mục tiêu gần nhất
    [SerializeField] private LayerMask targetLayer;      // Player thì chọn Layer 'Enemy', Enemy thì chọn Layer 'Player'

    public Weapon CurrentWeapon => currentWeapon;
    public Transform AttackPoint => attackPoint;

    private Vector2 lastDirection = Vector2.right;

    // Hàm nâng cấp: Track theo Target gần nhất -> Nếu không có thì quay về Track theo hướng di chuyển
    public void AimAtTargetOrDirection(Vector2 moveInput)
    {
        if (currentWeapon == null) return;

        Vector2 aimDirection = Vector2.zero;

        // 1. ƯU TIÊN: Tìm mục tiêu (Player/Enemy) gần nhất trong vùng autoTargetRadius
        if (enableAutoTargeting)
        {
            Transform closestTarget = GetClosestTarget();
            if (closestTarget != null)
            {
                aimDirection = (closestTarget.position - transform.position).normalized;
            }
        }

        // 2. DỰ PHÒNG: Nếu không có mục tiêu gần đó, quay về track theo hướng di chuyển
        if (aimDirection == Vector2.zero)
        {
            if (moveInput.sqrMagnitude > 0.01f)
            {
                lastDirection = moveInput.normalized;
            }
            aimDirection = lastDirection;
        }

        // 3. Thực hiện xoay vũ khí
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        currentWeapon.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    // Hàm tìm đối tượng thuộc targetLayer nằm gần vị trí nhân vật nhất
    private Transform GetClosestTarget()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, autoTargetRadius, targetLayer);
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider2D col in targets)
        {
            float distance = Vector2.Distance(transform.position, col.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = col.transform;
            }
        }

        return closest;
    }

    public bool TryAttack()
    {
        if (currentWeapon == null || attackPoint == null) return false;
        return currentWeapon.TryAttack(GetComponent<CharacterStats>(), attackPoint);
    }

    public void EquipWeapon(Weapon newWeapon)
    {
        currentWeapon = newWeapon;
    }

    private void OnDrawGizmosSelected()
    {
        // Hiển thị vòng tròn tầm Auto Target trong cửa sổ Scene
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, autoTargetRadius);
    }
}