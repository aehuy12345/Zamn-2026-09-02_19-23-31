    using UnityEngine;

public class PlayerPersistentData : MonoBehaviour
{
    public static PlayerPersistentData Instance { get; private set; }

    // Các chỉ số cần giữ lại
    public float SavedHealth { get; set; } = -1f;
    public float SavedShield { get; set; } = -1f;
    public float SavedEnergy { get; set; } = -1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Hàm gọi trước khi Load Scene mới
    public void SavePlayerData(CharacterStats stats)
    {
        if (stats == null) return;
        SavedHealth = stats.CurrentHealth;
        SavedShield = stats.CurrentShield;
        SavedEnergy = stats.CurrentEnergy;
        Debug.Log($"<color=yellow>[DataPersistent] Đã lưu data Player: Health={SavedHealth}</color>");
    }

    // Hàm kiểm tra xem có dữ liệu đã lưu từ Scene trước không
    public bool HasSavedData()
    {
        return SavedHealth >= 0f;
    }
}