using System;
using System.Collections.Generic;
using ProyectSecret.Interfaces;
using UnityEngine;
using System.Linq;
using ProyectSecret.Inventory.Equipamiento;
using ProyectSecret.Combat.SceneManagement;

namespace ProyectSecret.Inventory
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
            {
                if (item != null)
                    inventoryModel.AddItem(item);
            }
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
            var usable = item as Interfaces.IUsableItem;
            if (usable != null)
            {
                usable.Use(user);
                RemoveItem(itemId);
                OnInventoryChanged?.Invoke();
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
            if (equipable != null && equipmentSlots != null)
            {
                bool result = equipmentSlots.EquipItem(equipable, user);
                if (result)
                {
                    RemoveItem(itemId);
                    OnInventoryChanged?.Invoke();
                }
                return result;
            }
            return false;
        }

        /// <summary>
        /// Exporta el inventario completo (ítems y armas con estado) para persistencia entre escenas.
        /// </summary>
        public SerializableInventoryData ExportInventoryData()
        {
            var data = new SerializableInventoryData();
            foreach (var item in inventoryModel.GetItems())
            {
                if (item is WeaponItem weaponItem)
                {
                    // Buscar instancia equipada para obtener durabilidad y hits
                    var equipmentController = GetComponent<PlayerEquipmentController>();
                    if (equipmentController != null && equipmentController.EquippedWeaponInstance != null && equipmentController.EquippedWeaponInstance.weaponData == weaponItem)
                    {
                        data.weapons.Add(new SerializableWeaponData
                        {
                            weaponId = weaponItem.Id,
                            durability = equipmentController.EquippedWeaponInstance.currentDurability,
                            hits = equipmentController.EquippedWeaponInstance.hits
                        });
                    }
                    else
                    {
                        data.weapons.Add(new SerializableWeaponData
                        {
                            weaponId = weaponItem.Id,
                            durability = weaponItem.MaxDurability,
                            hits = 0
                        });
                    }
                }
                else if (item != null)
                {
                    data.itemIds.Add(item.Id);
                }
            }
            return data;
        }

        /// <summary>
        /// Importa el inventario completo desde datos serializados.
        /// </summary>
        public void ImportInventoryData(SerializableInventoryData data, ItemDatabase itemDatabase)
        {
            // Limpiar inventario actual
            var itemsToRemove = inventoryModel.GetItems().ToArray();
            foreach (var item in itemsToRemove)
                inventoryModel.RemoveItem(item.Id);

            // Restaurar ítems normales
            foreach (var id in data.itemIds)
            {
                var item = itemDatabase.GetItem(id);
                if (item != null)
                    inventoryModel.AddItem(item);
            }
            // Restaurar armas con estado
            foreach (var w in data.weapons)
            {
                var weaponItem = itemDatabase.GetItem(w.weaponId) as WeaponItem;
                if (weaponItem != null)
                {
                    inventoryModel.AddItem(weaponItem);
                    var equipmentController = GetComponent<PlayerEquipmentController>();
                    if (equipmentController != null && equipmentController.EquippedWeaponInstance != null && equipmentController.EquippedWeaponInstance.weaponData == weaponItem)
                    {
                        equipmentController.EquippedWeaponInstance.currentDurability = w.durability;
                        equipmentController.EquippedWeaponInstance.hits = w.hits;
                    }
                }
            }
            OnInventoryChanged?.Invoke();
        }
    }
}
