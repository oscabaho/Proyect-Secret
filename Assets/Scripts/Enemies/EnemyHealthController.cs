using System;
using UnityEngine;
using Characters;
using Components;
using Stats;

namespace Enemies
{
    /// <summary>
    /// Controlador de salud y muerte para enemigos únicos. Hereda de HealthControllerBase.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class EnemyHealthController : HealthControllerBase
    {
        protected override void Death()
        {
            // Aquí puedes agregar animaciones, efectos, recompensas, etc.
            Destroy(gameObject);
        }
    }
}
