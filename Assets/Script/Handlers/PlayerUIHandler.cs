using UnityEngine;
using UnityEngine.UI;
using TMPro; // Nhập namespace này nếu dùng TextMeshPro

public class PlayerUIHandler : MonoBehaviour
{
    [Header("Stats Reference")]
    [SerializeField] private CharacterStats playerStats;

    [Header("UI Bars (Image Type: Filled)")]
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Image shieldBarFill;
    [SerializeField] private Image energyBarFill;

    [Header("UI Text (TMP)")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI shieldText;
    [SerializeField] private TextMeshProUGUI energyText;

    private void OnEnable()
    {
        if (playerStats != null)
        {
            playerStats.OnHealthChanged += UpdateHealthUI;
            playerStats.OnShieldChanged += UpdateShieldUI;
            playerStats.OnEnergyChanged += UpdateEnergyUI;
        }
    }

    private void OnDisable()
    {
        if (playerStats != null)
        {
            playerStats.OnHealthChanged -= UpdateHealthUI;
            playerStats.OnShieldChanged -= UpdateShieldUI;
            playerStats.OnEnergyChanged -= UpdateEnergyUI;
        }
    }

    private void UpdateHealthUI(float current, float max)
    {
        if (healthBarFill != null)
            healthBarFill.fillAmount = current / max;

        if (healthText != null)
            healthText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }

    private void UpdateShieldUI(float current, float max)
    {
        if (shieldBarFill != null)
            shieldBarFill.fillAmount = current / max;

        if (shieldText != null)
            shieldText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }

    private void UpdateEnergyUI(float current, float max)
    {
        if (energyBarFill != null)
            energyBarFill.fillAmount = current / max;

        if (energyText != null)
            energyText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }
}