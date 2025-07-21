using UnityEngine;
using ProyectSecret.Utils;
using ProyectSecret.VFX;

namespace ProyectSecret.Managers
{
    /// <summary>
    /// Manager centralizado para gestionar efectos visuales y de sonido (VFX/SFX).
    /// Utiliza pools de objetos para un rendimiento óptimo.
    /// </summary>
    public class VFXManager : MonoBehaviour
    {
        public static VFXManager Instance { get; private set; }

        [Header("Pool de Partículas de Impacto")]
        [SerializeField] private GameObject impactParticlePrefab;
        [SerializeField] private int impactParticlePoolSize = 20;
        private ObjectPool<PooledParticleSystem> impactParticlePool;

        private void Awake()
        {
            // Implementación del patrón Singleton
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Inicializamos el pool de partículas que este manager gestionará
            if (impactParticlePrefab != null)
                impactParticlePool = new ObjectPool<PooledParticleSystem>(impactParticlePrefab, impactParticlePoolSize, transform);
        }

        /// <summary>
        /// Reproduce un efecto de impacto en una posición específica.
        /// </summary>
        /// <param name="position">El punto en el mundo donde se reproducirá el efecto.</param>
        /// <param name="sound">El clip de sonido a reproducir.</param>
        /// <param name="volume">El volumen del sonido.</param>
        public void PlayImpactEffect(Vector3 position, AudioClip sound, float volume = 1.0f)
        {
            // Reproducir sonido en el punto de impacto
            if (sound != null)
                AudioSource.PlayClipAtPoint(sound, position, volume);

            // Obtener y activar una partícula del pool
            if (impactParticlePool != null)
            {
                GameObject particleInstance = impactParticlePool.Get();
                if (particleInstance != null)
                {
                    particleInstance.transform.position = position;
                    particleInstance.SetActive(true);
                }
            }
        }
    }
}
