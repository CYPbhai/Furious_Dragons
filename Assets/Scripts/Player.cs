using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 720f;

    public event Action OnMove;
    public event Action OnIdle;
    public event Action OnBasicAttack;
    public event Action OnClawAttack;
    public event Action OnFlameAttack;
    public event Action OnFlyingFlameAttack;

    private PlayerInputProvider input;
    private Rigidbody rb;
    private bool isMoving = false;

    public bool IsAttacking { get; private set; }

    private void Awake()
    {
        input = GetComponent<PlayerInputProvider>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector2 move = input.MovementInput;
        Vector3 moveDir = new Vector3(move.x, 0f, move.y);

        bool wantsAttack1 = input.IsAttacking1;
        bool wantsAttack2 = input.IsAttacking2;
        bool wantsAttack3 = input.IsAttacking3;
        bool wantsAttack4 = input.IsAttacking4;

        if (!IsAttacking)
        {
            if (wantsAttack1) { StartAttack(); OnBasicAttack?.Invoke(); }
            else if (wantsAttack2) { StartAttack(); OnClawAttack?.Invoke(); }
            else if (wantsAttack3) { StartAttack(); OnFlameAttack?.Invoke(); }
            else if (wantsAttack4) { StartAttack(); OnFlyingFlameAttack?.Invoke(); }
        }

        bool nowMoving = !IsAttacking && moveDir.sqrMagnitude > 0.001f;

        if (nowMoving != isMoving)
        {
            isMoving = nowMoving;
            if (isMoving) OnMove?.Invoke();
            else OnIdle?.Invoke();
        }

        if (isMoving)
        {
            moveDir.Normalize();
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);

            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRot, rotateSpeed * Time.fixedDeltaTime));
        }
    }

    private void StartAttack()
    {
        IsAttacking = true;
    }

    public void OnAttackFinished()
    {
        IsAttacking = false;
    }

    public bool IsMoving() => isMoving;
}