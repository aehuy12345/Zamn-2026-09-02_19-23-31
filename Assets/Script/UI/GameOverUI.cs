using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance { get; private set; }

    [Header("UI Components")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button cronoButton;     // Nút Hồi sinh
    [SerializeField] private Button quitButton;      // Nút Esc / Quit
    [SerializeField] private TextMeshProUGUI cronoCountText; // Hiển thị số lượt hồi sinh còn lại

    [Header("Settings")]
    [SerializeField] private int maxCronoCount = 2; // Tối đa 2 lần hồi sinh
    [SerializeField] private string mainMenuSceneName = "MainMenu"; // Tên Scene Menu chính

    private int currentCronoCount;
    private Transform startRoomSpawnPoint; // Vị trí Spawn ở Start Room
    private GameObject cachedPlayer;       // Biến lưu giữ Player

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        currentCronoCount = maxCronoCount;
    }

    private void Start()
    {
        // Gắn hàm cho nút bấm
        if (cronoButton != null) cronoButton.onClick.AddListener(OnCronoButtonClicked);
        if (quitButton != null) quitButton.onClick.AddListener(OnQuitButtonClicked);

        // Lưu trước tham chiếu Player ngay khi vào Game (khi Player còn sống/Active)
        FindPlayerReference();

        // Ban đầu ẩn bảng Game Over
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    private void FindPlayerReference()
    {
        if (cachedPlayer == null)
        {
            cachedPlayer = GameObject.FindGameObjectWithTag("Player");
        }
    }

    // Hàm hỗ trợ tìm vị trí StartRoom
    private Transform GetStartRoomSpawnPoint()
    {
        if (startRoomSpawnPoint != null) return startRoomSpawnPoint;

        RoomController[] rooms = FindObjectsByType<RoomController>();
        foreach (var room in rooms)
        {
            if (room.CurrentRoomType == RoomController.RoomType.StartRoom)
            {
                startRoomSpawnPoint = room.transform;
                return startRoomSpawnPoint;
            }
        }

        return null;
    }

    public void ShowGameOverUI()
    {
        // Đảm bảo vẫn giữ tham chiếu Player trước khi dừng Game
        FindPlayerReference();

        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        if (cronoCountText != null)
        {
            cronoCountText.text = $"Lượt hồi sinh Crono: {currentCronoCount}/{maxCronoCount}";
        }

        if (cronoButton != null)
        {
            cronoButton.interactable = currentCronoCount > 0;
        }

        Time.timeScale = 0f;
        gameObject.SetActive(true);
    
        // Phát nhạc/tiếng Game Over
        if (AudioManager.Instance != null && AudioManager.Instance.gameOverSound != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.gameOverSound);
        }
    }

    private void OnCronoButtonClicked()
    {
        if (currentCronoCount <= 0) return;

        // 1. Thử tìm lại Player nếu chưa tìm thấy trước đó (kể cả khi bị Disable)
        if (cachedPlayer == null)
        {
            FindPlayerReference();

            // Nếu vẫn không tìm thấy, quét tất cả Object ẩn trong Scene
            if (cachedPlayer == null)
            {
                CharacterStats[] allStats = Resources.FindObjectsOfTypeAll<CharacterStats>();
                foreach (var stat in allStats)
                {
                    if (stat.CompareTag("Player"))
                    {
                        cachedPlayer = stat.gameObject;
                        break;
                    }
                }
            }
        }

        // 2. Xử lý hồi sinh
        if (cachedPlayer != null)
        {
            currentCronoCount--; 
            Time.timeScale = 1f;

            if (gameOverPanel != null) gameOverPanel.SetActive(false);

            // Tọa độ Start Room
            Transform spawnPoint = GetStartRoomSpawnPoint();
            if (spawnPoint != null)
            {
                Vector3 targetPos = spawnPoint.position;
                targetPos.z = 0f;

                cachedPlayer.transform.position = targetPos;

                if (cachedPlayer.TryGetComponent<Rigidbody2D>(out var rb))
                {
                    rb.position = targetPos;
                    rb.linearVelocity = Vector2.zero; // Hoặc rb.velocity = Vector2.zero với Unity cũ
                }
            }

            // Gọi hàm khôi phục
            if (cachedPlayer.TryGetComponent<CharacterStats>(out var stats))
            {
                stats.RevivePlayer();
            }

            Debug.Log($"<color=green>[GameOverUI] Đã hồi sinh Player! Số lần còn lại: {currentCronoCount}</color>");
        }
        else
        {
            Debug.LogError("[GameOverUI] Không thể tìm thấy Player (Kể cả Object bị ẩn)! Kiểm tra lại Tag 'Player' trong Inspector.");
        }
    }

    private void OnQuitButtonClicked()
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }
}