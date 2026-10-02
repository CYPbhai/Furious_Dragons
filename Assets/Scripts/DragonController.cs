using System;
using UnityEngine;
public enum State { Idle, Moving, Attacking, Hit, Dead }
public enum DragonColor { Blue, Red }

[RequireComponent(typeof(Rigidbody))]
public class DragonController : MonoBehaviour, IDragon
{
    [Header("Movement & Combat")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 720f;
    [SerializeField] private float basicCooldown = 1f, clawCooldown = 2f, flameCooldown = 3f, flyCooldown = 5f;

    [Header("Targeting")]
    [SerializeField] public Transform lookTarget;

    private IInputProvider input;
    private Rigidbody rb;
    private Combat combat;
    private DragonController targetDragon;
    private float basicReadyAt, clawReadyAt, flameReadyAt, flyReadyAt;

    public State state { get; private set; } = State.Idle;

    public event Action OnMove;
    public event Action OnIdle;
    public event Action OnBasicAttack;
    public event Action OnClawAttack;
    public event Action OnFlameAttack;
    public event Action OnFlyingFlameAttack;
    public event Action OnGetHit;
    public event Action OnDeath;

    public bool IsBasicReady => Time.time >= basicReadyAt;
    public bool IsClawReady => Time.time >= clawReadyAt;
    public bool IsFlameReady => Time.time >= flameReadyAt;
    public bool IsFlyReady => Time.time >= flyReadyAt;

    public float BasicProgress => basicCooldown <= 0f ? 1f : Mathf.Clamp01(1f - ((basicReadyAt - Time.time) / basicCooldown));
    public float ClawProgress => clawCooldown <= 0f ? 1f : Mathf.Clamp01(1f - ((clawReadyAt - Time.time) / clawCooldown));
    public float FlameProgress => flameCooldown <= 0f ? 1f : Mathf.Clamp01(1f - ((flameReadyAt - Time.time) / flameCooldown));
    public float FlyProgress => flyCooldown <= 0f ? 1f : Mathf.Clamp01(1f - ((flyReadyAt - Time.time) / flyCooldown));

    private void Awake()
    {
        input = GetComponent<IInputProvider>();
        rb = GetComponent<Rigidbody>();
        combat = GetComponent<Combat>();
    }
    private void OnEnable()
    {
        GetComponent<Health>().OnDeath += DragonController_OnDeath;
        basicReadyAt = Time.time + basicCooldown;
        clawReadyAt = Time.time + clawCooldown;
        flameReadyAt = Time.time + flameCooldown;
        flyReadyAt = Time.time + flyCooldown;
    }
    private void OnDisable()
    {
        GetComponent<Health>().OnDeath -= DragonController_OnDeath;
    }
    private void Start()
    {
        UpdateTargetReference();
    }

    public void SetLookTarget(Transform target)
    {
        lookTarget = target;
        UpdateTargetReference();
    }

    private void UpdateTargetReference()
    {
        if (lookTarget != null)
        {
            targetDragon = lookTarget.GetComponent<DragonController>();
        }
        else
        {
            targetDragon = null;
        }
    }

    private void DragonController_OnDeath()
    {
        Die();
    }

    private void FixedUpdate()
    {
        if (state == State.Dead || targetDragon.state == State.Dead) return;

        bool att1 = input.IsAttacking1;
        bool att2 = input.IsAttacking2;
        bool att3 = input.IsAttacking3;
        bool att4 = input.IsAttacking4;

        Vector2 move = input.MovementInput;
        Vector3 moveDir = new Vector3(move.x, 0f, move.y);

        if (lookTarget != null && targetDragon == null)
        {
            UpdateTargetReference();
        }

        bool targetIsAttacking = targetDragon != null && targetDragon.state == State.Attacking;

        if ((state == State.Idle || state == State.Moving) && !targetIsAttacking)
        {
            if (Time.time >= basicReadyAt && att1)
            {
                state = State.Attacking;
                basicReadyAt = Time.time + basicCooldown;
                OnBasicAttack?.Invoke();
            }
            else if (Time.time >= clawReadyAt && att2)
            {
                state = State.Attacking;
                clawReadyAt = Time.time + clawCooldown;
                OnClawAttack?.Invoke();
            }
            else if (Time.time >= flameReadyAt && att3)
            {
                state = State.Attacking;
                flameReadyAt = Time.time + flameCooldown;
                OnFlameAttack?.Invoke();
            }
            else if (Time.time >= flyReadyAt && att4)
            {
                state = State.Attacking;
                flyReadyAt = Time.time + flyCooldown;
                OnFlyingFlameAttack?.Invoke();
            }
        }

        bool wantsToMove = moveDir.sqrMagnitude > 0.001f;
        if (state == State.Idle || state == State.Moving)
        {
            State targetState = wantsToMove ? State.Moving : State.Idle;
            if (targetState != state)
            {
                state = targetState;
                if (targetState == State.Moving) OnMove?.Invoke();
                else OnIdle?.Invoke();
            }
        }

        if (state == State.Moving)
        {
            moveDir.Normalize();
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
        }

        if (lookTarget != null)
        {
            Vector3 toTarget = lookTarget.position - rb.position;
            toTarget.y = 0f;
            if (toTarget.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
                rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRot, rotateSpeed * Time.fixedDeltaTime));
            }
        }
        else if (state == State.Moving && wantsToMove)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir.normalized, Vector3.up);
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRot, rotateSpeed * Time.fixedDeltaTime));
        }
    }

    public void OnAttackFinished()
    {
        combat?.StopFlameChannel();
        if (state != State.Dead)
        {
            state = State.Idle;
        }
    }

    public void ReceiveHit()
    {
        if (state == State.Dead) return;
        combat?.StopFlameChannel();
        state = State.Hit;
        OnGetHit?.Invoke();
        // Debug.Log("Recieved Hit");
    }

    public void OnHitRecoveryFinished()
    {
        if (state != State.Dead) state = State.Idle;
    }

    public void Die()
    {
        combat?.StopFlameChannel();
        state = State.Dead;
        OnDeath?.Invoke();
    }
}