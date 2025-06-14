using System;
using UnityEngine;
using Base;
using Components;
using Stats;

/// <summary>
/// Enemigo que puede recibir daño y morir. Implementa Base.IDamageable.
/// </summary>
[RequireComponent(typeof(Collider))]
public class Enemy : MonoBehaviour, Base.IDamageable
{
    [Header("Stats")]
    [SerializeField] private HealthComponent health;
    public event Action OnDeath;
    public HealthComponent Health => health;

    private void Awake()
    {
        if (health == null)
            Debug.LogWarning("Enemy: HealthComponent no asignado.");
        if (GetComponent<Collider>() == null)
            Debug.LogWarning("Enemy: Falta Collider para detección de daño.");
    }

    public void TakeDamage(int amount)
    {
        if (health == null) return;
        health.AffectValue(-amount);
        if (health.CurrentValue <= 0)
        {
            OnDeath?.Invoke();
            Death();
        }
    }

    private void Death()
    {
        Destroy(gameObject);
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
