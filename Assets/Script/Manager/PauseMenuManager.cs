using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI Panels & Buttons")]
    [SerializeField] private GameObject pauseMenuPanel; // Canvas Group / Panel chứa 3 nút Home, Resume, Setting
    [SerializeField] private GameObject pauseButton;    // Nút Pause góc bên phải màn hình

    private bool isPaused = false;

    public bool IsPaused => isPaused;

    private void Start()
    {
        // Mặc định ban đầu: Game chạy bình thường, Menu Pause ẩn, Nút Pause hiện
        ResumeGame();
    }

    private void Update()
    {
        // Hỗ trợ bấm phím ESC trên bàn phím để Pause/Resume nhanh
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // Hàm gọi khi bấm vào Nút Pause (Góc màn hình)
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Đóng băng toàn bộ logic thời gian (Physics, Cooldown, Coroutine dùng WaitForSeconds)

        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        if (pauseButton != null) pauseButton.SetActive(false); // Ẩn nút Pause khi đang bật Menu
    }

    // Hàm LOGIC CHÍNH CHO NÚT RESUME
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Tiếp tục thời gian game bình thường

        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (pauseButton != null) pauseButton.SetActive(true); // Hiện lại nút Pause
    }

    // Hàm khung cho nút Home (Sẽ làm logic chuyển Scene sau)
    public void OnHomeButtonClicked()
    {
        Time.timeScale = 1f; // Luôn unfreeze thời gian trước khi chuyển Scene
        Debug.Log("Chuyển về trang Home / Main Menu...");
        SceneManager.LoadScene("MainMenu");
    }

    // Hàm khung cho nút Setting (Sẽ làm logic mở Bảng Cài đặt sau)
    public void OnSettingButtonClicked()
    {
        Debug.Log("Mở bảng Cài đặt Settings...");
    }
}   