using UnityEngine;
using ProyectSecret.Components;
using ProyectSecret.Interfaces;

using ProyectSecret.Inventory.Items;
using ProyectSecret.Inventory;

namespace ProyectSecret.Combat.Behaviours
{
    /// <summary>
    /// Componente modular para lógica de ataque. Puede ser añadido a cualquier entidad.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class AttackComponent : MonoBehaviour
    {
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private int staminaCost = 10;
        [SerializeField] private StaminaComponentBehaviour staminaBehaviour;
        [SerializeField] private PlayerEquipmentController equipmentController;
        private float lastAttackTime = -999f;
        [SerializeField]private WeaponItem weapon;

        private void Awake()
        {
            if (staminaBehaviour == null)
                staminaBehaviour = GetComponent<StaminaComponentBehaviour>();
            if (equipmentController == null)
                equipmentController = GetComponent<Inventory.PlayerEquipmentController>();
        }

        /// <summary>
        /// Inicia el ataque: activa el collider del arma equipada y consume stamina.
        /// </summary>
        public void TryAttack()
        {
            if (Time.time - lastAttackTime < attackCooldown)
                return;

            if (staminaBehaviour != null && staminaBehaviour.Stamina != null && staminaBehaviour.Stamina.CurrentStamina >= staminaCost)
            {
                if (equipmentController == null || equipmentController.EquippedWeaponInstance == null)
                {
                    Debug.LogWarning("No hay arma equipada para atacar.");
                    return;
                }
                staminaBehaviour.Stamina.UseStamina(staminaCost);
                lastAttackTime = Time.time;

                // Activar recuperación de stamina en el controlador del jugador
                var playerHealthController = GetComponent<ProyectSecret.Characters.PlayerHealthController>();
                if (playerHealthController != null)
                {
                    playerHealthController.OnPlayerAttack();
                }

                // Activa el collider del arma equipada (debe estar desactivado por defecto)
                var weaponInstance = equipmentController.EquippedWeaponInstance;
                if (weaponInstance != null && weaponInstance.WeaponData != null)
                {
                    var weaponHitbox = weaponInstance.WeaponData.GetWeaponHitboxInstance();
                    if (weaponHitbox != null)
                    {
                        weaponHitbox.SetWeapon(weaponInstance.WeaponData, gameObject);
                        weaponHitbox.EnableDamage();
                        // Opcional: desactivar después de un tiempo o por animación
                    }
                }
            }
            else
            {
                Debug.Log("No hay suficiente stamina para atacar.");
            }
        }
    }
}
