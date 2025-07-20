using UnityEngine;
using ProyectSecret.MonoBehaviours.Player;
using ProyectSecret.Combat.Behaviours;
using ProyectSecret.Inventory.Items;

namespace ProyectSecret.Inventory
{
    /// <summary>
    /// Controlador de equipamiento del jugador. Gestiona el equipamiento y auto-equipamiento de armas y otros ítems.
    /// </summary>
    public class PlayerEquipmentController : MonoBehaviour, ProyectSecret.Interfaces.IPlayerEquipmentController
    {
        private PlayerPointSwitcher pointSwitcher;
        private PaperMarioPlayerMovement playerMovement;
        private GameObject equippedWeaponGO;
        
        [SerializeField] private EquipmentSlots equipmentSlots;

        public WeaponInstance EquippedWeaponInstance { get; private set; }
        public EquipmentSlots EquipmentSlots => equipmentSlots;

        private void Awake()
        {
            pointSwitcher = GetComponent<PlayerPointSwitcher>();
            playerMovement = GetComponent<PaperMarioPlayerMovement>();
        }

        private void OnEnable()
        {
            if (playerMovement != null)
            {
                playerMovement.OnCameraInvertedChanged += HandleCameraInvertedChanged;
            }
        }

        private void OnDisable()
        {
            if (playerMovement != null)
            {
                playerMovement.OnCameraInvertedChanged -= HandleCameraInvertedChanged;
            }
        }

        private void HandleCameraInvertedChanged(bool isInverted)
        {
            if (equippedWeaponGO != null)
            {
                Transform newParent = GetActiveWeaponPoint();
                if (newParent != null)
                {
                    equippedWeaponGO.transform.SetParent(newParent, false);
                }
            }
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
                UpdateWeaponVisuals(instance.WeaponData);
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
            UpdateWeaponVisuals(weaponItem);
        }

        public bool CanAttack()
        {
            return EquippedWeaponInstance != null && EquippedWeaponInstance.WeaponData != null;
        }

        public bool Attack()
        {
            if (!CanAttack()) return false;

            WeaponItem weaponData = EquippedWeaponInstance.WeaponData;
            if (weaponData.HitBoxPrefab == null) return false;

            Transform hitBoxPoint = GetActiveHitboxPoint();
            if (hitBoxPoint == null) return false;

            GameObject hitboxObj = Instantiate(weaponData.HitBoxPrefab, hitBoxPoint.position, hitBoxPoint.rotation, hitBoxPoint);
            var weaponHitbox = hitboxObj.GetComponent<WeaponHitbox>();

            if (weaponHitbox != null)
            {
                weaponHitbox.SetWeapon(weaponData, gameObject);
                weaponHitbox.EnableDamage();
                Destroy(hitboxObj, 0.3f); // Autodestrucción para limpiar
                return true;
            }

            Destroy(hitboxObj); // Limpiar si no tiene el componente correcto
            return false;
        }

        private void UpdateWeaponVisuals(WeaponItem weaponItem)
        {
            if (equippedWeaponGO != null)
                Destroy(equippedWeaponGO);

            if (weaponItem != null && weaponItem.WeaponPrefab != null)
            {
                Transform weaponPoint = GetActiveWeaponPoint();
                if (weaponPoint != null)
                    equippedWeaponGO = Instantiate(weaponItem.WeaponPrefab, weaponPoint);
            }
        }

        private Transform GetActiveHitboxPoint()
        {
            if (pointSwitcher != null && playerMovement != null)
            {
                return playerMovement.isCameraInverted ? pointSwitcher.HitBoxPoint1 : pointSwitcher.HitBoxPoint;
            }
            return transform;
        }

        private Transform GetActiveWeaponPoint()
        {
            if (pointSwitcher != null && playerMovement != null)
            {
                return playerMovement.isCameraInverted ? pointSwitcher.WeaponPoint1 : pointSwitcher.WeaponPoint;
            }
            return transform;
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
