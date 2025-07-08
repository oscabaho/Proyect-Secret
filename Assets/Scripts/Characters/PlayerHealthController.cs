using System;
using UnityEngine;
using Characters;
using Components;
using Stats;

namespace Characters
{
    /// <summary>
    /// Controlador de salud y muerte para el jugador. Hereda de HealthControllerBase.
    /// </summary>
    [RequireComponent(typeof(Components.HealthComponentBehaviour))]
    [RequireComponent(typeof(Components.StaminaComponentBehaviour))]
    public class PlayerHealthController : HealthControllerBase
    {
        private StaminaComponentBehaviour staminaBehaviour;
        private HealthComponentBehaviour healthBehaviour;
        public StaminaComponent Stamina => staminaBehaviour != null ? staminaBehaviour.Stamina : null;
        public HealthComponent Health => healthBehaviour != null ? healthBehaviour.Health : null;

        protected override void Awake()
        {
            base.Awake();
            healthBehaviour = GetComponent<HealthComponentBehaviour>();
            if (healthBehaviour == null)
                Debug.LogWarning("PlayerHealthController: No se encontró HealthComponentBehaviour.");
            staminaBehaviour = GetComponent<StaminaComponentBehaviour>();
            if (staminaBehaviour == null)
                Debug.LogWarning("PlayerHealthController: No se encontró StaminaComponentBehaviour.");
        }

        protected override void Death()
        {
            // Aquí puedes agregar lógica personalizada para la muerte del jugador
            Destroy(gameObject);
        }
    }
}
