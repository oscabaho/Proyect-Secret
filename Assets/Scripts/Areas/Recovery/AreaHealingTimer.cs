using UnityEngine;
using Components;
using Base;

public class AreaHealingTimer : MonoBehaviour
{
    private int healAmount;
    private float healInterval;
    private float timer = 0f;
    private HealthComponent healthComponent;

    public void Init(int healAmount, float interval)
    {
        this.healAmount = healAmount;
        this.healInterval = interval;
        healthComponent = null;
        var statHolder = GetComponent<BaseStatHolder>();
        if (statHolder != null)
            healthComponent = statHolder.Health;
        if (healthComponent == null)
        {
            Debug.LogWarning("AreaHealingTimer: No se encontró HealthComponent en el objeto (vía BaseStatHolder).");
            Destroy(this);
        }
    }

    private void Update()
    {
        if (healthComponent == null) return;
        timer += Time.deltaTime;
        if (timer >= healInterval)
        {
            healthComponent.AffectValue(healAmount);
            Debug.Log($"AreaHealingTimer: {gameObject.name} recupera {healAmount} de vida continua por área.");
            timer = 0f;
        }
    }
}