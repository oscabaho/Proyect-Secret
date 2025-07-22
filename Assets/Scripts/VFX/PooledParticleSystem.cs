using System.Collections;
using UnityEngine;
using ProyectSecret.Utils;

namespace ProyectSecret.VFX
{
    /// <summary>
    /// Componente para un sistema de partículas gestionado por un ObjectPool.
    /// Se encarga de devolverse al pool cuando termina su efecto.
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class PooledParticleSystem : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        /// <summary>
        /// El pool de objetos al que pertenece esta instancia.
        /// </summary>
        public ObjectPool<PooledParticleSystem> Pool { get; set; }

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void OnEnable()
        {
            // Al activarse desde el pool, iniciamos la corrutina que lo devolverá.
            StartCoroutine(ReturnToPoolWhenFinished());
        }

        private IEnumerator ReturnToPoolWhenFinished()
        {
            // Esperamos hasta que el sistema de partículas (y todos sus hijos) haya terminado.
            yield return new WaitWhile(() => _particleSystem.IsAlive(true));

            // Una vez terminado, nos devolvemos al pool.
            Pool?.Return(gameObject);
        }
    }
}
