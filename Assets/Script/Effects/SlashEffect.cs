using UnityEngine;

public class SlashEffect : MonoBehaviour
{
    // Hàm này sẽ được gọi tự động bởi Animation Event ở keyframe cuối
    public void DestroyEffect()
    {
        Destroy(gameObject);
    }
}