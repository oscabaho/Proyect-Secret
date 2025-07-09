using UnityEngine;
using System;
using System.Linq;
using ProyectSecret.Inventory;
using ProyectSecret.Inventory.Items;

namespace ProyectSecret.Inventory
{
    /// <summary>
    /// Ejemplo de controlador para equipar ítems desde el inventario del jugador.
    /// Puede ser llamado desde UI o input.
    /// </summary>
    public class PlayerEquipmentController : MonoBehaviour
    {
        [SerializeField] private PlayerInventory playerInventory;
        [SerializeField] private GameObject playerObject; // Referencia al objeto jugador (puede ser this.gameObject)

        public WeaponInstance EquippedWeaponInstance { get; private set; }

        /// <summary>
<<<<<<< Updated upstream:Assets/Scripts/Inventory/PlayerEquipmentController.cs
=======
        /// Evento público para notificar el cambio de arma equipada a otros sistemas (UI, logros, etc).
        /// </summary>
        public event Action<WeaponInstance> OnWeaponEquipped;

        /// <summary>
>>>>>>> Stashed changes:Assets_dst/Scripts/Inventory/PlayerEquipmentController.cs
        /// Permite equipar directamente una instancia de arma (usado para restaurar durabilidad y maestría).
        /// </summary>
        public void EquipWeaponInstance(WeaponInstance instance)
        {
            EquippedWeaponInstance = instance;
<<<<<<< Updated upstream:Assets/Scripts/Inventory/PlayerEquipmentController.cs
=======
            OnWeaponEquipped?.Invoke(instance);
>>>>>>> Stashed changes:Assets_dst/Scripts/Inventory/PlayerEquipmentController.cs
            if (instance != null && instance.weaponData != null)
            {
                playerInventory.EquipItem(instance.weaponData.Id, playerObject);
                Debug.Log($"Arma restaurada: {instance.weaponData.DisplayName} (Durabilidad: {instance.currentDurability}, Hits: {instance.hits})");
            }
        }

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
            var item = playerInventory.GetItems().FirstOrDefault(i => i.Id == itemId);
            var weaponItem = item as WeaponItem;
            if (weaponItem != null)
            {
                EquippedWeaponInstance = new WeaponInstance(weaponItem);
                playerInventory.EquipItem(itemId, playerObject);
                Debug.Log($"Arma equipada: {weaponItem.DisplayName} (Durabilidad: {weaponItem.MaxDurability})");
            }
            else
            {
                Debug.LogWarning($"No se pudo equipar el ítem con ID: {itemId}");
            }
        }

        /// <summary>
        /// Llama este método cuando el arma golpea a un enemigo.
        /// </summary>
        public void OnWeaponHitEnemy()
        {
            if (EquippedWeaponInstance != null)
            {
                EquippedWeaponInstance.RegisterHit();
                Debug.Log($"Durabilidad actual: {EquippedWeaponInstance.currentDurability}");
                if (EquippedWeaponInstance.IsBroken())
                {
                    Debug.Log("¡El arma se ha roto!");
                    EquippedWeaponInstance = null;
                    AutoEquipFirstWeaponInInventory();
                }
            }
        }

        /// <summary>
        /// Busca y equipa automáticamente la primera arma disponible en el inventario.
        /// </summary>
        private void AutoEquipFirstWeaponInInventory()
        {
            var firstWeapon = playerInventory.GetItems().OfType<WeaponItem>().FirstOrDefault();
            if (firstWeapon != null)
            {
                EquipItemById(firstWeapon.Id);
                Debug.Log($"Arma de reserva equipada automáticamente: {firstWeapon.DisplayName}");
            }
            else
            {
                Debug.Log("No hay armas de reserva en el inventario.");
            }
        }
    }
}
