using UnityEngine;
using ProyectSecret.Interfaces;

namespace ProyectSecret.Inventory.Items
{
    /// <summary>
    /// Representa un ítem misterioso en el inventario. Su tipo real se revela solo al usarlo.
    /// </summary>
    [CreateAssetMenu(fileName = "MysteryItem", menuName = "Inventory/MysteryItem")]
    public abstract class MysteryItem : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private string description; // Descripción para tooltip
        [SerializeField] private Sprite icon;
        // Puedes agregar más campos visuales si lo deseas

        public virtual string Id => id;
        public virtual string DisplayName => displayName;
        public virtual string Description => description;
        public virtual Sprite Icon => icon;

        public abstract void Use();
    }
}
