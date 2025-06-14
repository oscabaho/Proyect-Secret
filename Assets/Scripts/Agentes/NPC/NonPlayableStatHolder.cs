using UnityEngine;
using System;
using Components;

/// <summary>
/// Portador no jugable: solo emplea vida, destruido al morir.
/// </summary>
public class NonPlayableStatHolder : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private HealthComponent health;
    public event Action OnDeath;
    public HealthComponent Health => health;

    private void Update()
    {
        if (health != null && health.CurrentValue <= 0)
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }

    public void CheckDeath()
    {
        if (health != null && health.CurrentValue <= 0)
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }
}
