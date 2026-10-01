using System.Collections;
using UnityEngine;

public class Combat : MonoBehaviour
{
    [SerializeField] private LayerMask opposerLayer;
    [SerializeField] private Transform mouthAttackPoint;

    [SerializeField] private float basicRange = 4f, basicDamage = 5f;
    [SerializeField] private float clawRange = 6f, clawDamage = 8f;
    [SerializeField] private float flameRadius = 10f, flameDamage = 16f;
    [SerializeField] private float flyRadius = 15f, flyDamage = 28f;

    [SerializeField] private float flameTickInterval = 0.25f;
    [SerializeField] private float flameConeAngle = 30f;

    private Coroutine flameRoutine;

    private Transform Origin => mouthAttackPoint != null ? mouthAttackPoint : transform;

    public void DealBasicAttackDamage()
    {
        SimpleAttackDamage(basicRange, basicDamage);
    }

    public void DealClawAttackDamage()
    {
        SimpleAttackDamage(clawRange, clawDamage);
    }

    public void DealFlameAttackDamage()
    {
        StopFlameChannel();
        flameRoutine = StartCoroutine(FlameChannelRoutine(flameRadius, flameDamage));
    }

    public void DealFlyAttackDamage()
    {
        StopFlameChannel();
        flameRoutine = StartCoroutine(FlameChannelRoutine(flyRadius, flyDamage));
    }

    private void SimpleAttackDamage(float range, float damage)
    {
        Vector3 originPos = Origin.position;
        Vector3 originForward = Origin.forward;
        Vector3 center = originPos + originForward * (range * 0.5f);

        foreach (var hit in Physics.OverlapSphere(center, range * 0.5f, opposerLayer))
        {
            hit.GetComponent<Health>()?.TakeDamage(damage);
        }
    }

    private IEnumerator FlameChannelRoutine(float radius, float damagePerSecond)
    {
        float tickDamage = damagePerSecond * flameTickInterval;
        var wait = new WaitForSeconds(flameTickInterval);
        float cosHalfAngle = Mathf.Cos(flameConeAngle * 0.5f * Mathf.Deg2Rad);

        while (true)
        {
            Vector3 originPos = Origin.position;
            Vector3 originForward = Origin.forward;

            foreach (var hit in Physics.OverlapSphere(originPos, radius, opposerLayer))
            {
                hit.GetComponent<Health>()?.TakeDamage(tickDamage);
            }
            yield return wait;
        }
    }

    public void StopFlameChannel()
    {
        if (flameRoutine != null)
        {
            StopCoroutine(flameRoutine);
            flameRoutine = null;
        }
    }

    private void OnDisable()
    {
        StopFlameChannel();
    }
}