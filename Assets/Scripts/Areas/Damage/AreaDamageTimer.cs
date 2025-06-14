using UnityEngine;
using Components;
using Base;

public class AreaDamageTimer : MonoBehaviour
{
    private int damage; // Solo privado
    private float damageInterval;
    private float timer = 0f;
    private HealthComponent healthComponent;

    // Inicialización desde AreaDamage
    public void Init(int damage, float interval)
    {
        this.damage = damage;
        this.damageInterval = interval;
    }

    private void Awake()
    {
        healthComponent = null;
        var statHolder = GetComponent<BaseStatHolder>();
        if (statHolder != null)
            healthComponent = statHolder.Health;
        if (healthComponent == null)
        {
            Debug.LogWarning("AreaDamageTimer: No se encontró HealthComponent en el objeto (vía BaseStatHolder).");
            enabled = false;
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= damageInterval)
        {
            ApplyDamage();
            timer = 0f;
        }
    }

    private void ApplyDamage()
    {
        if (healthComponent != null)
        {
            healthComponent.AffectValue(-damage);
            Debug.Log($"AreaDamageTimer: {gameObject.name} recibe {damage} de daño continuo por área.");
        }
    }
}
