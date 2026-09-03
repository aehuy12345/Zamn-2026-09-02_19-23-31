using UnityEngine;

public enum EnemyType
{
    ContactOnly, // Dạng Slime (Chạm là mất máu)
    Melee,       // Cận chiến (Đuổi -> Dừng lại đánh)
    Ranged,      // Bắn xa (Áp sát thì chạy lùi, không vừa chạy vừa bắn)
    Boss         // Đa năng (Vừa chạm mất máu, vừa đổi đòn Melee/Ranged)
}

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "ScriptableObjects/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("General AI Settings")]
    public EnemyType enemyType = EnemyType.Melee;
    public float detectionRange = 7f;   // Khoảng cách phát hiện Player để đuổi
    public float attackRange = 1.5f;    // Khoảng cách để đứng lại tấn công

    [Header("Ranged / Kite Settings (Dành cho Ranged & Boss)")]
    public float fleeRange = 3f;        // Khoảng cách bị áp sát khiến quái phải lùi lại

    [Header("Contact Damage (Dành cho Slime & Boss)")]
    public bool dealsContactDamage = false; 
    public float contactDamage = 10f;
    public float contactCooldown = 1f;  // Thời gian chờ giữa 2 lần gây sát thương chạm
}