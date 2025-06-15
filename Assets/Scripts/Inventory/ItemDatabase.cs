using System.Collections.Generic;

public static class ItemDatabase
{
    private static readonly Dictionary<string, IUsableItem> items = new Dictionary<string, IUsableItem>();

    static ItemDatabase()
    {
        // Registrar ítems aquí
        items["pocion_salud"] = new HealingItem("pocion_salud", 25);
        // Agrega más ítems según sea necesario
    }

    public static IUsableItem GetItem(string id)
    {
        items.TryGetValue(id, out var item);
        return item;
    }
}
