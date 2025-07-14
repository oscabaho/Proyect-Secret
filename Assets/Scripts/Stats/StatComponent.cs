using System;
using UnityEngine;

namespace ProyectSecret.Stats
{
    /// <summary>
    /// Componente base para estadísticas (vida, stamina, etc). Solo serializable si es necesario.
    /// </summary>
    [Serializable]
    public abstract class StatComponent
    {
        [field: NonSerialized]
        private event Action<StatComponent> _onValueChanged;
        [SerializeField, Tooltip("Valor máximo de la estadística")] private int maxValue = 100;
        [SerializeField, HideInInspector] private int currentValue;

        public int MaxValue { get { return maxValue; } }
        public int CurrentValue { get { return currentValue; } }

        // Exponer el evento solo para suscripción, no para asignación externa
        public event Action<StatComponent> OnValueChanged
        {
            add { _onValueChanged += value; }
            remove { _onValueChanged -= value; }
        }

        public virtual void Awake()
        {
            // Si currentValue es 0 al iniciar, iguala a maxValue
            if (currentValue <= 0)
                currentValue = maxValue;
        }

        public virtual void AffectValue(int value)
        {
            currentValue = Mathf.Clamp(currentValue + value, 0, maxValue);
            _onValueChanged?.Invoke(this);
        }
    }
}
