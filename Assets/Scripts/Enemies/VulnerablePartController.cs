using UnityEngine;

namespace ProyectSecret.Enemies
{
    using ProyectSecret.Interfaces;

    public class VulnerablePartController : MonoBehaviour, IDamageable
    {
        public event System.Action OnDeath;
        public float vulnerableTime = 3f;
        private bool isVulnerable = true;
        private EnemyHealthController enemyHealth;

        /// <summary>
        /// Inicializa la parte vulnerable con una referencia a la salud del enemigo.
        /// Debe ser llamado por quien lo instancia.
        /// </summary>
        public void Initialize(EnemyHealthController healthController)
        {
            enemyHealth = healthController;
        }

        private void Start()
        {
            Invoke(nameof(DisableVulnerability), vulnerableTime);
        }

        public void TakeDamage(int amount)
        {
            if (isVulnerable && enemyHealth != null)
            {
                enemyHealth.TakeDamage(amount);
            }
        }

        private void DisableVulnerability()
        {
            isVulnerable = false;
            // Aquí puedes ocultar o animar la retirada de la parte vulnerable
            OnDeath?.Invoke();
        }
    }
}
