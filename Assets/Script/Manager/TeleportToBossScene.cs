using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportToBossScene : MonoBehaviour
{
    [SerializeField] private string bossSceneName = "BossScene"; // Tên của Scene 3

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Lưu chỉ số Player
            if (other.TryGetComponent<CharacterStats>(out var stats))
            {
                if (PlayerPersistentData.Instance != null)
                {
                    PlayerPersistentData.Instance.SavePlayerData(stats);
                }
            }

            // 2. Chuyển sang Boss Scene
            SceneManager.LoadScene(bossSceneName);
        }
    }
}