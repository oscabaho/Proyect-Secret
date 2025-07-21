using UnityEngine;
using ProyectSecret.MonoBehaviours.Player;

namespace ProyectSecret.Events
{
    // =============================
    //  EVENTOS DE PERSONAJE Y COMBATE
    // =============================

    /// <summary>
    /// Evento publicado cuando el jugador gana un combate.
    /// </summary>
    public class CombatVictoryEvent
    {
        public GameObject Enemy { get; }
        public CombatVictoryEvent(GameObject enemy) { Enemy = enemy; }
    }

    /// <summary>
    /// Evento publicado cuando el jugador realiza una acción que consume stamina
    /// y que debería reiniciar el delay de recuperación de la misma.
    /// </summary>
    public class PlayerActionUsedStaminaEvent
    {
        public GameObject Player { get; }
        public int StaminaCost { get; }

        public PlayerActionUsedStaminaEvent(GameObject player, int staminaCost)
        {
            Player = player;
            StaminaCost = staminaCost;
        }
    }

    /// <summary>
    /// Evento publicado cuando el jugador muere.
    /// </summary>
    public class PlayerDiedEvent
    {
        public GameObject PlayerObject { get; }

        public PlayerDiedEvent(GameObject playerObject)
        {
            PlayerObject = playerObject;
        }
    }

    /// <summary>
    /// Evento que representa la muerte de una entidad (personaje, enemigo, etc.).
    /// </summary>
    public class CharacterDeathEvent
    {
        public GameObject Entity { get; }

        public CharacterDeathEvent(GameObject entity)
        {
            Entity = entity;
        }
    }

    /// <summary>
    /// Evento publicado cuando el GameObject del jugador es instanciado en una escena.
    /// </summary>
    public class PlayerSpawnedEvent
    {
        public GameObject PlayerObject { get; }

        public PlayerSpawnedEvent(GameObject playerObject)
        {
            PlayerObject = playerObject;
        }
    }

    // =============================
    //  EVENTOS DE INVENTARIO Y OBJETOS
    // =============================

    /// <summary>
    /// Evento publicado cuando se usa un ítem (poción, arma, etc).
    /// </summary>
    public class ItemUsedEvent
    {
        public string ItemId { get; }
        public GameObject User { get; }
        public ItemUsedEvent(string itemId, GameObject user) { ItemId = itemId; User = user; }
    }

    /// <summary>
    /// Evento publicado cuando el inventario del jugador cambia (añadir, quitar, usar ítem).
    /// </summary>
    public class InventoryChangedEvent
    {
        public PlayerInventory Inventory { get; }
        public InventoryChangedEvent(PlayerInventory inventory) { Inventory = inventory; }
    }

    // Puedes agrupar más eventos aquí según el dominio (misiones, UI, etc)
}
