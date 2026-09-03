using UnityEngine;

[CreateAssetMenu(fileName = "CharacterAnimationData", menuName = "ScriptableObjects/AnimationData")]
public class CharacterAnimationData : ScriptableObject
{
    [Header("Parameter Names")]
    [SerializeField] private string isRunningParam = "IsRunning";
    [SerializeField] private string hitParam = "Hit";

    public int IsRunningHash { get; private set; }
    public int HitHash { get; private set; }

    // Khởi tạo Hash ID khi Load Asset
    private void OnEnable()
    {
        IsRunningHash = Animator.StringToHash(isRunningParam);
        HitHash = Animator.StringToHash(hitParam);
    }
}