using System;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Data Config")]
    [SerializeField] private CharacterStatsData statsData;

    // Runtime variables (Giá trị thay đổi trong lúc chơi)
    private float currentHealth;
    private float currentShield;
    private float currentEnergy;
    private float lastDamageTime;

    // Events cập nhật UI (Current, Max)
    public event Action<float, float> OnHealthChanged;
    public event Action<float, float> OnShieldChanged;
    public event Action<float, float> OnEnergyChanged;
    public event Action OnDeath;

    public CharacterStatsData StatsData => statsData;
    public float CurrentHealth => currentHealth;
    public float CurrentEnergy => currentEnergy;

    public float CurrentMoveSpeed 
    {
        get 
        {
            if (statsData != null) return statsData.MoveSpeed;
            return 3f;
        }
    }

    private void Awake()
    {
        InitializeStats();
    }

    public void InitializeStats()
    {
        if (statsData == null)
        {
            Debug.LogError($"[CharacterStats] Thiếu CharacterStatsData trên {gameObject.name}!");
            return;
        }

        currentHealth = statsData.MaxHealth;
        currentShield = statsData.MaxShield;
        currentEnergy = statsData.MaxEnergy;
    }

    private void Start()
    {
        if (statsData == null) return;

        // Cập nhật UI ban đầu
        OnHealthChanged?.Invoke(currentHealth, statsData.MaxHealth);
        OnShieldChanged?.Invoke(currentShield, statsData.MaxShield);
        OnEnergyChanged?.Invoke(currentEnergy, statsData.MaxEnergy);
    }

    private void Update()
    {
        HandleShieldRegen();
    }

    public void TakeDamage(float damageAmount)
    {
        if (damageAmount <= 0 || currentHealth <= 0) return;

        lastDamageTime = Time.time;

        // 1. Trừ Giáp (Shield) trước
        if (currentShield > 0)
        {
            if (currentShield >= damageAmount)
            {
                currentShield -= damageAmount;
                damageAmount = 0;
            }
            else
            {
                damageAmount -= currentShield;
                currentShield = 0;
            }
            OnShieldChanged?.Invoke(currentShield, statsData.MaxShield);
        }

        // 2. Trừ Máu (Health) nếu damage còn lại > 0
        if (damageAmount > 0)
        {
            currentHealth -= damageAmount;
            currentHealth = Mathf.Max(currentHealth, 0);
            OnHealthChanged?.Invoke(currentHealth, statsData.MaxHealth);

            // 3. Xử lý khi hết máu
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    // [BỔ SUNG] Hàm Hồi Máu cho Player/Enemy
    public void Heal(float healAmount)
    {
        if (statsData == null || currentHealth <= 0) return;

        currentHealth = Mathf.Min(currentHealth + healAmount, statsData.MaxHealth);
        OnHealthChanged?.Invoke(currentHealth, statsData.MaxHealth);
    }

    // [BỔ SUNG] Logic khi hết máu
    private void Die()
    {
        OnDeath?.Invoke(); // Báo tín hiệu về RoomController để đếm số quái còn lại
        
        // Tiêu diệt GameObject
        Destroy(gameObject);
    }

    private void HandleShieldRegen()
    {
        if (statsData == null) return;

        if (currentShield < statsData.MaxShield && Time.time >= lastDamageTime + statsData.ShieldRegenDelay)
        {
            currentShield += statsData.ShieldRegenRate * Time.deltaTime;
            currentShield = Mathf.Min(currentShield, statsData.MaxShield);
            OnShieldChanged?.Invoke(currentShield, statsData.MaxShield);
        }
    }

    public bool ConsumeEnergy(float amount)
    {
        if (currentEnergy >= amount)
        {
            currentEnergy -= amount;
            OnEnergyChanged?.Invoke(currentEnergy, statsData.MaxEnergy);
            return true;
        }
        return false;
    }

    public void RestoreEnergy(float amount)
    {
        if (statsData == null) return;

        currentEnergy = Mathf.Min(currentEnergy + amount, statsData.MaxEnergy);
        OnEnergyChanged?.Invoke(currentEnergy, statsData.MaxEnergy);
    }
}