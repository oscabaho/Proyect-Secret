using System.Collections.Generic;
using ProyectSecret.Interfaces;
using ProyectSecret.Inventory.Items;
using UnityEngine;

namespace ProyectSecret.Inventory
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/ItemDatabase")]
    public class ItemDatabase : ScriptableObject
    {
        [SerializeField] private List<MysteryItem> itemsList = new List<MysteryItem>();
        private Dictionary<string, MysteryItem> itemsDict;

        /// <summary>
        /// Evento público para notificar cuando la base de datos de ítems cambia (añadir, quitar, modificar).
        /// </summary>
        public event System.Action OnDatabaseChanged;

        private void OnEnable()
        {
            itemsDict = new Dictionary<string, MysteryItem>();
            foreach (var item in itemsList)
            {
                if (item != null && !itemsDict.ContainsKey(item.Id))
                    itemsDict.Add(item.Id, item);
            }
        }

        public MysteryItem GetItem(string id)
        {
            if (itemsDict == null)
                OnEnable();
            itemsDict.TryGetValue(id, out var item);
            return item ?? NullMysteryItem.Instance;
        }

        public void AddItem(MysteryItem item)
        {
            if (item == null || itemsDict.ContainsKey(item.Id)) return;
            itemsList.Add(item);
            itemsDict[item.Id] = item;
            OnDatabaseChanged?.Invoke();
        }

        public void RemoveItem(string id)
        {
            if (!itemsDict.ContainsKey(id)) return;
            var item = itemsDict[id];
            itemsList.Remove(item);
            itemsDict.Remove(id);
            OnDatabaseChanged?.Invoke();
        }
    }

    /// <summary>
    /// Null Object para MysteryItem, evita comprobaciones de null.
    /// </summary>
    public class NullMysteryItem : MysteryItem
    {
        public static readonly MysteryItem Instance = new NullMysteryItem();
        public override string Id => string.Empty;
        public override string DisplayName => "Null Item";
        public override string Description => string.Empty;
        public override Sprite Icon => null;
        public override void Use() { }
    }
}
