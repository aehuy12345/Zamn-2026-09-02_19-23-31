using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform attackPoint; // Vị trí ở đầu lưỡi kiếm
    [SerializeField] private Weapon currentWeapon;   // Vũ khí hiện tại

    public Weapon CurrentWeapon => currentWeapon;
    public Transform AttackPoint => attackPoint;

    // Nhắm hướng thanh kiếm (đầu kiếm) về phía vị trí chỉ định (ví dụ: Con trỏ chuột)
    public void AimAt(Vector3 targetPosition)
    {
        if (currentWeapon == null) return;

        Vector3 aimDirection = (targetPosition - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        // Xoay trực tiếp thanh kiếm hướng đầu kiếm về con trỏ chuột
        // Trừ 90 độ nếu Sprite thanh kiếm mặc định quay đứng hướng lên trên
        currentWeapon.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
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
}