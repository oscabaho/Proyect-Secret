using System;
using UnityEngine;
using ProyectSecret.Components;
using ProyectSecret.Stats;

namespace ProyectSecret.Characters
{
    /// <summary>
    /// Controlador de salud y muerte para el jugador. Hereda de HealthControllerBase.
    /// </summary>
    [RequireComponent(typeof(HealthComponentBehaviour))]
    [RequireComponent(typeof(StaminaComponentBehaviour))]
    public class PlayerHealthController : HealthControllerBase
    {
        [SerializeField] private StaminaComponentBehaviour staminaBehaviour;
        public StaminaComponent Stamina => staminaBehaviour != null ? staminaBehaviour.Stamina : ProyectSecret.Components.NullObjects.NullStaminaComponent.Instance;

        /// <summary>
        /// Evento público para notificar la muerte del jugador a otros sistemas (UI, logros, etc).
        /// </summary>
        public event System.Action OnPlayerDeath;

        protected override void Awake()
        {
            base.Awake();
            if (staminaBehaviour == null)
                staminaBehaviour = GetComponent<StaminaComponentBehaviour>();
            if (staminaBehaviour == null)
                Debug.LogWarning("PlayerHealthController: No se encontró StaminaComponentBehaviour.");
        }

        protected override void Death()
        {
            OnPlayerDeath?.Invoke();
            Destroy(gameObject);
        }
    }

    // NullStaminaComponent ahora está en ProyectSecret.Components.NullObjects
}
