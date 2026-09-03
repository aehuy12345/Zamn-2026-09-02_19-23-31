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
    public float CurrentEnergy => currentEnergy;

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
        if (damageAmount <= 0) return;

        lastDamageTime = Time.time;

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

        if (damageAmount > 0)
        {
            currentHealth -= damageAmount;
            currentHealth = Mathf.Max(currentHealth, 0);
            OnHealthChanged?.Invoke(currentHealth, statsData.MaxHealth);

            if (currentHealth <= 0)
            {
                OnDeath?.Invoke();
            }
        }
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