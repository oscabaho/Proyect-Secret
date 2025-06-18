using UnityEngine;

namespace Inventory
{
    [System.Serializable]
    public class WeaponInstance
    {
        public WeaponItem weaponData;
        public float currentDurability;
        public int hits;

        public WeaponInstance(WeaponItem data)
        {
            weaponData = data;
            currentDurability = data.MaxDurability;
            hits = 0;
        }

        public void RegisterHit()
        {
            hits++;
            float t = Mathf.Clamp01((float)hits / weaponData.MaxDurability);
            currentDurability = weaponData.MaxDurability * weaponData.DurabilityCurve.Evaluate(t);
        }

        public bool IsBroken() => currentDurability <= 0;
    }
}
