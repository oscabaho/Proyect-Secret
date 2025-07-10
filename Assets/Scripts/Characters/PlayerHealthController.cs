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
    [RequireComponent(typeof(ProyectSecret.Components.HealthComponentBehaviour))]
    [RequireComponent(typeof(ProyectSecret.Components.StaminaComponentBehaviour))]
    public class PlayerHealthController : HealthControllerBase
    {
        private ProyectSecret.Components.StaminaComponentBehaviour staminaBehaviour;
        private new ProyectSecret.Components.HealthComponentBehaviour healthBehaviour;
        public ProyectSecret.Components.StaminaComponent Stamina => staminaBehaviour != null ? staminaBehaviour.Stamina : null;
        public new ProyectSecret.Components.HealthComponent Health => healthBehaviour != null ? healthBehaviour.Health : null;

        protected override void Awake()
        {
            base.Awake();
            healthBehaviour = GetComponent<ProyectSecret.Components.HealthComponentBehaviour>();
            if (healthBehaviour == null)
                Debug.LogWarning("PlayerHealthController: No se encontró HealthComponentBehaviour.");
            staminaBehaviour = GetComponent<ProyectSecret.Components.StaminaComponentBehaviour>();
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
