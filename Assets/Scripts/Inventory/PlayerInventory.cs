using System;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using System.Linq;
using Inventory.Equipamiento;

namespace Inventory
{
    /// <summary>
    /// Componente Unity que actúa como puente entre el motor y la lógica pura de inventario.
    /// </summary>
    public class PlayerInventory : MonoBehaviour, IInventory
    {
        [SerializeField] private int maxSlots = 5;
        [SerializeField] private EquipmentSlots equipmentSlots;
        [SerializeField] private List<MysteryItem> initialItems = new List<MysteryItem>();
        public event Action OnInventoryChanged;

        private InventoryModel inventoryModel;

        private void Awake()
        {
            inventoryModel = new InventoryModel(maxSlots);
            foreach (var item in initialItems)
                inventoryModel.AddItem(item);
            inventoryModel.OnInventoryChanged += () => OnInventoryChanged?.Invoke();
        }

        public bool HasItem(string itemId) => inventoryModel.HasItem(itemId);
        public bool AddItem(MysteryItem item) => inventoryModel.AddItem(item);
        public bool RemoveItem(string itemId) => inventoryModel.RemoveItem(itemId);
        public IReadOnlyList<MysteryItem> GetItems() => inventoryModel.GetItems();

        /// <summary>
        /// Usa un ítem del inventario, ejecutando su efecto y eliminándolo si es usable.
        /// </summary>
        public bool UseItem(string itemId, GameObject user)
        {
            var item = inventoryModel.GetItems().FirstOrDefault(i => i != null && i.Id == itemId);
            if (item == null) return false;
            var usable = item as IUsableItem;
            if (usable != null)
            {
                usable.Use(user);
                RemoveItem(itemId);
                GameEventBus.Instance.Publish(new InventoryChangedEvent(this));
                GameEventBus.Instance.Publish(new ItemUsedEvent(itemId, user));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Equipa un ítem del inventario si es equipable.
        /// </summary>
        public bool EquipItem(string itemId, GameObject user)
        {
            var item = inventoryModel.GetItems().FirstOrDefault(i => i != null && i.Id == itemId);
            var equipable = item as Interfaces.IEquipable;
            if (equipable == null || equipmentSlots == null) return false;
            bool result = equipmentSlots.EquipItem(equipable, user);
            if (result)
            {
                Debug.Log($"{item.DisplayName} equipado en slot {equipable.GetSlotType()} por {user.name}");
            }
            return result;
        }
    }
}
