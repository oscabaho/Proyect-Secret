using System;
using UnityEngine;
using ProyectSecret.Components;
using ProyectSecret.Stats;
using ProyectSecret.Characters;

namespace ProyectSecret.Characters.Enemies
{
    /// <summary>
    /// Controlador de salud y muerte para enemigos únicos. Hereda de HealthControllerBase.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(HealthComponentBehaviour))]
    public class EnemyHealthController : HealthControllerBase
    {
        /// <summary>
        /// Evento público para notificar la muerte del enemigo a otros sistemas (UI, logros, etc).
        /// </summary>
        public event System.Action OnEnemyDeath;

        protected override void Death()
        {
            OnEnemyDeath?.Invoke();
            Destroy(gameObject);
        }
    }
}
