using UnityEngine;
using ProyectSecret.Interfaces;
using ProyectSecret.Combat.Behaviours;

namespace ProyectSecret.Inventory.Items
{
    /// <summary>
    /// ScriptableObject base para armas. Permite instanciar el hitbox, aplicar daño y ser usado/equipado.
    /// </summary>
    [CreateAssetMenu(fileName = "WeaponItem", menuName = "Inventory/WeaponItem")]
    public class WeaponItem : MysteryItem, IUsableItem, IEquipable
    {
        [Header("Datos del arma")]
        [SerializeField] private int weaponDamage = 10;
        [SerializeField] private float attackSpeed = 1f;
        [SerializeField] private float maxDurability = 100f;
        [SerializeField] private AnimationCurve durabilityCurve = null;
        [SerializeField] private AnimationCurve masteryCurve = null;
        [SerializeField] private int maxMasteryHits = 100;
        [SerializeField] private GameObject weaponHitboxPrefab;

        public int WeaponDamage => weaponDamage;
        public float AttackSpeed => attackSpeed;
        public float MaxDurability => maxDurability;
        public AnimationCurve DurabilityCurve => durabilityCurve;
        public AnimationCurve MasteryCurve => masteryCurve;
        public int MaxMasteryHits => maxMasteryHits;
        public GameObject WeaponHitboxPrefab => weaponHitboxPrefab;

        /// <summary>
        /// Instancia y retorna el hitbox del arma (debe ser hijo del jugador o del arma física).
        /// </summary>
        public WeaponHitbox GetWeaponHitboxInstance(Transform parent = null)
        {
            if (weaponHitboxPrefab == null)
            {
                Debug.LogWarning($"WeaponItem {name} no tiene asignado un prefab de hitbox.");
                return null;
            }
            GameObject hitboxObj = parent != null
                ? Object.Instantiate(weaponHitboxPrefab, parent)
                : Object.Instantiate(weaponHitboxPrefab);
            return hitboxObj.GetComponent<WeaponHitbox>();
        }

        /// <summary>
        /// Aplica daño al objetivo usando la lógica del arma.
        /// </summary>
        public virtual void ApplyDamage(GameObject owner, GameObject target)
        {
            var damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(weaponDamage);
                Debug.Log($"{owner.name} infligió {weaponDamage} de daño a {target.name} con {name}.");
            }
        }

        // Implementación de IUsableItem
        public void Use(GameObject user)
        {
            var equipmentController = user.GetComponent<PlayerEquipmentController>();
            if (equipmentController != null)
            {
                equipmentController.EquipItemById(((MysteryItem)this).Id);
                Debug.Log($"{DisplayName} equipada.");
            }
            else
            {
                Debug.LogWarning("No se encontró PlayerEquipmentController en el usuario.");
            }
        }

        // Implementación de IEquipable
        public EquipmentSlotType GetSlotType() => EquipmentSlotType.Weapon;
        public void OnEquip(GameObject user)
        {
            Debug.Log($"{DisplayName} equipada en el jugador.");
        }
        public void OnUnequip(GameObject user)
        {
            Debug.Log($"{DisplayName} fue desequipada.");
        }
        public string GetId() => Id;
    }
}