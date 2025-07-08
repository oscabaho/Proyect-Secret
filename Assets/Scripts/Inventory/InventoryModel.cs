using System;
using System.Collections.Generic;
using ProyectSecret.Interfaces;

namespace ProyectSecret.Inventory
{
    /// <summary>
    /// Lógica pura de inventario, desacoplada de Unity.
    /// </summary>
    public class InventoryModel
    {
        public int MaxSlots { get; }
        private readonly List<MysteryItem> items = new List<MysteryItem>();
        public event Action OnInventoryChanged;

        public InventoryModel(int maxSlots)
        {
            MaxSlots = maxSlots;
        }

        public bool HasItem(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return false;
            return items.Exists(i => i != null && i.Id == itemId);
        }

        public bool AddItem(MysteryItem item)
        {
            if (item == null) return false;
            if (items.Count >= MaxSlots) return false;
            if (items.Exists(i => i != null && i.Id == item.Id)) return false;
            items.Add(item);
            OnInventoryChanged?.Invoke();
            return true;
        }

        public bool RemoveItem(string itemId)
        {
            var item = items.Find(i => i != null && i.Id == itemId);
            if (item == null) return false;
            items.Remove(item);
            OnInventoryChanged?.Invoke();
            return true;
        }

        public IReadOnlyList<MysteryItem> GetItems() => items.AsReadOnly();
    }
}
