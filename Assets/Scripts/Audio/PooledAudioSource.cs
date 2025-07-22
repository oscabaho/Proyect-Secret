using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace ProyectSecret.Audio
{
    /// <summary>
    /// Componente para un AudioSource gestionado por un ObjectPool.
    /// Se encarga de devolverse al pool cuando termina de reproducir el audio.
    /// Su única responsabilidad es reproducir un sonido configurado y gestionar su
    /// ciclo de vida para volver al pool.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class PooledAudioSource : MonoBehaviour
    {
        private AudioSource _audioSource;
        private Coroutine _returnToPoolCoroutine;

        /// <summary>
        /// El componente AudioSource nativo. Útil para configuraciones avanzadas (ej. 3D).
        /// </summary>
        public AudioSource Source => _audioSource;

        /// <summary>
        /// El pool de objetos al que pertenece esta instancia.
        /// </summary>
        public IObjectPool<PooledAudioSource> Pool { get; set; }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
        }

        /// <summary>
        /// Inicia la reproducción del clip de audio con una configuración específica.
        /// </summary>
        /// <param name="clip">El clip de audio a reproducir.</param>
        /// <param name="volume">El volumen del sonido (0.0 a 1.0).</param>
        /// <param name="pitch">El pitch del sonido.</param>
        /// <param name="loop">Indica si el sonido debe reproducirse en bucle.</param>
        /// <param name="spatialBlend">Mezcla espacial del sonido (0.0 para 2D, 1.0 para 3D).</param>
        public void Play(AudioClip clip, float volume = 1f, float pitch = 1f, bool loop = false, float spatialBlend = 0.0f)
        {
            if (clip == null)
            {
                Debug.LogWarning("Se intentó reproducir un AudioClip nulo. Devolviendo al pool de inmediato.");
                ReturnToPool();
                return;
            }

            gameObject.name = $"Pooled Audio - {clip.name}";

            _audioSource.clip = clip;
            _audioSource.volume = volume;
            _audioSource.pitch = pitch;
            _audioSource.loop = loop;
            _audioSource.spatialBlend = spatialBlend;
            
            _audioSource.Play();

            if (!loop)
            {
                _returnToPoolCoroutine = StartCoroutine(ReturnToPoolWhenFinished());
            }
        }

        /// <summary>
        /// Detiene la reproducción del sonido y lo devuelve inmediatamente al pool.
        /// </summary>
        public void Stop()
        {
            _audioSource.Stop();
            ReturnToPool();
        }

        private void ReturnToPool()
        {
            if (_returnToPoolCoroutine != null)
            {
                StopCoroutine(_returnToPoolCoroutine);
                _returnToPoolCoroutine = null;
            }
            
            Pool?.Release(this);
        }

        private IEnumerator ReturnToPoolWhenFinished()
        {
            yield return new WaitWhile(() => _audioSource.isPlaying);
            ReturnToPool();
        }
    }
}
