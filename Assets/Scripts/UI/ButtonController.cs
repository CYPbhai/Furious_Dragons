using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private DragonController playerDragon;

    [Header("Skill UI Elements (0: Basic, 1: Claw, 2: Flame, 3: Fly)")]
    [SerializeField] private Button[] buttons = new Button[4];
    [SerializeField] private Image[] attackImages = new Image[4];

    private void OnEnable()
    {
        if (playerDragon != null)
        {
            playerDragon.OnBasicAttack += () => OnAttackCasted(0);
            playerDragon.OnClawAttack += () => OnAttackCasted(1);
            playerDragon.OnFlameAttack += () => OnAttackCasted(2);
            playerDragon.OnFlyingFlameAttack += () => OnAttackCasted(3);
        }
    }

    private void OnDisable()
    {
        if (playerDragon != null)
        {
            playerDragon.OnBasicAttack -= () => OnAttackCasted(0);
            playerDragon.OnClawAttack -= () => OnAttackCasted(1);
            playerDragon.OnFlameAttack -= () => OnAttackCasted(2);
            playerDragon.OnFlyingFlameAttack -= () => OnAttackCasted(3);
        }
    }

    private void Update()
    {
        if (playerDragon == null) return;

        UpdateSkillUI(0, playerDragon.BasicProgress);
        UpdateSkillUI(1, playerDragon.ClawProgress);
        UpdateSkillUI(2, playerDragon.FlameProgress);
        UpdateSkillUI(3, playerDragon.FlyProgress);
    }

    private void OnAttackCasted(int index)
    {
        if (index < 0 || index >= buttons.Length) return;

        if (buttons[index] != null)
        {
            buttons[index].interactable = false;
        }

        if (attackImages[index] != null)
        {
            attackImages[index].enabled = true;
            attackImages[index].fillAmount = 0f;
        }
    }

    private void UpdateSkillUI(int index, float progress)
    {
        if (index < 0 || index >= attackImages.Length) return;

        Image cooldownOverlay = attackImages[index];
        Button btn = buttons[index];

        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = progress;

            if (progress >= 1f)
            {
                if (btn != null && !btn.interactable)
                {
                    btn.interactable = true;
                }
            }
            else
            {
                if (btn != null && btn.interactable)
                {
                    btn.interactable = false;
                }
            }
        }
    }
}