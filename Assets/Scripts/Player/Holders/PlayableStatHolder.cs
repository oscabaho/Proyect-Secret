using UnityEngine;
using Components;
using Stats;
using Base;
using System;

namespace Holders
{
    /// <summary>
    /// Portador jugable: puede tener vida y/o stamina por asociación. Implementa Base.IDamageable.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class PlayableStatHolder : MonoBehaviour, Base.IDamageable
    {
        [Header("Stats")]
        [SerializeField] private HealthComponent health;
        [SerializeField] private StaminaComponent stamina;
        [SerializeField] private GameObject explosionVFXPrefab;
        [SerializeField] private GameObject projectilePrefab;

        public event Action OnDeath;
        public HealthComponent Health => health;
        public StaminaComponent Stamina => stamina;

        private void Awake()
        {
            if (health == null)
                Debug.LogWarning("PlayableStatHolder: HealthComponent no asignado.");
            if (GetComponent<Collider>() == null)
                Debug.LogWarning("PlayableStatHolder: Falta Collider para detección de daño.");
        }

        public void Initialize(HealthComponent health = null, StaminaComponent stamina = null)
        {
            if (health != null) this.health = health;
            if (stamina != null) this.stamina = stamina;
        }

        public void TakeDamage(int amount)
        {
            health?.AffectValue(-amount);
            if (health != null && health.CurrentValue <= 0)
            {
                OnDeath?.Invoke();
                // Aquí puedes agregar lógica de muerte del jugador
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
}
