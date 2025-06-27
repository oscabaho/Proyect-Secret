namespace Interfaces
{
    /// <summary>
    /// Interfaz para ítems que pueden ser equipados en un slot.
    /// </summary>
    public interface IEquipable
    {
        /// <summary>
        /// Devuelve el tipo de slot donde puede ser equipado este ítem.
        /// </summary>
        EquipmentSlotType GetSlotType();
        void OnEquip(UnityEngine.GameObject user);
        void OnUnequip(UnityEngine.GameObject user);
    }

    public enum EquipmentSlotType
    {
        Weapon,
        Armor,
        Accessory
        // Puedes agregar más tipos según necesidad
    }
}
