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
        [Tooltip("Lista de IDs de los ítems en el inventario.")]
        public List<string> itemIds = new List<string>();
    }

    /// <summary>
    /// ScriptableObject para transferir datos persistentes del jugador entre escenas.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerPersistentData", menuName = "Combat/PlayerPersistentData")]
    public class PlayerPersistentData : ScriptableObject
    {
        [Header("Player Stats")]
        [SerializeField] public int playerHealth;
        [SerializeField] public int playerStamina;
        [SerializeField] public string equippedWeaponId;
        [SerializeField] public float equippedWeaponDurability;
        [SerializeField] public int equippedWeaponHits;
        [SerializeField] public SerializableInventoryData inventoryData = new SerializableInventoryData();

        [Header("State Flags")]
        [Tooltip("Se activa si el jugador vuelve de una derrota en combate.")]
        public bool CameFromDefeat = false;
        [Tooltip("Indica si hay una posición guardada para usar.")]
        public bool HasSavedPosition = false;
        public Vector3 LastPosition;

        public void SaveFromPlayer(GameObject player, bool savePosition = true)
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

            if (savePosition)
            {
                LastPosition = player.transform.position;
                HasSavedPosition = true;
            }
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

        /// <summary>
        /// Resetea todos los datos a sus valores por defecto.
        /// Útil para empezar una nueva partida.
        /// </summary>
        public void ResetData()
        {
            playerHealth = 0; // O tu valor inicial por defecto
            playerStamina = 0; // O tu valor inicial por defecto
            equippedWeaponId = null;
            equippedWeaponDurability = 0;
            equippedWeaponHits = 0;
            inventoryData = new SerializableInventoryData();
            CameFromDefeat = false;
            HasSavedPosition = false;
            LastPosition = Vector3.zero;
        }
    }
}
