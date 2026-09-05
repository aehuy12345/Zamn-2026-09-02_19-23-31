using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("BGM Clips")]
    [SerializeField] private AudioClip gameplayBGM;

    [Header("UI Sounds")]
    public SoundData uiClickSound;
    public SoundData gameOverSound;

    private void Awake()
    {
        // Xử lý Singleton đơn giản cho 1 Scene
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        // Tự động phát nhạc nền Gameplay khi Scene bắt đầu
        if (gameplayBGM != null && bgmSource != null)
        {
            bgmSource.clip = gameplayBGM;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    // Phát âm thanh UI / Game Over (âm thanh 2D)
    public void PlaySFX(SoundData soundData)
    {
        if (soundData != null && sfxSource != null)
        {
            soundData.Play(sfxSource);
        }
    }

    // Phát âm thanh tại vị trí nhân vật / quái (âm thanh 2D/3D)
    public void PlaySFXAtPosition(SoundData soundData, Vector3 position)
    {
        if (soundData == null || soundData.clips == null || soundData.clips.Length == 0) return;

        AudioClip clip = soundData.clips[Random.Range(0, soundData.clips.Length)];
        float pitch = Random.Range(soundData.pitchMin, soundData.pitchMax);

        GameObject tempGO = new GameObject("TempAudioSource");
        tempGO.transform.position = position;

        AudioSource tempSource = tempGO.AddComponent<AudioSource>();
        tempSource.clip = clip;
        tempSource.volume = soundData.volume;
        tempSource.pitch = pitch;
        tempSource.spatialBlend = 0.5f; // 0 = 2D, 1 = 3D (0.5 thích hợp cho Game Top-down)
        tempSource.Play();

        Destroy(tempGO, clip.length / pitch);
    }
}