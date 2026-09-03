using UnityEngine;

public class RangedWeapon : Weapon
{
    protected override void ExecuteAttack(Transform attackPoint)
    {
        if (weaponData.ProjectilePrefab == null) return;

        // 1. Tạo viên đạn tại vị trí đầu nòng/tay cầm
        GameObject bulletObj = Instantiate(weaponData.ProjectilePrefab, attackPoint.position, attackPoint.rotation);
        
        // 2. Gán tốc độ và sát thương cho viên đạn (Giả định đạn có script Bullet)
        if (bulletObj.TryGetComponent<Projectile>(out var projectile))
        {
            projectile.Setup(weaponData.Damage, weaponData.ProjectileSpeed);
        }
    }
}