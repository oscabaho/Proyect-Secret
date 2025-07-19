using ProyectSecret.MonoBehaviours.Player;
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
    // Referencias a los objetos instanciados de arma y hitbox
    private GameObject equippedWeaponGO;
    private GameObject equippedHitBoxGO;
        [SerializeField] private EquipmentSlots equipmentSlots;
        // Los puntos de hitbox y weapon holder ahora son gestionados por PlayerPointSwitcher

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
        /// Cambia el punto de equipamiento del arma equipada y el hitbox dinámicamente.
        /// </summary>
        // Ahora el cambio de puntos se gestiona por PlayerPointSwitcher
        // Si necesitas mover el arma/hitbox, llama a los métodos del switcher desde el controlador de movimiento

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

            // Instancia el prefab del arma equipada y el hitbox en los puntos activos del switcher
            var pointSwitcher = GetComponent<PlayerPointSwitcher>();
            Transform weaponPoint = null;
            Transform hitBoxPoint = null;
            if (pointSwitcher != null)
            {
                bool isCameraInverted = GetComponent<PaperMarioPlayerMovement>()?.isCameraInverted ?? false;
                weaponPoint = isCameraInverted ? pointSwitcher.WeaponPoint1 : pointSwitcher.WeaponPoint;
                hitBoxPoint = isCameraInverted ? pointSwitcher.HitBoxPoint1 : pointSwitcher.HitBoxPoint;
            }

            if (equippedWeaponGO != null)
                Destroy(equippedWeaponGO);
            if (weaponItem != null && weaponItem.WeaponPrefab != null && weaponPoint != null)
            {
                equippedWeaponGO = Instantiate(weaponItem.WeaponPrefab, weaponPoint);
                equippedWeaponGO.transform.localPosition = Vector3.zero;
                equippedWeaponGO.transform.localRotation = Quaternion.identity;
            }

            if (equippedHitBoxGO != null)
                Destroy(equippedHitBoxGO);
            if (weaponItem != null && weaponItem.HitBoxPrefab != null && hitBoxPoint != null)
            {
                equippedHitBoxGO = Instantiate(weaponItem.HitBoxPrefab, hitBoxPoint);
                equippedHitBoxGO.transform.localPosition = Vector3.zero;
                equippedHitBoxGO.transform.localRotation = Quaternion.identity;
                Destroy(equippedHitBoxGO, 0.3f);
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
            if (equippedHitBoxGO != null)
            {
                Destroy(equippedHitBoxGO);
                equippedHitBoxGO = null;
            }
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
