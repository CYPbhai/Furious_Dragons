using TMPro;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageText;
    private Health health;
    private Animator animator;
    private void Awake()
    {
        animator = GetComponentInParent<Animator>();
        health = GetComponentInParent<Health>();
    }
    private void OnEnable()
    {
        health.OnHit += Health_OnHit;
    }

    private void Health_OnHit(float amount)
    {
        damageText.text = "-" + amount.ToString();
        animator.SetTrigger("Damage");
    }

    private void OnDisable()
    {
        health.OnHit -= Health_OnHit;
    }
}
