
using UnityEngine;
using ProyectSecret.Inventory.Items;

namespace ProyectSecret.Inventory
{
    /// <summary>
    /// Controlador de equipamiento del jugador. Gestiona el equipamiento y auto-equipamiento de armas y otros ítems.
    /// </summary>
    public class PlayerEquipmentController : MonoBehaviour
    {
        [SerializeField] private EquipmentSlots equipmentSlots;

        // Instancia equipada actualmente (puede ser null si no hay arma equipada)
        public WeaponInstance EquippedWeaponInstance { get; private set; }

        public EquipmentSlots EquipmentSlots => equipmentSlots;

        public void EquipWeapon(WeaponItem weaponItem)
        {
            equipmentSlots.EquipWeapon(weaponItem);
            EquippedWeaponInstance = new WeaponInstance(weaponItem);
        }

        public void EquipItemById(string itemId)
        {
            // Lógica para equipar un ítem por su ID (ejemplo: buscar en inventario y asignar al slot correspondiente)
            Debug.Log($"Equipando ítem con ID: {itemId}");
            // Implementa la lógica real según tu inventario
        }

        public void UnequipWeapon()
        {
            equipmentSlots?.UnequipWeapon();
            EquippedWeaponInstance = null;
        }
    }
}
