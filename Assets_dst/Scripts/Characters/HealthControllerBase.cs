using System;
using UnityEngine;
using ProyectSecret.Components;
using ProyectSecret.Stats;

namespace ProyectSecret.Characters
{
    /// <summary>
    /// Clase abstracta base para controladores de salud y muerte de entidades.
    /// Gestiona vida, daño, debuffs y eventos de muerte.
    /// </summary>
    public abstract class HealthControllerBase : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] protected HealthComponentBehaviour healthBehaviour;
        public event Action OnDeath;
        public event Func<int, int> OnPreTakeDamage;
        public HealthComponent Health => healthBehaviour != null ? healthBehaviour.Health : null;

        protected virtual void Awake()
        {
            if (healthBehaviour == null)
                healthBehaviour = GetComponent<HealthComponentBehaviour>();
            if (healthBehaviour == null)
                Debug.LogWarning($"{GetType().Name}: HealthComponentBehaviour no asignado.");
        }

        public virtual void TakeDamage(int amount)
        {
            int finalAmount = OnPreTakeDamage != null ? OnPreTakeDamage.Invoke(amount) : amount;
            if (Health != null)
            {
                Health.AffectValue(-finalAmount);
                if (Health.CurrentValue <= 0)
                {
                    OnDeath?.Invoke();
                    Death();
                }
            }
        }

        protected abstract void Death();
    }
}
