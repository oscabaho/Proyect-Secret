using System.Collections.Generic;
using UnityEngine;
using Components;

namespace Base
{
    public abstract class BaseStatHolder
    {
        [Header("Stats")]
        [SerializeField] private HealthComponent health;

        public HealthComponent Health => health;

        protected virtual void Awake()
        {
            health.Awake();
        }

        public virtual void TakeDamage(int amount)
        {
            health?.AffectValue(-amount);
        }
    }
}