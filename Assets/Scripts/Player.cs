using System;
using UnityEngine;
using UnityEngine.Playables;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 720f;

    public event Action OnMove;
    public event Action OnIdle;
    public event Action OnBasicAttack;
    public event Action OnClawAttack;
    public event Action OnFlameAttack;
    public event Action OnFlyingFlameAttack;
    public event Action OnGetHit;
    public event Action OnDeath;

    private PlayerInputProvider input;
    private Rigidbody rb;
    public State state { get; private set; } = State.Idle;
    [SerializeField] private float basicCooldown = 1f, clawCooldown = 2f, flameCooldown = 3f, flyCooldown = 5f;
    private float basicReadyAt, clawReadyAt, flameReadyAt, flyReadyAt;

    private void Awake()
    {
        input = GetComponent<PlayerInputProvider>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (state == State.Dead) return;
        Vector2 move = input.MovementInput;
        Vector3 moveDir = new Vector3(move.x, 0f, move.y);

        bool wantsAttack1 = input.IsAttacking1;
        bool wantsAttack2 = input.IsAttacking2;
        bool wantsAttack3 = input.IsAttacking3;
        bool wantsAttack4 = input.IsAttacking4;

        bool canAct = state == State.Idle || state == State.Moving;

        if (canAct)
        {
            if (wantsAttack1 && Time.time >= basicReadyAt) 
            { 
                StartAttack(State.Attacking); 
                basicReadyAt = Time.time + basicCooldown; 
                OnBasicAttack?.Invoke(); 
            }
            else if (wantsAttack2 && Time.time >= clawReadyAt) 
            { 
                StartAttack(State.Attacking); 
                clawReadyAt = Time.time + clawCooldown; 
                OnClawAttack?.Invoke(); 
            }
            else if (wantsAttack3 && Time.time >= flameReadyAt) 
            { 
                StartAttack(State.Attacking); 
                flameReadyAt = Time.time + flameCooldown; 
                OnFlameAttack?.Invoke(); 
            }
            else if (wantsAttack4 && Time.time >= flyReadyAt) 
            { 
                StartAttack(State.Attacking); 
                flyReadyAt = Time.time + flyCooldown; 
                OnFlyingFlameAttack?.Invoke(); 
            }
        }

        bool wantsToMove = moveDir.sqrMagnitude > 0.001f;

        if (state == State.Idle || state == State.Moving)
        {
            State target = wantsToMove ? State.Moving : State.Idle;
            if (target != state)
            {
                state = target;
                if (target == State.Moving)
                {
                    OnMove?.Invoke();
                }
                else
                {
                    OnIdle?.Invoke();
                }
            }
        }

        if (state == State.Moving)
        {
            moveDir.Normalize();
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRot, rotateSpeed * Time.fixedDeltaTime));
        }
    }

    private void StartAttack(State s)
    {
        state = s;
    }
    public void OnAttackFinished()
    {
        if (state != State.Dead) state = State.Idle;
    }
    
    public void ReceiveHit()
    {
        if (state == State.Dead) return;
        state = State.Hit;
        OnGetHit?.Invoke();
    }

    public void OnHitRecoveryFinished()
    {
        if (state != State.Dead)
        {
            state = State.Idle;
        }
    }

    public void Die()
    {
        state = State.Dead;
        OnDeath?.Invoke();
    }
}