using System;
using UnityEngine;
using ProyectSecret.Components;
using ProyectSecret.Stats;
using Characters;

namespace ProyectSecret.Characters
{
    /// <summary>
    /// Controlador de salud y muerte para el jugador. Hereda de HealthControllerBase.
    /// </summary>
    [RequireComponent(typeof(HealthComponentBehaviour))]
    [RequireComponent(typeof(StaminaComponentBehaviour))]
    public class PlayerHealthController : HealthControllerBase
    {
        private StaminaComponentBehaviour staminaBehaviour;
        private new HealthComponentBehaviour healthBehaviour;
        public StaminaComponent Stamina => staminaBehaviour != null ? staminaBehaviour.Stamina : null;
        public new HealthComponent Health => healthBehaviour != null ? healthBehaviour.Health : null;

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
