using UnityEngine;

namespace ProyectSecret.Components
{
    /// <summary>
    /// Componente MonoBehaviour para exponer y gestionar HealthComponent en un GameObject.
    /// </summary>
    public class HealthComponentBehaviour : MonoBehaviour
    {
        [SerializeField]
        private HealthComponent health = new HealthComponent();
        /// <summary>
        /// Evento público para notificar cambios en la salud (útil para UI, logros, etc).
        /// </summary>
        public event System.Action<float> OnHealthChanged;

        public HealthComponent Health => health;

        private void Awake()
        {
            // Inicialización si es necesario
            if (health != null)
                health.OnHealthChanged += value => OnHealthChanged?.Invoke(value);
        }
    }
}
