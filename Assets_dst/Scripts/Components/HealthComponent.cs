using System;
using ProyectSecret.Stats;
using UnityEngine;

namespace ProyectSecret.Components
{
    [Serializable]
    public class HealthComponent : StatComponent
    {
        /// <summary>
        /// Evento público específico para cambios en la vida (útil para UI, logros, etc).
        /// </summary>
        public event Action<float> OnHealthChanged;

        public override void AffectValue(int value)
        {
            base.AffectValue(value);
            Debug.Log($"HealthComponent: Vida actual = {CurrentValue}");
            OnHealthChanged?.Invoke(CurrentValue);
        }
    }
}
