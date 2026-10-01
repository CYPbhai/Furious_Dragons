using UnityEngine;

public class DragonVisual : MonoBehaviour
{
    [Header("Win Event - Raise")]
    [SerializeField] private StringChannelEventSO OnWinSO;
    [SerializeField] private DragonColor opposerDragonColor;
    private Animator animator;
    private IDragon dragon;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        dragon = GetComponentInParent<IDragon>();

        dragon.OnMove += () => animator.SetTrigger("Move");
        dragon.OnIdle += () => animator.SetTrigger("Idle");
        dragon.OnBasicAttack += () => animator.SetTrigger("BasicAttack");
        dragon.OnClawAttack += () => animator.SetTrigger("ClawAttack");
        dragon.OnFlameAttack += () => animator.SetTrigger("FlameAttack");
        dragon.OnFlyingFlameAttack += () => animator.SetTrigger("FlyingFlameAttack");
        dragon.OnGetHit += () => animator.SetTrigger("GetHit");
        dragon.OnDeath += () => animator.SetTrigger("Die");
    }

    public void OnAttackFinished()
    {
        dragon.OnAttackFinished();
    }

    public void OnHitRecoveryFinished()
    {
        dragon.OnHitRecoveryFinished();
    }

    public void OnWin()
    {
        OnWinSO?.Raise(opposerDragonColor.ToString());
    }
}