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
    [RequireComponent(typeof(Enemies.EnemyHealthController))]
    public class PlayerHealthController : HealthControllerBase
    {
        [SerializeField] private StaminaComponent stamina;
        public StaminaComponent Stamina => stamina;

        protected override void Death()
        {
            // Aquí puedes agregar lógica personalizada para la muerte del jugador
            Destroy(gameObject);
        }
    }
}
