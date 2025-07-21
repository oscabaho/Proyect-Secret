using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProyectSecret.Characters;
using ProyectSecret.Inventory;
using ProyectSecret.Events;

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
        private WeaponInstance subscribedWeaponInstance;

        // Delegados para guardar las suscripciones y poder desuscribirlas correctamente
        private System.Action<ProyectSecret.Stats.StatComponent> healthChangedHandler;
        private System.Action<ProyectSecret.Stats.StatComponent> staminaChangedHandler;
        private System.Action weaponStateChangedHandler;

        private void OnEnable()
        {
            // Suscribirse a eventos globales para saber cuándo aparece el jugador y cuándo cambia el inventario.
            GameEventBus.Instance.Subscribe<PlayerSpawnedEvent>(HandlePlayerSpawned);
            GameEventBus.Instance.Subscribe<InventoryChangedEvent>(OnInventoryChanged);
        }

        private void OnDisable()
        {
            if (GameEventBus.Instance != null)
            {
                GameEventBus.Instance.Unsubscribe<PlayerSpawnedEvent>(HandlePlayerSpawned);
                GameEventBus.Instance.Unsubscribe<InventoryChangedEvent>(OnInventoryChanged);
            }
            
            // Desuscribirse de todos los eventos del jugador si ya no estamos en la escena.
            UnsubscribeFromPlayerEvents();
        }

        private void HandlePlayerSpawned(PlayerSpawnedEvent evt)
        {
            // Si ya estábamos suscritos a un jugador anterior, nos desuscribimos primero.
            UnsubscribeFromPlayerEvents();

            // Obtenemos los componentes del nuevo jugador que ha aparecido.
            playerHealth = evt.PlayerObject.GetComponent<PlayerHealthController>();
            equipmentController = evt.PlayerObject.GetComponent<PlayerEquipmentController>();

            // Nos suscribimos a los eventos del nuevo jugador.
            SubscribeToPlayerEvents();
        }

        private void SubscribeToPlayerEvents()
        {
            // Vida
            if (playerHealth != null && playerHealth.Health != null)
            {
                healthChangedHandler = (stat) => UpdateHealthBar(stat.CurrentValue, stat.MaxValue);
                playerHealth.Health.OnValueChanged += healthChangedHandler;
                UpdateHealthBar(playerHealth.Health.CurrentValue, playerHealth.Health.MaxValue);
            }
            // Stamina
            if (playerHealth != null && playerHealth.Stamina != null)
            {
                staminaChangedHandler = (stat) => UpdateStaminaBar(stat.CurrentValue, stat.MaxValue);
                playerHealth.Stamina.OnValueChanged += staminaChangedHandler;
                UpdateStaminaBar(playerHealth.Stamina.CurrentValue, playerHealth.Stamina.MaxValue);
            }
            // Arma (se gestiona a través de OnInventoryChanged)
            OnInventoryChanged(null); // Llamada inicial para configurar el arma equipada al aparecer.
        }

        private void UnsubscribeFromPlayerEvents()
        {
            if (playerHealth != null)
            {
                if (playerHealth.Health != null && healthChangedHandler != null)
                    playerHealth.Health.OnValueChanged -= healthChangedHandler;
                if (playerHealth.Stamina != null && staminaChangedHandler != null)
                    playerHealth.Stamina.OnValueChanged -= staminaChangedHandler;
            }
            if (subscribedWeaponInstance != null)
            {
                subscribedWeaponInstance.OnStateChanged -= weaponStateChangedHandler;
                subscribedWeaponInstance = null;
            }
        }

        private void OnInventoryChanged(InventoryChangedEvent evt)
        {
            // Desuscribirse del arma anterior para evitar memory leaks.
            if (subscribedWeaponInstance != null)
            {
                subscribedWeaponInstance.OnStateChanged -= weaponStateChangedHandler;
            }

            // Suscribirse a la nueva arma equipada, si existe.
            if (equipmentController != null)
            {
                subscribedWeaponInstance = equipmentController.EquippedWeaponInstance;
                if (subscribedWeaponInstance != null)
                {
                    weaponStateChangedHandler = UpdateWeaponDurability;
                    subscribedWeaponInstance.OnStateChanged += weaponStateChangedHandler;
                }
            }
            
            // Actualizar la UI inmediatamente.
            UpdateWeaponDurability();
        }

        private void UpdateWeaponDurability()
        {
            if (subscribedWeaponInstance != null)
            {
                UpdateWeaponDurabilityRadial(
                    subscribedWeaponInstance.CurrentDurability, 
                    subscribedWeaponInstance.WeaponData.MaxDurability);
            }
            else if (weaponDurabilityRadial != null)
            {
                // Si no hay arma equipada, ocultamos la barra de durabilidad.
                weaponDurabilityRadial.fillAmount = 0;
            }
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
