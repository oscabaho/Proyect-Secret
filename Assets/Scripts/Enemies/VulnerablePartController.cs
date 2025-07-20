using UnityEngine;
using ProyectSecret.Interfaces;
using System.Collections; // Necesario para la corutina

namespace ProyectSecret.Enemies
{
    public class VulnerablePartController : MonoBehaviour, IDamageable
    {
        [SerializeField] private float vulnerableTime = 3f;
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
            // Usamos una corutina para gestionar el ciclo de vida.
            StartCoroutine(LifecycleRoutine());
        }

        private IEnumerator LifecycleRoutine()
        {
            // Esperar el tiempo de vulnerabilidad.
            yield return new WaitForSeconds(vulnerableTime);

            // Desactivar la vulnerabilidad y destruir el objeto.
            isVulnerable = false;
            // Aquí puedes añadir una animación de desaparición antes de destruir.
            Destroy(gameObject);
        }

        public void TakeDamage(int amount)
        {
            if (isVulnerable && enemyHealth != null)
            {
                enemyHealth.TakeDamage(amount);
            }
        }
    }
}
