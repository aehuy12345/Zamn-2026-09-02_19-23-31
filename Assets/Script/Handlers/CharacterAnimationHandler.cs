using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimationHandler : MonoBehaviour
{
    [SerializeField] private CharacterAnimationData animData;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayMove(bool isMoving)
    {
        if (animData == null) return;
        animator.SetBool(animData.IsRunningHash, isMoving);
    }

    public void PlayHit()
    {
        if (animData == null) return;
        animator.SetTrigger(animData.HitHash);
    }
}