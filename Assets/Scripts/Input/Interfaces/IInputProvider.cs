using UnityEngine;
public interface IInputProvider
{
    public Vector2 MovementInput { get; }
    public bool IsAttacking1 { get; }
    public bool IsAttacking2 { get; }
    public bool IsAttacking3 { get; }
    public bool IsAttacking4 { get; }
}