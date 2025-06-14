using UnityEngine;
using System;
using Components;
using Stats;
using Base;

/// <summary>
/// Portador no jugable: solo emplea vida, destruido al morir. Implementa Base.IDamageable.
/// </summary>
[RequireComponent(typeof(Collider))]
public class NonPlayableStatHolder : MonoBehaviour, Base.IDamageable
{
    [Header("Stats")]
    [SerializeField] private HealthComponent health;
    public event Action OnDeath;
    public HealthComponent Health => health;

    private void Awake()
    {
        if (health == null)
            Debug.LogWarning("NonPlayableStatHolder: HealthComponent no asignado.");
        if (GetComponent<Collider>() == null)
            Debug.LogWarning("NonPlayableStatHolder: Falta Collider para detección de daño.");
    }

    private void Update()
    {
        if (health != null && health.CurrentValue <= 0)
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int amount)
    {
        if (health == null) return;
        health.AffectValue(-amount);
        if (health.CurrentValue <= 0)
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }

    // Permite suscripción externa al evento de muerte
    public void SubscribeOnDeath(Action callback)
    {
        OnDeath += callback;
    }
    public void UnsubscribeOnDeath(Action callback)
    {
        OnDeath -= callback;
    }
}
