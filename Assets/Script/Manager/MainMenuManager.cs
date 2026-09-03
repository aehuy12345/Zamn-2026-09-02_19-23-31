using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Tên của Scene chứa màn chơi chính (Ví dụ: MainGame hoặc GameScene)")]
    [SerializeField] private string gameSceneName = "MainGame";

    // Hàm gọi khi bấm Nút Play
    public void PlayGame()
    {
        // Đảm bảo thời gian game chạy bình thường khi vào màn chơi
        Time.timeScale = 1f;

        // Tải Scene màn chơi chính theo tên
        SceneManager.LoadScene(gameSceneName);
    }

    // Hàm gọi khi bấm Nút Quit
    public void QuitGame()
    {
        Debug.Log("Thoát Game!");

        // Thoát ứng dụng (Hoạt động khi build game ra file .exe/.apk/...)
        Application.Quit();

#if UNITY_EDITOR
        // Hỗ trợ dừng chế độ Play khi đang test trong Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}