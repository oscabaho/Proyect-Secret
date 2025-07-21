using UnityEngine;
using ProyectSecret.Utils;
using ProyectSecret.VFX;
using System.Collections;

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

        /// <summary>
        /// Inicia un efecto de desvanecimiento y destrucción en un objeto.
        /// </summary>
        /// <param name="targetObject">El GameObject que se desvanecerá y destruirá.</param>
        /// <param name="fadeDuration">La duración del desvanecimiento.</param>
        public void PlayFadeAndDestroyEffect(GameObject targetObject, float fadeDuration)
        {
            if (targetObject == null) return;
            StartCoroutine(FadeAndDestroyRoutine(targetObject, fadeDuration));
        }

        private IEnumerator FadeAndDestroyRoutine(GameObject targetObject, float fadeDuration)
        {
            Renderer[] renderers = targetObject.GetComponentsInChildren<Renderer>();
            var propBlock = new MaterialPropertyBlock();
            int colorID = Shader.PropertyToID("_Color");

            float timer = 0f;
            while (timer < fadeDuration)
            {
                float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
                foreach (var rend in renderers)
                {
                    rend.GetPropertyBlock(propBlock);
                    propBlock.SetColor(colorID, new Color(rend.material.color.r, rend.material.color.g, rend.material.color.b, alpha));
                    rend.SetPropertyBlock(propBlock);
                }
                timer += Time.deltaTime;
                yield return null;
            }
            Destroy(targetObject);
        }
    }
}
