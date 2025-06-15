using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    /// <summary>
    /// Inventario robusto del jugador. Permite agregar, quitar y consultar ítems de forma segura.
    /// Notifica cambios mediante evento.
    /// </summary>
    public class PlayerInventory : MonoBehaviour, IInventory
    {
        [SerializeField] private List<string> items = new List<string>();
        public event Action OnInventoryChanged;

        /// <summary>
        /// Verifica si el inventario contiene el ítem especificado.
        /// </summary>
        public bool HasItem(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return false;
            return items.Contains(itemId);
        }

        /// <summary>
        /// Agrega un ítem al inventario si no existe y es válido.
        /// </summary>
        public bool AddItem(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                Debug.LogWarning("PlayerInventory: itemId nulo o vacío.");
                return false;
            }
            if (items.Contains(itemId))
            {
                Debug.LogWarning($"PlayerInventory: El ítem '{itemId}' ya está en el inventario.");
                return false;
            }
            items.Add(itemId);
            OnInventoryChanged?.Invoke();
            return true;
        }

        /// <summary>
        /// Quita un ítem del inventario si existe.
        /// </summary>
        public bool RemoveItem(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return false;
            if (!items.Contains(itemId))
            {
                Debug.LogWarning($"PlayerInventory: El ítem '{itemId}' no está en el inventario.");
                return false;
            }
            items.Remove(itemId);
            OnInventoryChanged?.Invoke();
            return true;
        }

        /// <summary>
        /// Devuelve una copia de la lista de ítems (solo lectura).
        /// </summary>
        public IReadOnlyList<string> GetItems()
        {
            return items.AsReadOnly();
        }

        /// <summary>
        /// Usa un ítem del inventario, ejecutando su efecto y eliminándolo si es usable.
        /// </summary>
        public bool UseItem(string itemId, GameObject user)
        {
            if (!HasItem(itemId)) return false;
            var item = ItemDatabase.GetItem(itemId);
            if (item != null)
            {
                item.Use(user);
                RemoveItem(itemId);
                return true;
            }
            Debug.LogWarning($"PlayerInventory: El ítem '{itemId}' no es usable o no está registrado en el catálogo.");
            return false;
        }
    }
}
