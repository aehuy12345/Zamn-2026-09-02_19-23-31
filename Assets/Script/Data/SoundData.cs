using UnityEngine;

[CreateAssetMenu(fileName = "NewSoundData", menuName = "Audio/Sound Data")]
public class SoundData : ScriptableObject
{
    [Header("Clips")]
    public AudioClip[] clips; // Danh sách các biến thể âm thanh

    [Header("Settings")]
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 2f)] public float pitchMin = 0.9f;
    [Range(0.1f, 2f)] public float pitchMax = 1.1f;

    public void Play(AudioSource source)
    {
        if (clips == null || clips.Length == 0 || source == null) return;

        // Chọn ngẫu nhiên 1 clip trong danh sách
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        
        source.pitch = Random.Range(pitchMin, pitchMax);
        source.PlayOneShot(clip, volume);
    }
}