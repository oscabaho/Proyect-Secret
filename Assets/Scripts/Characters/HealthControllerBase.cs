using System;
using UnityEngine;
using Components;
using Stats;

namespace Characters
{
    /// <summary>
    /// Clase abstracta base para controladores de salud y muerte de entidades.
    /// Gestiona vida, daño, debuffs y eventos de muerte.
    /// </summary>
    public abstract class HealthControllerBase : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] protected HealthComponent health;
        public event Action OnDeath;
        public event Func<int, int> OnPreTakeDamage;
        public HealthComponent Health => health;

        protected virtual void Awake()
        {
            if (health == null)
                Debug.LogWarning($"{GetType().Name}: HealthComponent no asignado.");
        }

        public virtual void TakeDamage(int amount)
        {
            if (health == null) return;
            int finalAmount = amount;
            if (OnPreTakeDamage != null)
            {
                foreach (Func<int, int> handler in OnPreTakeDamage.GetInvocationList())
                {
                    finalAmount = handler(finalAmount);
                }
            }
            health.AffectValue(-finalAmount);
            if (health.CurrentValue <= 0)
            {
                OnDeath?.Invoke();
                Death();
            }
        }

        protected abstract void Death();
    }
}
