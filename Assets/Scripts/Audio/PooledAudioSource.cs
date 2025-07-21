using UnityEngine;

namespace ProyectSecret.Audio
{
    /// <summary>
    /// Componente marcador para identificar GameObjects que contienen un AudioSource
    /// y que son gestionados por un ObjectPool.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class PooledAudioSource : MonoBehaviour
    {
        // Este script no necesita lógica. Su única función es actuar como un
        // componente de tipo MonoBehaviour para que el ObjectPool<T> funcione,
        // ya que AudioSource no hereda de MonoBehaviour.
    }
}
