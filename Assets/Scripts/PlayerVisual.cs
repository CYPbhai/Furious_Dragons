using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private Animator animator;
    private Player player;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        player = GetComponentInParent<Player>();

        player.OnMove += () => animator.SetTrigger("Move");
        player.OnIdle += () => animator.SetTrigger("Idle");
        player.OnBasicAttack += () => animator.SetTrigger("BasicAttack");
        player.OnClawAttack += () => animator.SetTrigger("ClawAttack");
        player.OnFlameAttack += () => animator.SetTrigger("FlameAttack");
        player.OnFlyingFlameAttack += () => animator.SetTrigger("FlyingFlameAttack");
    }

    public void OnAttackFinished()
    {
        player.OnAttackFinished();
        animator.ResetTrigger("Idle");
    }
}
