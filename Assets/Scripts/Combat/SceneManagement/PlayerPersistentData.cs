
using UnityEngine;
using System.Collections.Generic;
using ProyectSecret.Inventory;
using ProyectSecret.Inventory.Items;
using ProyectSecret.Components;

namespace ProyectSecret.Combat.SceneManagement
{
    [System.Serializable]
    public class SerializableWeaponData
    {
        public string weaponId;
        public float durability;
        public int hits;
    }

    [System.Serializable]
    public class SerializableInventoryData
    {
        public List<string> itemIds = new List<string>(); // IDs de ítems normales
        public List<SerializableWeaponData> weapons = new List<SerializableWeaponData>(); // Armas con estado
    }

    /// <summary>
    /// ScriptableObject para transferir datos persistentes del jugador entre escenas.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerPersistentData", menuName = "Combat/PlayerPersistentData")]
    public class PlayerPersistentData : ScriptableObject
    {
        [SerializeField] private int playerHealth;
        [SerializeField] private string equippedWeaponId;
        [SerializeField] private float equippedWeaponDurability;
        [SerializeField] private int equippedWeaponHits;
        [SerializeField] private SerializableInventoryData inventoryData = new SerializableInventoryData();

        public int PlayerHealth => playerHealth;
        public string EquippedWeaponId => equippedWeaponId;
        public float EquippedWeaponDurability => equippedWeaponDurability;
        public int EquippedWeaponHits => equippedWeaponHits;
        public SerializableInventoryData InventoryData => inventoryData;

        public void SaveFromPlayer(GameObject player)
        {
            var health = player.GetComponent<ProyectSecret.Components.HealthComponent>();
            if (health != null)
                playerHealth = health.CurrentValue;

            var equipment = player.GetComponent<ProyectSecret.Inventory.PlayerEquipmentController>();
            if (equipment != null && equipment.EquippedWeaponInstance != null)
            {
                equippedWeaponId = equipment.EquippedWeaponInstance.weaponData != null ? equipment.EquippedWeaponInstance.weaponData.Id : null;
                equippedWeaponDurability = equipment.EquippedWeaponInstance.currentDurability;
                equippedWeaponHits = equipment.EquippedWeaponInstance.hits;
            }
            else
            {
                equippedWeaponId = null;
                equippedWeaponDurability = 0;
                equippedWeaponHits = 0;
            }
            // Guardar inventario
            var playerInventory = player.GetComponent<ProyectSecret.Inventory.PlayerInventory>();
            if (playerInventory != null)
                inventoryData = playerInventory.ExportInventoryData();
        }

        public void ApplyToPlayer(GameObject player, ItemDatabase itemDatabase)
        {
            var health = player.GetComponent<Components.HealthComponent>();
            if (health != null)
                health.AffectValue(playerHealth - health.CurrentValue);

            var equipment = player.GetComponent<ProyectSecret.Inventory.PlayerEquipmentController>();
            if (equipment != null && !string.IsNullOrEmpty(equippedWeaponId))
            {
                var weaponItem = itemDatabase.GetItem(equippedWeaponId) as WeaponItem;
                if (weaponItem != null)
                {
                    var weaponInstance = new WeaponInstance(weaponItem);
                    weaponInstance.currentDurability = equippedWeaponDurability;
                    weaponInstance.hits = equippedWeaponHits;
                    equipment.EquipWeaponInstance(weaponInstance);
                }
            }
            // Restaurar inventario
            var playerInventory = player.GetComponent<ProyectSecret.Inventory.PlayerInventory>();
            if (playerInventory != null)
                playerInventory.ImportInventoryData(inventoryData, itemDatabase);
        }
    }
}
