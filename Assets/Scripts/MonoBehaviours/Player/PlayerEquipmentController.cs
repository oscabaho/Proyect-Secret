using UnityEngine;
using ProyectSecret.Interfaces;
using ProyectSecret.Inventory;
using ProyectSecret.Inventory.Items;
using ProyectSecret.Combat.Behaviours;
using System.Collections;
using ProyectSecret.Events;
using ProyectSecret.MonoBehaviours.Player;
using ProyectSecret.Combat.SceneManagement;
using ProyectSecret.Managers; // Para el HitboxManager

// Asumo que tu controlador está en un namespace similar a este.
namespace ProyectSecret.Characters.Player
{
    public class PlayerEquipmentController : MonoBehaviour, IPlayerEquipmentController, IPersistentData
    {
        // El punto de anclaje del arma ahora se obtiene del PlayerPointSwitcher.
        private PlayerPointSwitcher pointSwitcher;

        private GameObject currentWeaponVisual;
        private GameObject currentHitboxInstance;
        
        // Propiedades de la interfaz IPlayerEquipmentController
        public WeaponInstance EquippedWeaponInstance { get; private set; }
        public EquipmentSlots EquipmentSlots { get; private set; } // Asumo que tienes esta clase

        private void Awake()
        {
            // Inicializar EquipmentSlots si es necesario
            EquipmentSlots = new EquipmentSlots();
            pointSwitcher = GetComponent<PlayerPointSwitcher>();

            if (pointSwitcher == null)
            {
                Debug.LogError("PlayerEquipmentController requiere un PlayerPointSwitcher en el mismo GameObject para funcionar correctamente.", this);
            }
        }

        public void EquipWeapon(WeaponItem weaponItem)
        {
            // 1. Desequipar siempre el arma actual para asegurar un estado limpio.
            UnequipWeapon();

            if (weaponItem == null) return;

            // 2. Crear la instancia lógica del arma (con su durabilidad, etc.).
            var weaponInstance = new WeaponInstance(weaponItem);
            EquipWeaponInstance(weaponInstance);
        }

        public void EquipWeaponInstance(WeaponInstance instance)
        {
            if (instance == null) return;

            EquippedWeaponInstance = instance;

            // 3. Instanciar el prefab visual del arma.
            if (instance.WeaponData.WeaponPrefab != null && pointSwitcher?.ActiveWeaponPoint != null)
            {
                currentWeaponVisual = Instantiate(instance.WeaponData.WeaponPrefab, pointSwitcher.ActiveWeaponPoint);
                // Es una buena práctica resetear la transformación local.
                currentWeaponVisual.transform.localPosition = Vector3.zero;
                currentWeaponVisual.transform.localRotation = Quaternion.identity;
                currentWeaponVisual.transform.localScale = Vector3.one;
            }

            // Notificar al item que ha sido equipado y publicar el evento
            instance.WeaponData?.OnEquip(gameObject);
            GameEventBus.Instance?.Publish(new PlayerWeaponEquippedEvent(gameObject, instance));

            // Aquí podrías notificar a otros sistemas (ej. animador) que se ha equipado un arma.
            // Por ejemplo: animator.SetInteger("WeaponType", (int)weaponItem.Type);
        }

        public void UnequipWeapon()
        {
            if (currentWeaponVisual != null)
            {
                Destroy(currentWeaponVisual);
                currentWeaponVisual = null;
            }

            if (EquippedWeaponInstance != null)
            {
                EquippedWeaponInstance.WeaponData?.OnUnequip(gameObject);
                GameEventBus.Instance?.Publish(new PlayerWeaponUnequippedEvent(gameObject, EquippedWeaponInstance));
            }

            EquippedWeaponInstance = null;
            // Notificar a otros sistemas que no hay arma equipada.
        }

        public bool Attack()
        {
            if (EquippedWeaponInstance == null || currentHitboxInstance != null)
                return false;

            var weaponData = EquippedWeaponInstance.WeaponData;
            var hitboxSpawnPoint = pointSwitcher?.ActiveHitBoxPoint;

            if (weaponData.HitBoxPrefab == null || hitboxSpawnPoint == null)
                return false;

            // Obtener la hitbox del pool en lugar de instanciarla.
            var hitbox = HitboxManager.Instance?.Get(weaponData.HitBoxPrefab);
            if (hitbox == null) return false;

            currentHitboxInstance = hitbox.gameObject;
            currentHitboxInstance.transform.SetParent(hitboxSpawnPoint, false); // false para no usar world position

            // Reproducir el VFX de ataque si está definido en el arma.
            if (!string.IsNullOrEmpty(weaponData.AttackVfxKey))
            {
                GameEventBus.Instance?.Publish(new PlayVFXRequest(weaponData.AttackVfxKey, hitboxSpawnPoint.position, hitboxSpawnPoint.rotation));
            }

            hitbox.Initialize(EquippedWeaponInstance, gameObject);
            hitbox.EnableDamage();
            // Usamos una corutina para desactivar la hitbox después de un tiempo.
            // La hitbox se devolverá al pool automáticamente en su OnDisable.
            StartCoroutine(HitboxLifecycle(hitbox, weaponData.AttackDuration));

            return true;
        }

        private IEnumerator HitboxLifecycle(WeaponHitbox hitbox, float duration)
        {
            yield return new WaitForSeconds(duration);
            if (hitbox != null && hitbox.gameObject.activeSelf)
            {
                hitbox.DisableDamage();
                hitbox.gameObject.SetActive(false); // Esto activará OnDisable y lo devolverá al pool.
                currentHitboxInstance = null;
            }
        }

        public bool CanAttack()
        {
            // Comprobar si se puede atacar (ej. no estar en cooldown).
            return EquippedWeaponInstance != null;
        }

        #region IPersistentData Implementation

        public void SaveData(PlayerPersistentData data)
        {
            if (EquippedWeaponInstance != null)
            {
                data.equippedWeaponId = EquippedWeaponInstance.WeaponData?.Id;
                data.equippedWeaponDurability = EquippedWeaponInstance.CurrentDurability;
                data.equippedWeaponHits = EquippedWeaponInstance.Hits;
            }
            else
            {
                data.equippedWeaponId = null;
                data.equippedWeaponDurability = 0;
                data.equippedWeaponHits = 0;
            }
        }

        public void LoadData(PlayerPersistentData data, ItemDatabase itemDatabase)
        {
            if (!string.IsNullOrEmpty(data.equippedWeaponId))
            {
                var weaponItem = itemDatabase.GetItem(data.equippedWeaponId) as WeaponItem;
                if (weaponItem == null) return;
                
                var weaponInstance = new WeaponInstance(weaponItem);
                weaponInstance.SetDurability(data.equippedWeaponDurability);
                weaponInstance.SetHits(data.equippedWeaponHits);
                EquipWeaponInstance(weaponInstance);
            }
        }

        #endregion
    }
}
