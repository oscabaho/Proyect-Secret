using UnityEngine;
using ProyectSecret.Enemies;

namespace MonoBehaviours.Enemies
{
    /// <summary>
    /// Controlador principal del enemigo. Orquesta IA, ataques y referencia a los sub-componentes.
    /// </summary>
    [RequireComponent(typeof(EnemyHealthController))]
    [RequireComponent(typeof(EnemyAttackController))]
    public class Enemy : MonoBehaviour
    {
        private EnemyHealthController healthController;
        private EnemyAttackController attackController;

        private void Awake()
        {
            healthController = GetComponent<EnemyHealthController>();
            attackController = GetComponent<EnemyAttackController>();

            if (healthController == null)
                Debug.LogError("Enemy: No se encontró EnemyHealthController.");
            if (attackController == null)
                Debug.LogError("Enemy: No se encontró EnemyAttackController.");
        }

        public void TakeDamage(int amount)
        {
            healthController?.TakeDamage(amount);
        }

        // Puedes agregar aquí la lógica de IA que llame a attackController.StartPhase1(), etc.
    }
}
