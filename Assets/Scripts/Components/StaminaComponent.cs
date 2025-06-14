using System;
using Base;
using UnityEngine;

namespace Components
{
    [Serializable]
    public class StaminaComponent : StatComponent
    {
        public int MaxStamina => MaxValue;
        public int CurrentStamina => CurrentValue;

        public StaminaComponent(int maxValue = 100)
        {
            // No hay constructor base con parámetros, así que asignamos aquí
            var field = typeof(StatComponent).GetField("maxValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
                field.SetValue(this, maxValue);
        }

        public void UseStamina(int amount)
        {
            int cost = Math.Max(0, amount);
            AffectValue(-cost);
        }

        public void RecoverStamina(int amount)
        {
            int gain = Math.Max(0, amount);
            AffectValue(gain);
        }

        public void SetStamina(int value)
        {
            int clamped = Mathf.Clamp(value, 0, MaxValue);
            int diff = clamped - CurrentValue;
            AffectValue(diff);
        }
    }
}
