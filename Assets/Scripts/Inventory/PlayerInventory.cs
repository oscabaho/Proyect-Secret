using System;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

namespace Inventory
{
    /// <summary>
    /// Inventario robusto del jugador. Permite agregar, quitar y consultar ítems de forma segura.
    /// Notifica cambios mediante evento.
    /// </summary>
    public class PlayerInventory : MonoBehaviour, IInventory
    {
        [SerializeField] private int maxSlots = 5;
        [SerializeField] private List<MysteryItem> items = new List<MysteryItem>();
        public event Action OnInventoryChanged;

        /// <summary>
        /// Verifica si el inventario contiene el ítem especificado.
        /// </summary>
        public bool HasItem(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return false;
            return items.Exists(i => i.Id == itemId);
        }

        /// <summary>
        /// Agrega un ítem al inventario si no existe y es válido.
        /// </summary>
        public bool AddItem(MysteryItem item)
        {
            if (item == null)
            {
                Debug.LogWarning("PlayerInventory: item nulo.");
                return false;
            }
            if (items.Count >= maxSlots)
            {
                Debug.LogWarning("PlayerInventory: Inventario lleno.");
                return false;
            }
            if (items.Exists(i => i.Id == item.Id))
            {
                Debug.LogWarning($"PlayerInventory: El ítem '{item.Id}' ya está en el inventario.");
                return false;
            }
            items.Add(item);
            OnInventoryChanged?.Invoke();
            return true;
        }

        /// <summary>
        /// Quita un ítem del inventario si existe.
        /// </summary>
        public bool RemoveItem(string itemId)
        {
            var item = items.Find(i => i.Id == itemId);
            if (item == null)
            {
                Debug.LogWarning($"PlayerInventory: El ítem '{itemId}' no está en el inventario.");
                return false;
            }
            items.Remove(item);
            OnInventoryChanged?.Invoke();
            return true;
        }

        /// <summary>
        /// Devuelve una copia de la lista de ítems (solo lectura).
        /// </summary>
        public IReadOnlyList<MysteryItem> GetItems()
        {
            return items.AsReadOnly();
        }

        /// <summary>
        /// Usa un ítem del inventario, ejecutando su efecto y eliminándolo si es usable.
        /// </summary>
        public bool UseItem(string itemId, GameObject user)
        {
            var item = items.Find(i => i.Id == itemId);
            if (item == null) return false;
            var usable = ItemDatabase.GetItem(itemId);
            if (usable != null)
            {
                usable.Use(user);
                RemoveItem(itemId);
                return true;
            }
            Debug.LogWarning($"PlayerInventory: El ítem '{itemId}' no es usable o no está registrado en el catálogo.");
            return false;
        }
    }
}
