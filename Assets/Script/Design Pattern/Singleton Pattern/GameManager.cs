using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public bool IsPaused { get; private set; }

    public void TogglePause()
    {
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f; // Dừng/tiếp tục thời gian trong game
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}