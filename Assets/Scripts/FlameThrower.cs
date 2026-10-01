using UnityEngine;
using UnityEngine.VFX;

public class FlameThrower : MonoBehaviour
{
    [SerializeField] private VisualEffect flameEffect;

    public void StartFlame()
    {
        flameEffect.Play();
    }
    public void StopFlame()
    {
        flameEffect.Stop();
    }
}
