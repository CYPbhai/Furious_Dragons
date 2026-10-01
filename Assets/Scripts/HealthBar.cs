using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthFillImage;

    private Health healthComponent;

    private void Awake()
    {
        healthComponent = GetComponentInParent<Health>();
        healthComponent.OnHealthChanged += HealthComponent_OnHealthChanged;
    }

    private void HealthComponent_OnHealthChanged(float currentHealth, float maxHealth)
    {
        healthFillImage.fillAmount = currentHealth / maxHealth;
        healthFillImage.color = Color.Lerp(Color.green, Color.red, 1 - (currentHealth / maxHealth));
    }
}
