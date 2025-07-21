using UnityEngine;
using System.Collections;
using ProyectSecret.Managers;
using Characters; // Necesario para heredar de HealthControllerBase

namespace ProyectSecret.Enemies
{
    /// <summary>
    /// Controlador de salud para el enemigo. Ahora hereda de la clase base para unificar la lógica.
    /// </summary>
    public class EnemyHealthController : HealthControllerBase
    {
        [Header("Fade Out Config")]
        [SerializeField] private float fadeDuration = 2f;

        // La clase base ya se encarga de recibir daño (TakeDamage) y de publicar el evento de muerte.
        // Solo necesitamos implementar el comportamiento específico de la muerte del enemigo.
        protected override void Death()
        {
            // Desactivar el collider para evitar más interacciones mientras muere.
            var collider = GetComponent<Collider>();
            if (collider != null) collider.enabled = false;

            // El evento CharacterDeathEvent ya ha sido publicado por la clase base.
            // Ahora delegamos el efecto visual de la muerte al VFXManager.
            // El propio manager se encargará de destruir el objeto.
            VFXManager.Instance?.PlayFadeAndDestroyEffect(gameObject, fadeDuration);
        }
    }
}
