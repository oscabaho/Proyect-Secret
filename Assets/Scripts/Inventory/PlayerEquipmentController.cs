using UnityEngine;

namespace Inventory
{
    /// <summary>
    /// Ejemplo de controlador para equipar ítems desde el inventario del jugador.
    /// Puede ser llamado desde UI o input.
    /// </summary>
    public class PlayerEquipmentController : MonoBehaviour
    {
        [SerializeField] private PlayerInventory playerInventory;
        [SerializeField] private GameObject playerObject; // Referencia al objeto jugador (puede ser this.gameObject)

        private void Awake()
        {
            if (playerInventory == null)
                playerInventory = GetComponent<PlayerInventory>();
            if (playerObject == null)
                playerObject = gameObject;
        }

        /// <summary>
        /// Llama este método para equipar un ítem por su ID (por ejemplo, desde la UI).
        /// </summary>
        public void EquipItemById(string itemId)
        {
            bool result = playerInventory.EquipItem(itemId, playerObject);
            if (!result)
                Debug.LogWarning($"No se pudo equipar el ítem con ID: {itemId}");
        }
    }
}
