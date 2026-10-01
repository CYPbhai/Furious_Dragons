using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100;
    private float currentHealth;

    private float CurrentHealth 
    { 
        get
        {
            return currentHealth;
        }
        set
        {
            currentHealth = value;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        } 
    }

    public event Action<float, float> OnHealthChanged; // current, max

    public event Action OnDeath;

    private void OnEnable()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (CurrentHealth <= 0) return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);

        if(CurrentHealth <=0)
        {
            OnDeath?.Invoke();
        }
        else
        {
            GetComponent<IDamageable>()?.ReceiveHit();
        }
    }

}
