using ProyectSecret.Inventory.Items;

namespace ProyectSecret.Inventory
{
    /// <summary>
    /// Representa la instancia de un arma equipada, con estado propio (durabilidad, golpes, etc).
    /// </summary>
    public class WeaponInstance
    {
        public WeaponItem WeaponData { get; private set; }
        public float CurrentDurability { get; private set; }
        public int Hits { get; private set; }

        public void SetDurability(float value)
        {
            CurrentDurability = value;
        }

        public void SetHits(int value)
        {
            Hits = value;
        }

        public WeaponInstance(WeaponItem weaponData)
        {
            WeaponData = weaponData;
            CurrentDurability = weaponData.MaxDurability;
            Hits = 0;
        }
    }
}
