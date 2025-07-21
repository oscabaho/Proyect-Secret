using UnityEngine;
using ProyectSecret.Utils;
using ProyectSecret.Audio; // Importar el nuevo namespace
using System.Collections;

/// <summary>
/// Gestiona la reproducción de música y efectos de sonido en todo el juego.
/// Sigue el patrón Singleton para ser accesible desde cualquier script.
/// </summary>
public class SoundManager : MonoBehaviour
{
    #region Singleton
    private static SoundManager _instancia;
    public static SoundManager Instancia
    {
        get
        {
            if (_instancia == null)
            {
                _instancia = FindFirstObjectByType<SoundManager>();
                if (_instancia == null)
                {
                    GameObject soundManagerGO = new GameObject("SoundManager");
                    _instancia = soundManagerGO.AddComponent<SoundManager>();
                }
            }
            return _instancia;
        }
    }

    private void Awake()
    {
        if (_instancia != null && _instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        _instancia = this;
        DontDestroyOnLoad(gameObject);

        if (_musicaSource == null || _efectosSource == null)
        {
            AudioSource[] sources = GetComponents<AudioSource>();
            if (sources.Length < 2)
            {
                #if UNITY_EDITOR
                Debug.LogError("SoundManager necesita al menos 2 componentes AudioSource. Por favor, añádelos al GameObject.");
                #endif
            }
            else
            {
                _musicaSource = sources[0];
                _efectosSource = sources[1];
            }
        }
        
        // Inicializar el pool de AudioSource para sonidos espaciales
        if (_oneShotPrefab != null)
            _oneShotSourcePool = new ObjectPool<PooledAudioSource>(_oneShotPrefab, 10, transform);
    }
    #endregion

    [Header("Fuentes de Audio")]
    [Tooltip("Fuente para la música de fondo. Debería tener 'Loop' activado.")]
    [SerializeField] private AudioSource _musicaSource;
    [Tooltip("Fuente para efectos de sonido de UI y 2D.")]
    [SerializeField] private AudioSource _efectosSource;
    
    [Header("Pool para Sonidos 3D")]
    [Tooltip("Prefab que solo contenga un componente AudioSource y un PooledAudioSource para usar en el pool.")]
    [SerializeField] private GameObject _oneShotPrefab;
    private ObjectPool<PooledAudioSource> _oneShotSourcePool;

    public void IniciarMusica(AudioClip clipMusica)
    {
        if (clipMusica != null && _musicaSource.clip != clipMusica)
        {
            _musicaSource.clip = clipMusica;
            _musicaSource.loop = true;
            _musicaSource.Play();
        }
    }

    public void ReproducirEfecto(AudioClip clipEfecto, float volumen = 1.0f)
    {
        if (clipEfecto != null)
        {
            _efectosSource.PlayOneShot(clipEfecto, volumen);
        }
    }

    /// <summary>
    /// Reproduce un sonido en un punto del espacio 3D de forma eficiente usando un pool.
    /// </summary>
    public void ReproducirEfectoEnPunto(AudioClip clip, Vector3 position, float volume = 1.0f)
    {
        if (clip == null || _oneShotSourcePool == null) return;

        GameObject sourceGO = _oneShotSourcePool.Get();
        if (sourceGO != null)
        {
            sourceGO.transform.position = position;
            // Obtenemos el AudioSource del GameObject que nos da el pool.
            AudioSource source = sourceGO.GetComponent<AudioSource>();
            source.clip = clip;
            source.volume = volume;
            source.spatialBlend = 1.0f; // Asegurarse de que es 3D
            source.Play();
            StartCoroutine(ReturnSourceToPool(sourceGO, clip.length));
        }
    }

    private IEnumerator ReturnSourceToPool(GameObject sourceGO, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (sourceGO != null)
            _oneShotSourcePool.Return(sourceGO);
    }

    public void IniciarEfectoEnLoop(AudioClip clipEfecto, AudioSource source)
    {
        if (clipEfecto == null || source == null) return;

        if (!source.isPlaying || source.clip != clipEfecto)
        {
            source.clip = clipEfecto;
            source.loop = true;
            source.Play();
        }
    }

    public void DetenerEfectoEnLoop(AudioSource source)
    {
        if (source != null && source.isPlaying)
        {
            source.Stop();
        }
    }
}
