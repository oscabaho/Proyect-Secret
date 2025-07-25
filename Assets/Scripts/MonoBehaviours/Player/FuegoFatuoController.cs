using UnityEngine;

namespace ProyectSecret.MonoBehaviours.Player
{
    /// <summary>
    /// Controla el comportamiento de una luz tipo "fuego fatuo", haciéndola orbitar,
    /// flotar y parpadear de forma natural alrededor de su objeto padre.
    /// </summary>
    [RequireComponent(typeof(Light))]
    public class FuegoFatuoController : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private Light pointLight; // La luz que parpadea
        [SerializeField] private ParticleSystem trailParticles; // El sistema de partículas del rastro

        [Header("Movimiento Orbital")]
        [Tooltip("Distancia a la que la luz orbita alrededor del jugador.")]
        [SerializeField] private float orbitDistance = 0.8f;
        [Tooltip("Velocidad a la que la luz orbita.")]
        [SerializeField] private float orbitSpeed = 50f;

        [Header("Movimiento Flotante (Bobbing)")]
        [Tooltip("Altura máxima que la luz sube y baja.")]
        [SerializeField] private float bobAmount = 0.2f;
        [Tooltip("Velocidad del movimiento de flotación.")]
        [SerializeField] private float bobSpeed = 2f;

        [Header("Parpadeo (Flicker)")]
        [Tooltip("Intensidad mínima de la luz.")]
        [SerializeField] private float minIntensity = 0.8f;
        [Tooltip("Intensidad máxima de la luz.")]
        [SerializeField] private float maxIntensity = 1.5f;
        [Tooltip("Velocidad del parpadeo. Valores más altos crean un parpadeo más rápido.")]
        [SerializeField] private float flickerSpeed = 5f;

        private float orbitAngle = 0f;
        private float bobTime = 0f;
        private Vector3 initialLocalPosition;

        private void Awake()
        {
            if (pointLight == null)
                pointLight = GetComponent<Light>();
            if (trailParticles == null)
                trailParticles = GetComponent<ParticleSystem>();

            initialLocalPosition = transform.localPosition;
        }

        private void Update()
        {
            // --- Lógica de Órbita y Flotación ---
            orbitAngle += orbitSpeed * Time.deltaTime;
            bobTime += bobSpeed * Time.deltaTime;

            float x = Mathf.Cos(Mathf.Deg2Rad * orbitAngle) * orbitDistance;
            float z = Mathf.Sin(Mathf.Deg2Rad * orbitAngle) * orbitDistance;
            float y = initialLocalPosition.y + (Mathf.Sin(bobTime) * bobAmount);

            transform.localPosition = new Vector3(x, y, z);

            // --- Lógica de Parpadeo y Emisión de Partículas ---
            // Usamos PerlinNoise para un parpadeo suave y natural, en lugar de un Random brusco.
            float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);
            pointLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);

            // Hacemos que la emisión de partículas esté ligada a la intensidad de la luz.
            if (trailParticles != null)
            {
                var emission = trailParticles.emission;
                // Multiplicamos la intensidad por un factor para obtener una tasa de emisión deseada.
                emission.rateOverTime = pointLight.intensity * 10f; 
            }
        }
    }
}