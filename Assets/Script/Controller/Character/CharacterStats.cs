using System;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Data Config")]
    [SerializeField] private CharacterStatsData statsData;
    [SerializeField] private bool isPlayer = false; // Đánh dấu nếu Script này gắn trên Player

    // Runtime variables
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
    public float CurrentShield => currentShield;
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

        // Nếu là Player và có dữ liệu đã lưu từ Scene trước (GamePlay Scene -> Boss Scene)
        if (isPlayer && PlayerPersistentData.Instance != null && PlayerPersistentData.Instance.HasSavedData())
        {
            currentHealth = PlayerPersistentData.Instance.SavedHealth;
            currentShield = PlayerPersistentData.Instance.SavedShield;
            currentEnergy = PlayerPersistentData.Instance.SavedEnergy;
            Debug.Log($"<color=green>[CharacterStats] Tải chỉ số lưu thành công: HP={currentHealth}</color>");
        }

        // Cập nhật lên UI
        OnHealthChanged?.Invoke(currentHealth, statsData.MaxHealth);
        OnShieldChanged?.Invoke(currentShield, statsData.MaxShield);
        OnEnergyChanged?.Invoke(currentEnergy, statsData.MaxEnergy);
    }

    private void Update()
    {
        HandleShieldRegen();
    }

    private void HandleShieldRegen()
    {

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

        // 2. Trừ Máu (Health)
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

    public void Heal(float healAmount)
    {
        if (statsData == null || currentHealth <= 0) return;

        currentHealth = Mathf.Min(currentHealth + healAmount, statsData.MaxHealth);
        OnHealthChanged?.Invoke(currentHealth, statsData.MaxHealth);
    }

    private void Die()
    {
        OnDeath?.Invoke(); 

        if (isPlayer)
        {
            RoomController[] rooms = FindObjectsByType<RoomController>();
            foreach (var room in rooms)
            {
                room.ResetRoomIfUncleared();
            }

            if (GameOverUI.Instance != null)
            {
                GameOverUI.Instance.ShowGameOverUI();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RevivePlayer()
    {
        if (statsData == null) return;

        gameObject.SetActive(true);

        currentHealth = statsData.MaxHealth * 0.5f; 
        currentShield = statsData.MaxShield;
        currentEnergy = statsData.MaxEnergy;

        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var s in sprites)
        {
            s.enabled = true;
        }

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>(true);
        foreach (var c in colliders)
        {
            c.enabled = true;
        }

        if (TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }

        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null)
        {
            anim.Rebind();
            anim.Update(0f);
            anim.Play("Idle");
        }

        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            script.enabled = true;
        }

        OnHealthChanged?.Invoke(currentHealth, statsData.MaxHealth);
        OnShieldChanged?.Invoke(currentShield, statsData.MaxShield);
        OnEnergyChanged?.Invoke(currentEnergy, statsData.MaxEnergy);

        Debug.Log("<color=green>[CharacterStats] Đã hồi sinh Player thành công!</color>");
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