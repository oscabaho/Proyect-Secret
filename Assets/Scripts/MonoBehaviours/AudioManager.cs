using UnityEngine;
using UnityEngine.Pool;
using ProyectSecret.Audio;
using System.Collections.Generic;

namespace ProyectSecret.Managers
{
    /// <summary>
    /// Manager de audio unificado. Gestiona la música de fondo, los efectos de sonido
    /// 2D (UI) y un pool de AudioSources para efectos 3D eficientes.
    /// Es el único punto de entrada para todas las operaciones de audio.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Fuentes de Audio Dedicadas")]
        [Tooltip("Fuente para la música de fondo. Debería tener 'Loop' activado.")]
        [SerializeField] private AudioSource musicSource;
        [Tooltip("Fuente para efectos de sonido de UI y 2D no dinámicos.")]
        [SerializeField] private AudioSource effectsSource2D;

        [Header("Configuración del Pool de Audio 3D")]
        [Tooltip("Capacidad inicial del pool de AudioSources.")]
        [SerializeField] private int defaultPoolCapacity = 20;
        [Tooltip("Tamaño máximo que puede alcanzar el pool.")]
        [SerializeField] private int maxPoolSize = 50;

        private IObjectPool<PooledAudioSource> _sfxPool3D;
        private Dictionary<GameObject, PooledAudioSource> _loopedSources = new Dictionary<GameObject, PooledAudioSource>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializePool();
        }

        private void InitializePool()
        {
            _sfxPool3D = new ObjectPool<PooledAudioSource>(
                CreatePooledAudio,
                OnGetFromPool,
                OnReleaseToPool,
                OnDestroyPooledAudio,
                true,
                defaultPoolCapacity,
                maxPoolSize
            );
        }

        #region API Pública de Audio

        /// <summary>
        /// Inicia o cambia la música de fondo.
        /// </summary>
        public void PlayMusic(AudioClip musicClip)
        {
            if (musicClip != null && musicSource.clip != musicClip)
            {
                musicSource.clip = musicClip;
                musicSource.loop = true;
                musicSource.Play();
            }
        }

        /// <summary>
        /// Reproduce un sonido 2D, ideal para UI. Usa una fuente de audio dedicada.
        /// </summary>
        public void PlaySound2D(AudioData audioData)
        {
            if (audioData != null)
            {
                effectsSource2D.outputAudioMixerGroup = audioData.outputMixerGroup;
                effectsSource2D.PlayOneShot(audioData.GetClip(), audioData.GetVolume());
            }
        }

        /// <summary>
        /// Reproduce un sonido 3D en un punto del espacio usando el pool.
        /// </summary>
        public void PlaySound3D(AudioData audioData, Vector3 position)
        {
            if (audioData == null || audioData.clips.Length == 0) return;

            var pooledAudio = _sfxPool3D.Get();
            if (pooledAudio == null) return;

            pooledAudio.transform.position = position;
            pooledAudio.Play(audioData, 1.0f); // 1.0f para spatialBlend 3D
        }

        /// <summary>
        /// Reproduce un sonido en bucle asociado a un GameObject. Si ya hay un sonido en bucle, lo detiene.
        /// </summary>
        public void PlayLoopingSoundOnObject(AudioData audioData, GameObject target)
        {
            if (target == null || audioData == null) return;

            // Si ya hay un sonido en bucle en este objeto, lo detenemos primero.
            StopLoopingSoundOnObject(target);

            var pooledAudio = _sfxPool3D.Get();
            if (pooledAudio == null) return;

            pooledAudio.transform.SetParent(target.transform);
            pooledAudio.transform.localPosition = Vector3.zero;
            pooledAudio.Play(audioData, 0.0f, true); // 0.0f para spatialBlend 2D (sonido no posicional), loop = true

            _loopedSources[target] = pooledAudio;
        }

        /// <summary>
        /// Detiene el sonido en bucle que se esté reproduciendo en un GameObject.
        /// </summary>
        public void StopLoopingSoundOnObject(GameObject target)
        {
            if (target != null && _loopedSources.TryGetValue(target, out var pooledAudio))
            {
                pooledAudio.Stop(); // Esto lo devolverá al pool
                _loopedSources.Remove(target);
            }
        }

        #endregion

        #region Métodos de Gestión del Pool

        private PooledAudioSource CreatePooledAudio()
        {
            var go = new GameObject("PooledAudioSource");
            var pooledAudio = go.AddComponent<PooledAudioSource>();
            pooledAudio.Pool = _sfxPool3D;
            go.transform.SetParent(this.transform);
            return pooledAudio;
        }

        private void OnGetFromPool(PooledAudioSource pooledAudio)
        {
            pooledAudio.gameObject.SetActive(true);
        }

        private void OnReleaseToPool(PooledAudioSource pooledAudio)
        {
            pooledAudio.Source.Stop();
            pooledAudio.Source.clip = null;
            pooledAudio.transform.SetParent(this.transform);
            pooledAudio.transform.localPosition = Vector3.zero;
            _loopedSources.Remove(pooledAudio.gameObject); // Asegurarse de limpiar la referencia si era un sonido en bucle
            pooledAudio.gameObject.SetActive(false);
        }

        private void OnDestroyPooledAudio(PooledAudioSource pooledAudio)
        {
            if (pooledAudio != null)
            {
                Destroy(pooledAudio.gameObject);
            }
        }

        #endregion
    }
}
