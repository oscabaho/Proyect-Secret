using UnityEngine;
using ProyectSecret.Utils; // 1. Importar el namespace del ObjectPool
using ProyectSecret.Managers; // Importar el nuevo manager
using ProyectSecret.Interfaces;

namespace ProyectSecret.Enemies
{
    [RequireComponent(typeof(Rigidbody))]
    public class RockController : MonoBehaviour
    {
        [Tooltip("Define qué capas se consideran 'suelo' para que la roca se destruya.")]
        [SerializeField] private LayerMask groundLayer;

        [Header("Efectos de Impacto")]
        [SerializeField] private AudioClip impactSound;
        [SerializeField] [Range(0f, 1f)] private float soundVolume = 0.8f;
        
        private ObjectPool<RockController> rockPool;
        private GameObject shadowInstance;
        private Rigidbody rb;

        private void Awake()
        {
            // Cacheamos el Rigidbody para un acceso más eficiente.
            rb = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Asigna el pool al que esta roca debe regresar.
        /// También recibe la instancia de la sombra que debe desactivar.
        /// </summary>
        public void Initialize(ObjectPool<RockController> objectPool, GameObject shadow)
        {
            rockPool = objectPool;
            shadowInstance = shadow;
        }

        private void OnDisable()
        {
            // Al desactivarse (cuando vuelve al pool), reseteamos su estado físico.
            // Esto es importante para que no mantenga velocidades de caídas anteriores.
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            // Al desactivar la roca, también desactivamos su sombra para que vuelva al pool.
            if (shadowInstance != null)
                shadowInstance.SetActive(false);
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Comprobamos si la capa del objeto con el que colisionamos está incluida en nuestra LayerMask.
            // Esta es una forma mucho más eficiente y robusta que usar CompareTag.
            if ((groundLayer.value & (1 << collision.gameObject.layer)) > 0)
            {
                PlayImpactEffects(collision.contacts[0].point);
                rockPool?.Return(gameObject);
            }
            // También nos devolvemos al pool si chocamos con el jugador.
            else if (collision.gameObject.CompareTag("Player"))
            {
                PlayImpactEffects(collision.contacts[0].point);

                // Aplicamos daño al jugador antes de devolverse al pool.
                var damageable = collision.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    // TODO: Definir el daño de la roca, por ahora un valor fijo.
                    damageable.TakeDamage(10);
                }
                rockPool?.Return(gameObject);
            }
        }

        private void PlayImpactEffects(Vector3 position)
        {
            // Le pedimos al manager central que reproduzca los efectos por nosotros.
            // 1. Efecto visual de impacto
            VFXManager.Instance?.PlayImpactEffect(position);

            // 2. Sonido de impacto
            if (impactSound != null)
            {
                SoundManager.Instancia?.ReproducirEfectoEnPunto(impactSound, position, soundVolume);
            }
        }
    }
}
