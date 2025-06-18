using UnityEngine;
using Interfaces;

namespace Inventory
{
    /// <summary>
    /// Representa un ítem misterioso en el inventario. Su tipo real se revela solo al usarlo.
    /// </summary>
    [CreateAssetMenu(fileName = "MysteryItem", menuName = "Inventory/MysteryItem")]
    public class MysteryItem : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private string description; // Descripción para tooltip
        [SerializeField] private Sprite icon;
        // Puedes agregar más campos visuales si lo deseas

        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => icon;
    }

    [CreateAssetMenu(fileName = "WeaponItem", menuName = "Inventory/WeaponItem")]
    public class WeaponItem : MysteryItem, IUsableItem, IEquipable
    {
        [SerializeField] private int baseDamage;
        [SerializeField] private float attackSpeed; // ataques por segundo
        [SerializeField] private float maxDurability = 100f;
        [SerializeField] private AnimationCurve durabilityCurve = AnimationCurve.Linear(0, 1, 1, 0);

        public int BaseDamage => baseDamage;
        public float AttackSpeed => attackSpeed;
        public float DPS => baseDamage * attackSpeed;
        public float MaxDurability => maxDurability;
        public AnimationCurve DurabilityCurve => durabilityCurve;

        // Implementación de IUsableItem
        public void Use(GameObject user)
        {
            // Lógica placeholder: podrías equipar el arma, mostrar feedback, etc.
        }

        // Implementación de IEquipable
        public EquipmentSlotType GetSlotType() => EquipmentSlotType.Weapon;
        public void OnEquip(GameObject user)
        {
            // Aquí puedes agregar lógica de equipamiento (por ejemplo, modificar stats)
        }
        public void OnUnequip(GameObject user)
        {
            // Aquí puedes revertir efectos de equipamiento
        }

        public string GetId() => Id;
    }
}
