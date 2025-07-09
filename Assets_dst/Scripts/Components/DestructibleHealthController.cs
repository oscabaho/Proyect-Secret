using UnityEngine;
using ProyectSecret.Components;
using ProyectSecret.Stats;
using ProyectSecret.Characters;

namespace ProyectSecret.Components
{
    /// <summary>
    /// Controlador de salud y muerte para objetos destruibles o NPCs simples. Hereda de HealthControllerBase.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(HealthComponentBehaviour))]
    public class DestructibleHealthController : HealthControllerBase
    {
        /// <summary>
        /// Evento público para notificar la destrucción del objeto a otros sistemas (UI, logros, etc).
        /// </summary>
        public event System.Action OnDestruction;

        protected override void Death()
        {
            OnDestruction?.Invoke();
            Destroy(gameObject);
        }
    }
}
