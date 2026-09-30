using UnityEngine;

public class PlayerInputProvider : MonoBehaviour, IInputProvider
{
    public Vector2 MovementInput => GameInput.Instance.GetMovementVector();
    public bool IsAttacking1 => GameInput.Instance.GetIsAttacking1();
    public bool IsAttacking2 => GameInput.Instance.GetIsAttacking2();
    public bool IsAttacking3 => GameInput.Instance.GetIsAttacking3();
    public bool IsAttacking4 => GameInput.Instance.GetIsAttacking4();
}
