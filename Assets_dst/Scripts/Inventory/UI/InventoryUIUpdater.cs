using UnityEngine;
using ProyectSecret.Inventory;
using ProyectSecret.Events;

namespace ProyectSecret.Inventory.UI
{
    /// <summary>
    /// Ejemplo de suscriptor al evento InventoryChangedEvent para actualizar la UI.
    /// </summary>
    public class InventoryUIUpdater : MonoBehaviour
    {
        private void OnEnable()
        {
            GameEventBus.Instance.Subscribe<InventoryChangedEvent>(OnInventoryChanged);
            // Suscribirse a eventos directos del inventario si es necesario
            var playerInventory = Object.FindFirstObjectByType<PlayerInventory>();
            if (playerInventory != null)
                playerInventory.OnInventoryChanged += UpdateUI;
        }

        private void OnDisable()
        {
            GameEventBus.Instance.Unsubscribe<InventoryChangedEvent>(OnInventoryChanged);
            var playerInventory = Object.FindFirstObjectByType<PlayerInventory>();
            if (playerInventory != null)
                playerInventory.OnInventoryChanged -= UpdateUI;
        }

        private void OnInventoryChanged(InventoryChangedEvent evt)
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            var playerInventory = Object.FindFirstObjectByType<PlayerInventory>();
            int count = playerInventory != null ? playerInventory.GetItems().Count : 0;
            Debug.Log($"Inventario actualizado. Total de ítems: {count}");
            // Ejemplo: Redibujar slots, actualizar iconos, etc.
        }
    }
}
