using UnityEngine;
using ProyectSecret.Inventory.Items;

namespace ProyectSecret.Inventory
{
    /// <summary>
    /// Controlador de equipamiento del jugador. Gestiona el equipamiento y auto-equipamiento de armas y otros ítems.
    /// </summary>
    [RequireComponent(typeof(EquipmentSlots))]
public class PlayerEquipmentController : MonoBehaviour, ProyectSecret.Interfaces.IPlayerEquipmentController
    {
        [SerializeField] private EquipmentSlots equipmentSlots;

        // Instancia equipada actualmente (puede ser null si no hay arma equipada)
        public WeaponInstance EquippedWeaponInstance { get; private set; }

        public EquipmentSlots EquipmentSlots => equipmentSlots;

        private void Awake()
        {
            // Inyección de dependencias y cacheo de referencias
            if (equipmentSlots == null)
                equipmentSlots = GetComponent<EquipmentSlots>();
        }

        /// <summary>
        /// Permite restaurar directamente una instancia de arma equipada (para persistencia).
        /// </summary>
        public void EquipWeaponInstance(WeaponInstance instance)
        {
            if (equipmentSlots == null)
            {
                Debug.LogError("[PlayerEquipmentController] EquipmentSlots no asignado.");
                return;
            }
            if (instance != null)
            {
                equipmentSlots.EquipWeapon(instance.weaponData);
                EquippedWeaponInstance = instance;
            }
            else
            {
                UnequipWeapon();
            }
        }

        public void EquipWeapon(WeaponItem weaponItem)
        {
            if (equipmentSlots == null)
            {
                Debug.LogError("[PlayerEquipmentController] EquipmentSlots no asignado.");
                return;
            }
            equipmentSlots.EquipWeapon(weaponItem);
            EquippedWeaponInstance = new WeaponInstance(weaponItem);
        }

        public void EquipItemById(string itemId)
        {
            Debug.Log($"Equipando ítem con ID: {itemId}");
            // Implementa la lógica real según tu inventario
        }

        public void UnequipWeapon()
        {
            if (equipmentSlots == null)
            {
                Debug.LogError("[PlayerEquipmentController] EquipmentSlots no asignado.");
                return;
            }
            equipmentSlots?.UnequipWeapon();
            EquippedWeaponInstance = null;
        }
    }
}
