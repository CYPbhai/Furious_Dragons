using System;

public interface IDragon
{
    event Action OnMove;
    event Action OnIdle;
    event Action OnBasicAttack;
    event Action OnClawAttack;
    event Action OnFlameAttack;
    event Action OnFlyingFlameAttack;
    event Action OnGetHit;
    event Action OnDeath;

    State state { get; }

    void ReceiveHit();
    void OnAttackFinished();
    void OnHitRecoveryFinished();
    void Die();
}