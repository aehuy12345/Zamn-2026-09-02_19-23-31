using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomDoorController : MonoBehaviour
{
    [Header("Tilemap References")]
    [SerializeField] private Tilemap doorTilemap;
    [SerializeField] private TilemapCollider2D doorCollider;

    [Header("Room Settings")]
    [Tooltip("Danh sách các Enemy trong phòng này")]
    [SerializeField] private List<BaseCharacter> roomEnemies = new List<BaseCharacter>();

    private bool isRoomActive = false;

    private void Awake()
    {
        // Tự lấy component nếu chưa gán trong Inspector
        if (doorTilemap == null) doorTilemap = GetComponent<Tilemap>();
        if (doorCollider == null) doorCollider = GetComponent<TilemapCollider2D>();

        // Mặc định ẩn và tắt va chạm cổng khi mới bắt đầu game
        CloseDoorSystem(false);
    }

    private void OnEnable()
    {
        // Đăng ký sự kiện lắng nghe khi quái chết
        foreach (var enemy in roomEnemies)
        {
            if (enemy != null && enemy.Stats != null)
            {
                enemy.Stats.OnDeath += CheckEnemiesStatus;
            }
        }
    }

    private void OnDisable()
    {
        // Hủy đăng ký sự kiện để tránh rác bộ nhớ
        foreach (var enemy in roomEnemies)
        {
            if (enemy != null && enemy.Stats != null)
            {
                enemy.Stats.OnDeath -= CheckEnemiesStatus;
            }
        }
    }

    // Hàm gọi khi Player bước vào phòng (Kích hoạt chiến đấu)
    public void StartRoomCombat()
    {
        if (isRoomActive) return;

        // Lọc lại danh sách các enemy còn sống
        roomEnemies.RemoveAll(enemy => enemy == null);

        // Nếu trong phòng còn quái -> Khóa cổng chặn người chơi
        if (roomEnemies.Count > 0)
        {
            isRoomActive = true;
            CloseDoorSystem(true);
        }
    }

    // Bật / Tắt cổng (Hình ảnh + Va chạm)
    private void CloseDoorSystem(bool isLocked)
    {
        if (doorTilemap != null)
        {
            var renderer = doorTilemap.GetComponent<TilemapRenderer>();
            if (renderer != null) renderer.enabled = isLocked; // Bật/tắt hiển thị Tilemap
        }

        if (doorCollider != null)
        {
            doorCollider.enabled = isLocked; // Bật/tắt va chạm vật lý
        }
    }

    // Kiểm tra số lượng quái còn lại mỗi khi có 1 con chết
    private void CheckEnemiesStatus()
    {
        if (!isRoomActive) return;

        // Lọc bỏ những enemy đã bị Destroy
        roomEnemies.RemoveAll(enemy => enemy == null);

        // Khi toàn bộ quái trong phòng đã chết
        if (roomEnemies.Count == 0)
        {
            isRoomActive = false;
            CloseDoorSystem(false); // Mở/Tắt cổng
            Debug.Log("Đã tiêu diệt toàn bộ quái! Cổng đã mở.");
        }
    }

    // Phát hiện Player bước vào vùng kích hoạt phòng (Trigger Zone)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isRoomActive)
        {
            StartRoomCombat();
        }
    }
}