using System.Collections;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [Header("Melee Specifics")]
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Procedural Swing Settings")]
    [SerializeField] private float swingAngle = 90f;      // Góc chém
    [SerializeField] private float swingDuration = 0.12f; // Tốc độ chém

    private bool isSwinging = false;

    protected override void ExecuteAttack(Transform attackPoint)
    {
        // 1. Quét gây sát thương tại vị trí AttackPoint (đầu lưỡi kiếm)
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.TryGetComponent<BaseCharacter>(out var target))
            {
                target.TakeDamage(weaponData.Damage);
            }
        }

        // 2. Sinh hiệu ứng VFX tại đầu lưỡi kiếm
        if (weaponData.AttackEffectPrefab != null)
        {
            Instantiate(weaponData.AttackEffectPrefab, attackPoint.position, attackPoint.rotation);
        }

        // 3. Thực hiện vung kiếm bằng Code
        if (!isSwinging)
        {
            StartCoroutine(SwingRoutine());
        }
    }

    private IEnumerator SwingRoutine()
    {
        isSwinging = true;

        Quaternion baseRotation = transform.rotation;
        Quaternion startSwing = baseRotation * Quaternion.Euler(0, 0, swingAngle / 2f);
        Quaternion endSwing = baseRotation * Quaternion.Euler(0, 0, -swingAngle / 2f);

        float elapsed = 0f;
        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / swingDuration;
            transform.rotation = Quaternion.Slerp(startSwing, endSwing, t);
            yield return null;
        }

        transform.rotation = baseRotation;
        isSwinging = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}