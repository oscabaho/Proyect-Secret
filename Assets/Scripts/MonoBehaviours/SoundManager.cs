using UnityEngine;
using ProyectSecret.Audio; // Corregido

/// <summary>
/// Gestiona la reproducción de música y efectos de sonido de UI.
/// Delega la reproducción de efectos de sonido dinámicos al AudioPoolManager.
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
    }
    #endregion

    [Header("Fuentes de Audio Dedicadas")]
    [Tooltip("Fuente para la música de fondo. Debería tener 'Loop' activado.")]
    [SerializeField] private AudioSource _musicaSource;
    [Tooltip("Fuente para efectos de sonido de UI y 2D no dinámicos.")]
    [SerializeField] private AudioSource _efectosSource;

    public void IniciarMusica(AudioClip clipMusica)
    {
        if (clipMusica != null && _musicaSource.clip != clipMusica)
        {
            _musicaSource.clip = clipMusica;
            _musicaSource.loop = true;
            _musicaSource.Play();
        }
    }

    /// <summary>
    /// Reproduce un efecto de sonido 2D (como un clic de UI) usando la fuente dedicada.
    /// </summary>
    public void ReproducirEfecto(AudioData audioData)
    {
        if (audioData != null)
        {
            _efectosSource.PlayOneShot(audioData.GetClip(), audioData.GetVolume());
        }
    }

    /// <summary>
    /// Reproduce un sonido en un punto del espacio 3D usando el AudioPoolManager.
    /// </summary>
    public void ReproducirEfectoEnPunto(AudioData audioData, Vector3 position)
    {
        // Delegamos la responsabilidad al AudioPoolManager.
        if (AudioPoolManager.Instance != null)
        {
            AudioPoolManager.Instance.PlayAtPoint(audioData, position);
        }
        else
        {
            Debug.LogWarning("AudioPoolManager no encontrado. Usando AudioSource.PlayClipAtPoint como fallback.");
            if (audioData != null)
            {
                AudioSource.PlayClipAtPoint(audioData.GetClip(), position, audioData.GetVolume());
            }
        }
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
