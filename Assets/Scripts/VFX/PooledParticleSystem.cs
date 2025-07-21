using UnityEngine;
using System.Collections;

namespace ProyectSecret.VFX
{
    /// <summary>
    /// Gestiona un sistema de partículas que pertenece a un pool.
    /// Se reproduce al activarse y se desactiva solo al terminar.
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class PooledParticleSystem : MonoBehaviour
    {
        private ParticleSystem ps;

        private void Awake()
        {
            ps = GetComponent<ParticleSystem>();
        }

        private void OnEnable()
        {
            // Se asegura de que el sistema de partículas se reproduzca al ser activado.
            ps.Play();
            // Inicia una corrutina para devolverse al pool después de que termine la animación.
            StartCoroutine(ReturnToPoolAfterDelay(ps.main.duration));
        }

        private IEnumerator ReturnToPoolAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(false);
        }
    }
}
