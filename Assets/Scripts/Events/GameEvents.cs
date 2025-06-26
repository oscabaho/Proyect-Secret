using UnityEngine;

public class CharacterDeathEvent
{
    public GameObject Character { get; }
    public CharacterDeathEvent(GameObject character) { Character = character; }
}

public class CombatVictoryEvent
{
    public GameObject Enemy { get; }
    public CombatVictoryEvent(GameObject enemy) { Enemy = enemy; }
}

public class ItemUsedEvent
{
    public string ItemId { get; }
    public GameObject User { get; }
    public ItemUsedEvent(string itemId, GameObject user) { ItemId = itemId; User = user; }
}

public class InventoryChangedEvent
{
    public Inventory.PlayerInventory Inventory { get; }
    public InventoryChangedEvent(Inventory.PlayerInventory inventory) { Inventory = inventory; }
}
