using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "ScriptableObjects/Character Stats Data")]
public class CharacterStatsData : ScriptableObject
{
    [Header("Base Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float maxShield = 50f;
    [SerializeField] private float maxEnergy = 100f;

    [Header("Shield Regen Settings")]
    [SerializeField] private float shieldRegenDelay = 3f; // Thời gian chờ trước khi hồi giáp (s)
    [SerializeField] private float shieldRegenRate = 10f; // Tốc độ hồi giáp/giây

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    // Properties để các Script khác đọc dữ liệu
    public float MaxHealth => maxHealth;
    public float MaxShield => maxShield;
    public float MaxEnergy => maxEnergy;
    public float ShieldRegenDelay => shieldRegenDelay;
    public float ShieldRegenRate => shieldRegenRate;
    public float MoveSpeed => moveSpeed;
}