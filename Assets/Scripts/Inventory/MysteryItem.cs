using UnityEngine;

namespace Inventory
{
    /// <summary>
    /// Representa un ítem misterioso en el inventario. Su tipo real se revela solo al usarlo.
    /// </summary>
    [System.Serializable]
    public class MysteryItem
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

        public MysteryItem(string id, string displayName, string description, Sprite icon = null)
        {
            this.id = id;
            this.displayName = displayName;
            this.description = description;
            this.icon = icon;
        }
    }
}
