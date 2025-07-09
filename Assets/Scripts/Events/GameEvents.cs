using UnityEngine;

// =============================
//  EVENTOS DE PERSONAJE Y COMBATE
// =============================

namespace ProyectSecret.Events
{
    /// <summary>
    /// Evento publicado cuando un personaje muere (jugador, enemigo, NPC).
    /// </summary>
    public class CharacterDeathEvent
    {
        public GameObject Character { get; }
        public CharacterDeathEvent(GameObject character) { Character = character; }
    }

    /// <summary>
    /// Evento publicado cuando el jugador gana un combate.
    /// </summary>
    public class CombatVictoryEvent
    {
        public GameObject Enemy { get; }
        public CombatVictoryEvent(GameObject enemy) { Enemy = enemy; }
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
        public ProyectSecret.Inventory.PlayerInventory Inventory { get; }
        public InventoryChangedEvent(ProyectSecret.Inventory.PlayerInventory inventory) { Inventory = inventory; }
    }

// Puedes agrupar más eventos aquí según el dominio (misiones, UI, etc)
