using UnityEngine;

public class LevelDungeonManager : MonoBehaviour
{
    public static LevelDungeonManager Instance { get; private set; }

    [Header("Dungeon Progress")]
    [SerializeField] private int totalCombatRooms = 4; // 4 phòng đánh quái
    [SerializeField] private GameObject teleportPortal; // Cổng dịch chuyển ở phòng Exit

    private int clearedRoomsCount = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (teleportPortal != null)
        {
            teleportPortal.SetActive(false); // Khóa cổng dịch chuyển ban đầu
        }
    }

    public void OnRoomCleared()
    {
        clearedRoomsCount++;
        Debug.Log($"Đã dọn sạch: {clearedRoomsCount}/{totalCombatRooms} phòng.");

        // Nếu đã dọn sạch tất cả các phòng quái -> Kích hoạt cổng chuyển Map
        if (clearedRoomsCount >= totalCombatRooms)
        {
            if (teleportPortal != null)
            {
                teleportPortal.SetActive(true);
                Debug.Log("Đã mở Cổng Dịch Chuyển sang Map mới!");
            }
        }
    }
}