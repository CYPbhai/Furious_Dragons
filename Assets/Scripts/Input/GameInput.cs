using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    private InputSystem_Actions inputSystemActions;

    private bool attack1Queued;
    private bool attack2Queued;
    private bool attack3Queued;
    private bool attack4Queued;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        inputSystemActions = new InputSystem_Actions();
        inputSystemActions.Player.Enable();

        inputSystemActions.Player.Attack1.performed += ctx => attack1Queued = true;
        inputSystemActions.Player.Attack2.performed += ctx => attack2Queued = true;
        inputSystemActions.Player.Attack3.performed += ctx => attack3Queued = true;
        inputSystemActions.Player.Attack4.performed += ctx => attack4Queued = true;
    }

    public Vector2 GetMovementVector()
    {
        return inputSystemActions.Player.Move.ReadValue<Vector2>();
    }

    public bool GetIsAttacking1()
    {
        if (!attack1Queued) return false;
        attack1Queued = false;
        return true;
    }

    public bool GetIsAttacking2()
    {
        if (!attack2Queued) return false;
        attack2Queued = false;
        return true;
    }

    public bool GetIsAttacking3()
    {
        if (!attack3Queued) return false;
        attack3Queued = false;
        return true;
    }

    public bool GetIsAttacking4()
    {
        if (!attack4Queued) return false;
        attack4Queued = false;
        return true;
    }

    private void OnDestroy()
    {
        inputSystemActions.Player.Disable();
    }
}