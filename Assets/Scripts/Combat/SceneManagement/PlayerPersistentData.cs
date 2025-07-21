using UnityEngine;
using System.Collections.Generic;
using ProyectSecret.Inventory;
using ProyectSecret.Inventory.Items;

namespace ProyectSecret.Combat.SceneManagement
{
    [System.Serializable]
    public class SerializableInventoryData
    {
        // Ahora solo guardamos los IDs de todos los ítems, incluidas las armas.
        public List<string> itemIds = new List<string>();
    }

    /// <summary>
    /// ScriptableObject para transferir datos persistentes del jugador entre escenas.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerPersistentData", menuName = "Combat/PlayerPersistentData")]
    public class PlayerPersistentData : ScriptableObject
    {
        [SerializeField] private int playerHealth;
        [SerializeField] private int playerStamina;
        [HideInInspector] public bool CameFromDefeat = false;
        [SerializeField] private string equippedWeaponId;
        [SerializeField] private float equippedWeaponDurability;
        [SerializeField] private int equippedWeaponHits;
        [SerializeField] private SerializableInventoryData inventoryData = new SerializableInventoryData();

        public int PlayerHealth => playerHealth;
        public int PlayerStamina => playerStamina;
        public string EquippedWeaponId => equippedWeaponId;
        public float EquippedWeaponDurability => equippedWeaponDurability;
        public int EquippedWeaponHits => equippedWeaponHits;
        public SerializableInventoryData InventoryData => inventoryData;

        public void SaveFromPlayer(GameObject player)
        {
            var healthComp = player.GetComponent<ProyectSecret.Components.HealthComponentBehaviour>();
            if (healthComp != null)
                playerHealth = healthComp.Health.CurrentValue;

            var staminaComp = player.GetComponent<ProyectSecret.Components.StaminaComponentBehaviour>();
            if (staminaComp != null)
                playerStamina = staminaComp.Stamina.CurrentValue;

            var equipment = player.GetComponent<ProyectSecret.Inventory.PlayerEquipmentController>();
            if (equipment != null && equipment.EquippedWeaponInstance != null)
            {
                equippedWeaponId = equipment.EquippedWeaponInstance.WeaponData != null ? equipment.EquippedWeaponInstance.WeaponData.Id : null;
                equippedWeaponDurability = equipment.EquippedWeaponInstance.CurrentDurability;
                equippedWeaponHits = equipment.EquippedWeaponInstance.Hits;
            }
            else
            {
                equippedWeaponId = null;
                equippedWeaponDurability = 0;
                equippedWeaponHits = 0;
            }
            
            var playerInventory = player.GetComponent<ProyectSecret.MonoBehaviours.Player.PlayerInventory>();
            if (playerInventory != null)
                inventoryData = playerInventory.ExportInventoryData();
        }

        public void ApplyToPlayer(GameObject player, ItemDatabase itemDatabase)
        {
            var healthComp = player.GetComponent<ProyectSecret.Components.HealthComponentBehaviour>();
            if (healthComp != null)
                healthComp.Health.SetValue(playerHealth);

            var staminaComp = player.GetComponent<ProyectSecret.Components.StaminaComponentBehaviour>();
            if (staminaComp != null)
                staminaComp.Stamina.SetValue(playerStamina);

            var equipment = player.GetComponent<ProyectSecret.Inventory.PlayerEquipmentController>();
            if (equipment != null && !string.IsNullOrEmpty(equippedWeaponId))
            {
                var weaponItem = itemDatabase.GetItem(equippedWeaponId) as WeaponItem;
                if (weaponItem != null)
                {
                    var weaponInstance = new WeaponInstance(weaponItem);
                    weaponInstance.SetDurability(equippedWeaponDurability);
                    weaponInstance.SetHits(equippedWeaponHits);
                    equipment.EquipWeaponInstance(weaponInstance);
                }
            }
            
            var playerInventory = player.GetComponent<ProyectSecret.MonoBehaviours.Player.PlayerInventory>();
            if (playerInventory != null)
                playerInventory.ImportInventoryData(inventoryData, itemDatabase);
        }
    }
}
