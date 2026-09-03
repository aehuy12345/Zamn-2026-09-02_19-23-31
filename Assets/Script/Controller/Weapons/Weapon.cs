using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected WeaponData weaponData;
    protected float lastAttackTime;

    public WeaponData Data => weaponData;

    // Kiểm tra xem đã hết thời gian Cooldown chưa
    public bool CanAttack()
    {
        return Time.time >= lastAttackTime + weaponData.Cooldown;
    }

    // Hàm gọi khi thực hiện đòn đánh
    public virtual bool TryAttack(CharacterStats stats, Transform attackPoint)
    {
        if (!CanAttack()) return false;

        // Kiểm tra xem Player/Character có đủ năng lượng sử dụng vũ khí không
        if (stats != null && !stats.ConsumeEnergy(weaponData.EnergyCost))
        {
            Debug.Log("Không đủ năng lượng để sử dụng vũ khí!");
            return false;
        }

        lastAttackTime = Time.time;
        ExecuteAttack(attackPoint);
        return true;
    }

    protected abstract void ExecuteAttack(Transform attackPoint);
}