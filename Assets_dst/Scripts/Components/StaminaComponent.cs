using System;
using ProyectSecret.Stats;
using UnityEngine;

namespace ProyectSecret.Components
{
    [Serializable]
    public class StaminaComponent : StatComponent
    {
        public int MaxStamina => MaxValue;
        public virtual float CurrentStamina => CurrentValue;

        public StaminaComponent(int maxValue = 100)
        {
            // No hay constructor base con parámetros, así que asignamos aquí
            var field = typeof(StatComponent).GetField("maxValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
                field.SetValue(this, maxValue);
        }

        /// <summary>
        /// Evento público específico para cambios en la stamina (útil para UI, logros, etc).
        /// </summary>
        public event Action<float> OnStaminaChanged;

        public void UseStamina(int amount)
        {
            int cost = Math.Max(0, amount);
            AffectValue(-cost);
            OnStaminaChanged?.Invoke(CurrentStamina);
        }

        public void RecoverStamina(int amount)
        {
            int gain = Math.Max(0, amount);
            AffectValue(gain);
            OnStaminaChanged?.Invoke(CurrentStamina);
        }

        public void SetStamina(int value)
        {
            int clamped = Mathf.Clamp(value, 0, MaxValue);
            int diff = clamped - CurrentValue;
            AffectValue(diff);
            OnStaminaChanged?.Invoke(CurrentStamina);
        }
    }
}
