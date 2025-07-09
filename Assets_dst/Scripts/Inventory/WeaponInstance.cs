using UnityEngine;
using ProyectSecret.Inventory.Items;

namespace ProyectSecret.Inventory
{
    [System.Serializable]
    public class WeaponInstance
    {
        public WeaponItem weaponData;
        public float currentDurability;
        public int hits;

        /// <summary>
        /// Evento público para notificar cuando el arma se rompe.
        /// </summary>
        public event System.Action<WeaponInstance> OnWeaponBroken;

        /// <summary>
        /// Evento público para notificar cuando la durabilidad cambia.
        /// </summary>
        public event System.Action<WeaponInstance> OnDurabilityChanged;

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
            OnDurabilityChanged?.Invoke(this);
            if (IsBroken())
                OnWeaponBroken?.Invoke(this);
        }

        public bool IsBroken() => currentDurability <= 0;
    }
}
