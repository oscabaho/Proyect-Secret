using UnityEngine;

namespace ProyectSecret.Components
{
    /// <summary>
    /// Componente MonoBehaviour para exponer y gestionar StaminaComponent en un GameObject.
    /// </summary>
    public class StaminaComponentBehaviour : MonoBehaviour
    {
        [SerializeField]
        private StaminaComponent stamina = new StaminaComponent();
        /// <summary>
        /// Evento público para notificar cambios en la stamina (útil para UI, logros, etc).
        /// </summary>
        public event System.Action<float> OnStaminaChanged;

        public virtual StaminaComponent Stamina => stamina;

        private void Awake()
        {
            // Inicialización si es necesario
            if (stamina != null)
                stamina.OnStaminaChanged += value => OnStaminaChanged?.Invoke(value);
        }
    }
}
