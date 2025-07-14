using System;
using UnityEngine;
using ProyectSecret.Components;
using ProyectSecret.Events; // Add this line or the correct namespace for GameEventBus

namespace Characters
{

    /// <summary>
    /// Clase abstracta base para controladores de salud y muerte de entidades.
    /// Gestiona vida, daño, debuffs y eventos de muerte.
    /// </summary>
    [RequireComponent(typeof(ProyectSecret.Components.HealthComponentBehaviour))]
    public abstract class HealthControllerBase : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] protected ProyectSecret.Components.HealthComponentBehaviour healthBehaviour;
        public event Action OnDeath;
        public event Func<int, int> OnPreTakeDamage;
        public ProyectSecret.Components.HealthComponent Health => healthBehaviour != null ? healthBehaviour.Health : null;

        protected virtual void Awake()
        {
            if (healthBehaviour == null)
                healthBehaviour = GetComponent<ProyectSecret.Components.HealthComponentBehaviour>();
            if (healthBehaviour == null)
                Debug.LogWarning($"{GetType().Name}: HealthComponentBehaviour no asignado.");
        }
        public virtual void TakeDamage(int amount)
        {
            if (Health == null) return;
            int finalAmount = amount;
            if (OnPreTakeDamage != null)
            {
                foreach (Func<int, int> handler in OnPreTakeDamage.GetInvocationList())
                {
                    finalAmount = handler(finalAmount);
                }
            }
            Health.AffectValue(-finalAmount);
            if (Health.CurrentValue <= 0)
            {
                OnDeath?.Invoke();
                GameEventBus.Instance.Publish(new CharacterDeathEvent(gameObject));
                Death();
            }
        }

        protected abstract void Death();
    }
}
