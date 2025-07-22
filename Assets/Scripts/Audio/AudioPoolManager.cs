using UnityEngine;
using UnityEngine.Pool;

namespace ProyectSecret.Audio
{
    /// <summary>
    /// Gestiona un pool de objetos PooledAudioSource para reproducir sonidos de
    /// manera eficiente. Actúa como un Singleton para ser accesible globalmente.
    /// Su única responsabilidad es la gestión del ciclo de vida del pool de audio.
    /// </summary>
    public class AudioPoolManager : MonoBehaviour
    {
        public static AudioPoolManager Instance { get; private set; }

        [Header("Configuración del Pool")]
        [Tooltip("Capacidad inicial del pool de AudioSources.")]
        [SerializeField] private int _defaultCapacity = 15;
        
        [Tooltip("Tamaño máximo que puede alcanzar el pool.")]
        [SerializeField] private int _maxSize = 50;

        private IObjectPool<PooledAudioSource> _pool;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _pool = new ObjectPool<PooledAudioSource>(
                CreatePooledAudio,
                OnGetFromPool,
                OnReleaseToPool,
                OnDestroyPooledAudio,
                true,
                _defaultCapacity,
                _maxSize
            );
        }

        /// <summary>
        /// Obtiene un AudioSource del pool y reproduce un sonido 2D usando un AudioData.
        /// </summary>
        public PooledAudioSource Play(AudioData audioData)
        {
            if (audioData == null || audioData.clips.Length == 0)
            {
                Debug.LogWarning("AudioPoolManager: Se intentó reproducir un AudioData nulo o sin clips.");
                return null;
            }

            var pooledAudio = _pool.Get();
            if (pooledAudio == null) return null;

            pooledAudio.Source.outputAudioMixerGroup = audioData.outputMixerGroup;
            pooledAudio.Play(
                audioData.GetClip(),
                audioData.GetVolume(),
                audioData.GetPitch(),
                audioData.loop,
                0.0f // 2D
            );
            
            return pooledAudio;
        }

        /// <summary>
        /// Obtiene un AudioSource del pool y reproduce un sonido 3D en una posición usando un AudioData.
        /// </summary>
        public PooledAudioSource PlayAtPoint(AudioData audioData, Vector3 position)
        {
            if (audioData == null || audioData.clips.Length == 0)
            {
                Debug.LogWarning("AudioPoolManager: Se intentó reproducir un AudioData nulo o sin clips.");
                return null;
            }

            var pooledAudio = _pool.Get();
            if (pooledAudio == null) return null;

            pooledAudio.transform.position = position;
            pooledAudio.Source.outputAudioMixerGroup = audioData.outputMixerGroup;
            pooledAudio.Play(
                audioData.GetClip(),
                audioData.GetVolume(),
                audioData.GetPitch(),
                false, // Los sonidos en un punto no suelen ser en bucle
                1.0f   // 3D
            );

            return pooledAudio;
        }

        #region Métodos de Gestión del Pool

        private PooledAudioSource CreatePooledAudio()
        {
            var go = new GameObject("PooledAudioSource");
            var pooledAudio = go.AddComponent<PooledAudioSource>();
            pooledAudio.Pool = _pool;
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
            pooledAudio.gameObject.SetActive(false);
        }

        private void OnDestroyPooledAudio(PooledAudioSource pooledAudio)
        {
            Destroy(pooledAudio.gameObject);
        }

        #endregion
    }
}
