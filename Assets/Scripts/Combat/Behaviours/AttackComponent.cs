using UnityEngine;
using Components;
using Interfaces;
using Inventory;

namespace Combat.Behaviours
{
    /// <summary>
    /// Componente modular para lógica de ataque. Puede ser añadido a cualquier entidad.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class AttackComponent : MonoBehaviour
    {
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private int staminaCost = 10;
        [SerializeField] private StaminaComponent staminaComponent;
        [SerializeField] private PlayerEquipmentController equipmentController;
        private float lastAttackTime = -999f;
        [SerializeField]private WeaponItem weapon;

        private void Awake()
        {
            if (staminaComponent == null)
                staminaComponent = GetComponent<StaminaComponent>();
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

            if (staminaComponent != null && staminaComponent.CurrentStamina >= staminaCost)
            {
                if (equipmentController == null || equipmentController.EquippedWeaponInstance == null)
                {
                    Debug.LogWarning("No hay arma equipada para atacar.");
                    return;
                }
                staminaComponent.UseStamina(staminaCost);
                lastAttackTime = Time.time;
                // Activa el collider del arma equipada (debe estar desactivado por defecto)
                var weaponInstance = equipmentController.EquippedWeaponInstance;
                if (weaponInstance != null && weaponInstance.weaponData != null)
                {
                    var weaponHitbox = weaponInstance.weaponData.GetWeaponHitboxInstance();
                    if (weaponHitbox != null)
                    {
                        weaponHitbox.SetWeapon(weaponInstance.weaponData, gameObject);
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
