using Interfaces;
using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(fileName = "WeaponItem", menuName = "Inventory/WeaponItem")]
    public class WeaponItem : ScriptableObject, IUsableItem, IEquipable
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private int baseDamage;
        [SerializeField] private float attackSpeed; // ataques por segundo

        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => icon;
        public int BaseDamage => baseDamage;
        public float AttackSpeed => attackSpeed;
        public float DPS => baseDamage * attackSpeed;

        // Implementación de IUsableItem
        public void Use(GameObject user)
        {
            // Lógica placeholder: podrías equipar el arma, mostrar feedback, etc.
            Debug.Log($"{DisplayName} equipada. Daño base: {baseDamage}, Velocidad de ataque: {attackSpeed}");
        }

        // Implementación de IEquipable
        public EquipmentSlotType GetSlotType() => EquipmentSlotType.Weapon;
        public void OnEquip(GameObject user)
        {
            Debug.Log($"{DisplayName} equipada por {user.name}");
            // Aquí puedes agregar lógica de equipamiento (por ejemplo, modificar stats)
        }
        public void OnUnequip(GameObject user)
        {
            Debug.Log($"{DisplayName} desequipada por {user.name}");
            // Aquí puedes revertir efectos de equipamiento
        }

        public string GetId() => Id;
    }
}
