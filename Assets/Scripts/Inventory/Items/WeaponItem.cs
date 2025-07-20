using UnityEngine;
using ProyectSecret.Interfaces;
using ProyectSecret.Combat.Behaviours;

namespace ProyectSecret.Inventory.Items
{
    /// <summary>
    /// ScriptableObject base para armas. Define sus propiedades y comportamiento al ser equipado.
    /// </summary>
    [CreateAssetMenu(fileName = "WeaponItem", menuName = "Inventory/WeaponItem")]
    public class WeaponItem : MysteryItem, IEquipable
    {
        [Header("Datos del arma")]
        [SerializeField] private int weaponDamage = 10;
        [SerializeField] private float attackSpeed = 1f;
        [SerializeField] private float maxDurability = 100f;
        [SerializeField] private AnimationCurve durabilityCurve = null;
        [SerializeField] private AnimationCurve masteryCurve = null;
        [SerializeField] private int maxMasteryHits = 100;
        
        [Header("Prefab de hitbox de ataque")]
        [SerializeField] private GameObject hitBoxPrefab;
        [Header("Prefab visual del arma")]
        [SerializeField] private GameObject weaponPrefab;

        public int WeaponDamage => weaponDamage;
        public float AttackSpeed => attackSpeed;
        public float MaxDurability => maxDurability;
        public AnimationCurve DurabilityCurve => durabilityCurve;
        public AnimationCurve MasteryCurve => masteryCurve;
        public int MaxMasteryHits => maxMasteryHits;
        public GameObject HitBoxPrefab => hitBoxPrefab;
        public GameObject WeaponPrefab => weaponPrefab;
        public AudioClip AttackSoundAir => AttackSoundAir;
        public AudioClip AttackSoundEnemy => AttackSoundEnemy;
        public AudioClip AttackSoundObject => AttackSoundObject;

        /// <summary>
        /// Aplica daño al objetivo usando la lógica del arma.
        /// </summary>
        public virtual void ApplyDamage(GameObject owner, GameObject target)
        {
            var damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(weaponDamage);
                #if UNITY_EDITOR
                Debug.Log($"{owner.name} infligió {weaponDamage} de daño a {target.name} con {name}.");
                #endif
            }
        }

        // Implementación de IEquipable
        public EquipmentSlotType GetSlotType() => EquipmentSlotType.Weapon;
        public void OnEquip(GameObject user)
        {
            #if UNITY_EDITOR
            Debug.Log($"{DisplayName} equipada en el jugador.");
            #endif
        }
        public void OnUnequip(GameObject user)
        {
            #if UNITY_EDITOR
            Debug.Log($"{DisplayName} fue desequipada.");
            #endif
        }
        public string GetId() => Id;
    }
}
