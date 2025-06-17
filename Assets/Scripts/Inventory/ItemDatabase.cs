using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using Inventory;

namespace Inventory
{
    public static class ItemDatabase
    {
        private static readonly Dictionary<string, IUsableItem> items = new Dictionary<string, IUsableItem>();

        static ItemDatabase()
        {
            // Registrar ítems de curación
            items["pocion_salud"] = new HealingItem("pocion_salud", 25);

            // Registrar armas
            items["hacha"] = new WeaponItem("hacha", "Hacha", "Un hacha pesada y poderosa.", 30, 0.7f);
            items["espada"] = new WeaponItem("espada", "Espada", "Una espada equilibrada.", 20, 1.0f);
            items["dagas"] = new WeaponItem("dagas", "Dagas", "Dagas ligeras y rápidas.", 12, 1.7f);
        }

        public static IUsableItem GetItem(string id)
        {
            items.TryGetValue(id, out var item);
            return item;
        }
    }
}
