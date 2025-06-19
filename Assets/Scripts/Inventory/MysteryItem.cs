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
        [SerializeField] private AnimationCurve masteryCurve = AnimationCurve.Linear(0, 0, 1, 1); // Nueva curva para destreza
        [SerializeField] private int maxMasteryHits = 100; // Golpes para llegar a la máxima destreza

        public int BaseDamage => baseDamage;
        public float AttackSpeed => attackSpeed;
        public float DPS => baseDamage * attackSpeed;
        public float MaxDurability => maxDurability;
        public AnimationCurve DurabilityCurve => durabilityCurve;
        public AnimationCurve MasteryCurve => masteryCurve;
        public int MaxMasteryHits => maxMasteryHits;

        // Implementación de IUsableItem
        public void Use(GameObject user)
        {
            // Equipa el arma al jugador desde la UI o inventario.
            var equipmentController = user.GetComponent<Inventory.PlayerEquipmentController>();
            if (equipmentController != null)
            {
                equipmentController.EquipItemById(this.Id);
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
            // Aquí puedes agregar lógica de equipamiento:
            // - Aplicar efectos visuales al jugador.
            // - Modificar stats temporales (si tienes un sistema de stats).
            // - Mostrar feedback en la UI.
            Debug.Log($"{DisplayName} equipada en el jugador.");
        }

        public void OnUnequip(GameObject user)
        {
            // Aquí puedes revertir efectos de equipamiento:
            // - Quitar modificadores de stats.
            // - Desactivar efectos visuales.
            // - Mostrar feedback en la UI.
            Debug.Log($"{DisplayName} fue desequipada.");
        }

        public string GetId() => Id;
    }
}
