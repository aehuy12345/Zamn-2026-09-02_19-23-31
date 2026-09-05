using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportPortal : MonoBehaviour
{
    [Header("Next Scene Settings")]
    [SerializeField] private string nextSceneName = "Level_2";
    [SerializeField] private bool requireInteractionKey = false;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem portalEffect;
    [SerializeField] private SpriteRenderer portalSprite;

    [Header("Rooms Monitoring")]
    [Tooltip("Nếu để trống, script sẽ tự động tìm TẤT CẢ RoomController trong Scene")]
    [SerializeField] private List<RoomController> allRooms = new List<RoomController>();

    private bool canTeleport = false;
    private bool isPortalOpened = false;

    private void Awake()
    {
        if (allRooms.Count == 0)
        {
            RoomController[] foundRooms = FindObjectsByType<RoomController>();
            allRooms.AddRange(foundRooms);
        }

        SetPortalActiveState(false);
    }

    private void Update()
    {
        if (!isPortalOpened)
        {
            CheckAllRoomsCleared();
        }

        if (canTeleport && requireInteractionKey && Input.GetKeyDown(KeyCode.E))
        {
            TeleportToNextLevel();
        }
    }

    private void CheckAllRoomsCleared()
    {
        int totalCombatRooms = 0;
        int clearedCombatRooms = 0;

        foreach (RoomController room in allRooms)
        {
            if (room == null) continue;

            // Chỉ đếm các phòng Combat (Bỏ qua StartRoom và ExitRoom)
            if (room.CurrentRoomType == RoomController.RoomType.CombatRoom)
            {
                totalCombatRooms++;

                // Điều kiện để 1 phòng được tính là HOÀN THÀNH THỰC SỰ:
                // Người chơi ĐÃ VÀO PHÒNG ĐÓ (HasBeenVisited) VÀ ĐÃ ĐÁNH XONG QUÁI (IsCleared)
                if (room.HasBeenVisited && room.IsCleared)
                {
                    clearedCombatRooms++;
                }
            }
        }

        // Nếu không có phòng combat nào trong Scene, hoặc ĐÃ ĐÁNH XONG TẤT CẢ CÁC PHÒNG COMBAT
        if (totalCombatRooms > 0 && clearedCombatRooms == totalCombatRooms)
        {
            OpenPortal();
        }
    }

    private void OpenPortal()
    {
        isPortalOpened = true;
        SetPortalActiveState(true);
        Debug.Log("<color=yellow>[TeleportPortal] TOÀN BỘ PHÒNG COMBAT ĐÃ CLEAR! CỔNG DỊCH CHUYỂN ĐÃ MỞ!</color>");

        if (portalEffect != null)
        {
            portalEffect.Play();
        }
    }

    private void SetPortalActiveState(bool active)
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = active;

        if (portalSprite != null) portalSprite.enabled = active;
        else
        {
            SpriteRenderer childSprite = GetComponentInChildren<SpriteRenderer>();
            if (childSprite != null) childSprite.enabled = active;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isPortalOpened)
        {
            if (!requireInteractionKey)
            {
                TeleportToNextLevel();
            }
            else
            {
                canTeleport = true;
                Debug.Log("Nhấn phím [E] để qua màn!");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canTeleport = false;
        }
    }

    private void TeleportToNextLevel()
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("[TeleportPortal] Chưa nhập tên 'Next Scene Name'!");
        }
    }
}