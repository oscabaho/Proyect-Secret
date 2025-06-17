using Interfaces;
using UnityEngine;

namespace Inventory
{
    [System.Serializable]
    public class WeaponItem : MysteryItem, IUsableItem
    {
        [SerializeField] private int baseDamage;
        [SerializeField] private float attackSpeed; // ataques por segundo

        public int BaseDamage => baseDamage;
        public float AttackSpeed => attackSpeed;
        public float DPS => baseDamage * attackSpeed;

        public WeaponItem(string id, string displayName, string description, int baseDamage, float attackSpeed, Sprite icon = null)
            : base(id, displayName, description, icon)
        {
            this.baseDamage = baseDamage;
            this.attackSpeed = attackSpeed;
        }

        // Implementación de IUsableItem
        public void Use(GameObject user)
        {
            // Lógica placeholder: podrías equipar el arma, mostrar feedback, etc.
            Debug.Log($"{DisplayName} equipada. Daño base: {baseDamage}, Velocidad de ataque: {attackSpeed}");
        }

        public string GetId() => Id;
    }
}
