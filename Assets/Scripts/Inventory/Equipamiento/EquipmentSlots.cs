using System.Collections.Generic;
using Interfaces;
using UnityEngine;

namespace Inventory.Equipamiento
{
    /// <summary>
    /// Gestiona los ítems equipados por el jugador en diferentes slots.
    /// </summary>
    public class EquipmentSlots : MonoBehaviour
    {
        private readonly Dictionary<EquipmentSlotType, IEquipable> equippedItems = new Dictionary<EquipmentSlotType, IEquipable>();

        public IEquipable GetEquipped(EquipmentSlotType slotType)
        {
            equippedItems.TryGetValue(slotType, out var item);
            return item;
        }

        public bool EquipItem(IEquipable item, GameObject user)
        {
            if (item == null) return false;
            var slot = item.GetSlotType();
            if (equippedItems.TryGetValue(slot, out var current))
            {
                current.OnUnequip(user);
            }
            equippedItems[slot] = item;
            item.OnEquip(user);
            return true;
        }

        public bool UnequipItem(EquipmentSlotType slotType, GameObject user)
        {
            if (equippedItems.TryGetValue(slotType, out var item))
            {
                item.OnUnequip(user);
                equippedItems.Remove(slotType);
                return true;
            }
            return false;
        }

        public IReadOnlyDictionary<EquipmentSlotType, IEquipable> GetAllEquipped() => equippedItems;
    }
}
