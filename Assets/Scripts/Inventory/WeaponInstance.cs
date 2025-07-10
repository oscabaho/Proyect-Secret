using ProyectSecret.Inventory.Items;

namespace ProyectSecret.Inventory
{
    /// <summary>
    /// Representa la instancia de un arma equipada, con estado propio (durabilidad, golpes, etc).
    /// </summary>
    public class WeaponInstance
    {
        public WeaponItem weaponData;
        public float currentDurability;
        public int hits;

        public WeaponInstance(WeaponItem weaponData)
        {
            this.weaponData = weaponData;
            this.currentDurability = weaponData.MaxDurability;
            this.hits = 0;
        }
    }
}
