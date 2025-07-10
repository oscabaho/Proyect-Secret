using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProyectSecret.Characters;
using ProyectSecret.Inventory;

namespace ProyectSecret.UI
{
    /// <summary>
    /// Controlador centralizado del HUD del jugador: vida, stamina, durabilidad de arma, quest e indicaciones.
    /// </summary>
    public class PlayerHUDController : MonoBehaviour
    {
        [Header("Stats UI")]
        [SerializeField] private Image healthFill;
        [SerializeField] private Image staminaFill;
        [SerializeField] private Image weaponDurabilityRadial;
        [SerializeField] private Color durabilityBaseColor = Color.green;
        [SerializeField] private Color durabilityWarningColor = Color.red;

        [Header("Quest & Guidance UI")]
        [SerializeField] private TMP_Text questInfoText;
        [SerializeField] private TMP_Text guidanceText;

        private PlayerHealthController playerHealth;
        private PlayerEquipmentController equipmentController;

        // Delegados para guardar las suscripciones y poder desuscribirlas correctamente
        private System.Action<ProyectSecret.Stats.StatComponent> healthChangedHandler;
        private System.Action<ProyectSecret.Stats.StatComponent> staminaChangedHandler;

        private void Start()
        {
            if (playerHealth == null)
                playerHealth = FindFirstObjectByType<PlayerHealthController>();
            if (equipmentController == null)
                equipmentController = FindFirstObjectByType<PlayerEquipmentController>();

            // Vida
            if (playerHealth != null && playerHealth.Health != null)
            {
                UpdateHealthBar(playerHealth.Health.CurrentValue, playerHealth.Health.MaxValue);
                healthChangedHandler = (stat) => UpdateHealthBar(stat.CurrentValue, stat.MaxValue);
                playerHealth.Health.OnValueChanged += healthChangedHandler;
            }
            // Stamina
            if (playerHealth != null && playerHealth.Stamina != null)
            {
                UpdateStaminaBar(playerHealth.Stamina.CurrentValue, playerHealth.Stamina.MaxValue);
                staminaChangedHandler = (stat) => UpdateStaminaBar(stat.CurrentValue, stat.MaxValue);
                playerHealth.Stamina.OnValueChanged += staminaChangedHandler;
            }
            // Durabilidad
            if (equipmentController != null && equipmentController.EquippedWeaponInstance != null)
            {
                UpdateWeaponDurabilityRadial(equipmentController.EquippedWeaponInstance.currentDurability, equipmentController.EquippedWeaponInstance.weaponData.MaxDurability);
            }
        }

        private void OnEnable()
        {
            // Aquí podrías suscribirte a eventos globales de quest/guidance si tienes un sistema de quests
        }

        private void OnDisable()
        {
            if (playerHealth != null && playerHealth.Health != null && healthChangedHandler != null)
                playerHealth.Health.OnValueChanged -= healthChangedHandler;
            if (playerHealth != null && playerHealth.Stamina != null && staminaChangedHandler != null)
                playerHealth.Stamina.OnValueChanged -= staminaChangedHandler;
        }

        private void UpdateHealthBar(float current, float max)
        {
            if (healthFill != null)
                healthFill.fillAmount = Mathf.Clamp01(current / max);
        }

        private void UpdateStaminaBar(float current, float max)
        {
            if (staminaFill != null)
                staminaFill.fillAmount = Mathf.Clamp01(current / max);
        }

        public void UpdateWeaponDurabilityRadial(float current, float max)
        {
            if (weaponDurabilityRadial == null) return;
            float percent = Mathf.Clamp01(current / max);
            weaponDurabilityRadial.fillAmount = percent;
            weaponDurabilityRadial.color = GetDurabilityColor(percent);
        }

        private Color GetDurabilityColor(float percent)
        {
            if (percent > 0.1f)
                return durabilityBaseColor;
            // Interpolación de color base a rojo intenso
            float t = Mathf.InverseLerp(0.1f, 0.01f, percent);
            return Color.Lerp(durabilityBaseColor, durabilityWarningColor, 1 - t);
        }

        public void SetQuestInfo(string questTitle, string questObjective)
        {
            if (questInfoText != null)
                questInfoText.text = $"<b>{questTitle}</b>\n{questObjective}";
        }

        public void SetGuidance(string message)
        {
            if (guidanceText != null)
                guidanceText.text = message;
        }
    }
}
