using UnityEngine;
using ProyectSecret.Inventory.Items;

namespace ProyectSecret.Inventory
{
    /// <summary>
    /// Controlador de equipamiento del jugador. Gestiona el equipamiento y auto-equipamiento de armas y otros ítems.
    /// </summary>
    // [RequireComponent(typeof(EquipmentSlots))] // EquipmentSlots no es un MonoBehaviour
public class PlayerEquipmentController : MonoBehaviour, ProyectSecret.Interfaces.IPlayerEquipmentController
    {
        [SerializeField] private EquipmentSlots equipmentSlots;
        [Header("Punto de equipamiento (Weapon Holder)")]
        [SerializeField] private Transform weaponHolder; // Asigna el hijo en el prefab del jugador
        private GameObject equippedWeaponGO;

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
                #if UNITY_EDITOR
                Debug.LogError("[PlayerEquipmentController] EquipmentSlots no asignado.");
                #endif
                return;
            }
            if (instance != null)
            {
                equipmentSlots.EquipWeapon(instance.WeaponData);
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
                #if UNITY_EDITOR
                Debug.LogError("[PlayerEquipmentController] EquipmentSlots no asignado.");
                #endif
                return;
            }
            equipmentSlots.EquipWeapon(weaponItem);
            EquippedWeaponInstance = new WeaponInstance(weaponItem);

            // Instancia el prefab del arma equipada en el punto de equipamiento
            if (equippedWeaponGO != null)
                Destroy(equippedWeaponGO);
            if (weaponItem != null && (weaponItem as WeaponItem).WeaponPrefab != null && weaponHolder != null)
            {
                equippedWeaponGO = Instantiate((weaponItem as WeaponItem).WeaponPrefab, weaponHolder);
                equippedWeaponGO.transform.localPosition = Vector3.zero;
                equippedWeaponGO.transform.localRotation = Quaternion.identity;
            }
        }

        public void EquipItemById(string itemId)
        {
            #if UNITY_EDITOR
            Debug.Log($"Equipando ítem con ID: {itemId}");
            #endif
            // Implementa la lógica real según tu inventario
        }

        public void UnequipWeapon()
        {
            if (equipmentSlots == null)
            {
                #if UNITY_EDITOR
                Debug.LogError("[PlayerEquipmentController] EquipmentSlots no asignado.");
                #endif
                return;
            }
            equipmentSlots?.UnequipWeapon();
            EquippedWeaponInstance = null;
            if (equippedWeaponGO != null)
            {
                Destroy(equippedWeaponGO);
                equippedWeaponGO = null;
            }
        }
    }
}
