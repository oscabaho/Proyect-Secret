using UnityEngine;
using ProyectSecret.MonoBehaviours.Player;
using System.Collections;
using ProyectSecret.Combat.Behaviours;
using ProyectSecret.Inventory.Items;
using ProyectSecret.Utils; // Importar el nuevo namespace

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

        [Header("Object Pooling")]
        [SerializeField] private int hitboxPoolSize = 3;
        private ObjectPool<WeaponHitbox> hitboxPool;
        
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
                    // Reseteamos la posición y rotación al cambiar de padre
                    equippedWeaponGO.transform.localPosition = Vector3.zero;
                    equippedWeaponGO.transform.localRotation = Quaternion.identity;
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
                InitializeEquippedWeapon(instance.WeaponData);
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
            InitializeEquippedWeapon(weaponItem);
        }

        public bool CanAttack()
        {
            return EquippedWeaponInstance != null && EquippedWeaponInstance.WeaponData != null;
        }

        public bool Attack()
        {
            if (!CanAttack()) return false;
            if (hitboxPool == null) return false; // No hay pool si no hay arma con hitbox
            
            GameObject hitboxObj = hitboxPool.Get();
            if (hitboxObj == null)
            {
                #if UNITY_EDITOR
                Debug.LogWarning("No hay hitbox disponible en el pool. El ataque podría ser demasiado rápido o el pool demasiado pequeño.");
                #endif
                return false;
            }
            
            Transform hitBoxPoint = GetActiveHitboxPoint();
            if (hitBoxPoint == null) return false;

            var weaponHitbox = hitboxObj.GetComponent<WeaponHitbox>();
            if (weaponHitbox != null)
            {
                hitboxObj.transform.SetParent(hitBoxPoint, false);
                hitboxObj.transform.localPosition = Vector3.zero;
                hitboxObj.transform.localRotation = Quaternion.identity;
                hitboxObj.SetActive(true);

                weaponHitbox.Initialize(EquippedWeaponInstance, gameObject);
                weaponHitbox.EnableDamage();
                
                StartCoroutine(ReturnHitboxToPool(hitboxObj, 0.3f));
                return true;
            }

            // Si el objeto del pool no tiene el componente, lo desactivamos para evitar problemas.
            hitboxObj.SetActive(false);
            return false;
        }

        private void InitializeEquippedWeapon(WeaponItem weaponItem)
        {
            UpdateWeaponVisuals(weaponItem);
            
            // Limpiar el pool anterior y crear uno nuevo para la nueva arma.
            hitboxPool?.Clear();
            hitboxPool = null; // Liberar la referencia
            if (weaponItem != null && weaponItem.HitBoxPrefab != null)
                hitboxPool = new ObjectPool<WeaponHitbox>(weaponItem.HitBoxPrefab, hitboxPoolSize, transform);
        }

        private void UpdateWeaponVisuals(WeaponItem weaponItem)
        {
            if (equippedWeaponGO != null)
                Destroy(equippedWeaponGO);

            if (weaponItem != null && weaponItem.WeaponPrefab != null)
            {
                Transform weaponPoint = GetActiveWeaponPoint();
                if (weaponPoint != null)
                {
                    equippedWeaponGO = Instantiate(weaponItem.WeaponPrefab, weaponPoint);
                    // Forzamos la posición y rotación a cero relativo al padre.
                    equippedWeaponGO.transform.localPosition = Vector3.zero;
                    equippedWeaponGO.transform.localRotation = Quaternion.identity;
                }
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
            // Limpiar el pool de hitboxes al desequipar.
            hitboxPool?.Clear();
            hitboxPool = null;
        }

        private IEnumerator ReturnHitboxToPool(GameObject hitboxObj, float delay)
        {
            yield return new WaitForSeconds(delay);
            // Devolver el objeto al pool para su reutilización.
            hitboxPool?.Return(hitboxObj);
        }
    }
}
