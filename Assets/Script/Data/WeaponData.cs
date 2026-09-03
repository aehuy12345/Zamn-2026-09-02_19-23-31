using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "ScriptableObjects/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("General Info")]
    [SerializeField] private string weaponName;
    [SerializeField] private WeaponType weaponType;
    [SerializeField] private Sprite weaponIcon;
    [SerializeField] private GameObject weaponPrefab;

    [Header("Base Stats")]
    [SerializeField] private float damage = 20f;
    [SerializeField] private float energyCost = 10f; // Năng lượng tiêu hao mỗi lần dùng

    [Header("Attack Rate / Cooldown")]
    [Tooltip("Khoảng thời gian chờ (giây) giữa 2 lần đánh (dành cho Kiếm/Búa)")]
    [SerializeField] private float attackCooldown = 0.5f;

    [Header("Ranged Settings (Cung / Trượng)")]
    [Tooltip("Số đạn bắn ra trong 1 giây (Fire Rate)")]
    [SerializeField] private float fireRate = 2f; 
    [Tooltip("Tốc độ bay của viên đạn")]
    [SerializeField] private float projectileSpeed = 15f;
    [Tooltip("Prefab của viên đạn hoặc hiệu ứng chưởng")]
    [SerializeField] private GameObject projectilePrefab;

    [Header("VFX / Effects")]
    [Tooltip("Hiệu ứng Visual Effect khi chém/đập hoặc tại vị trí trúng đòn")]
    [SerializeField] private GameObject attackEffectPrefab;

    // --- Properties để truy cập từ Code ---
    public string WeaponName => weaponName;
    public WeaponType WeaponType => weaponType;
    public Sprite WeaponIcon => weaponIcon;
    public GameObject WeaponPrefab => weaponPrefab;
    public float Damage => damage;
    public float EnergyCost => energyCost;
    
    // Nếu là Ranged thì Cooldown tính bằng 1 / FireRate, ngược lại dùng AttackCooldown
    public float Cooldown => (weaponType == WeaponType.Bow || weaponType == WeaponType.Staff) 
                             ? (fireRate > 0 ? 1f / fireRate : 0.5f) 
                             : attackCooldown;

    public float ProjectileSpeed => projectileSpeed;
    public GameObject ProjectilePrefab => projectilePrefab;
    public GameObject AttackEffectPrefab => attackEffectPrefab;
}