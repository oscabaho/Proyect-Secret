using Characters;
using UnityEngine;

public class HealingItem : IUsableItem
{
    private readonly string id;
    private readonly int healAmount;

    public HealingItem(string id, int healAmount)
    {
        this.id = id;
        this.healAmount = healAmount;
    }

    public void Use(GameObject user)
    {
        var health = user.GetComponent<PlayerHealthController>();
        if (health != null && health.Health != null)
        {
            health.Health.AffectValue(healAmount);
            Debug.Log($"Curado {healAmount} puntos de vida.");
        }
        else
        {
            Debug.LogWarning("No se pudo curar: PlayerHealthController o HealthComponent no encontrado.");
        }
    }

    public string GetId() => id;
}
