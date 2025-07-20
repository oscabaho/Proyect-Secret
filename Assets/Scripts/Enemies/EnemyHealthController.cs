using UnityEngine;
using System.Collections;
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
            // El evento CharacterDeathEvent ya ha sido publicado por la clase base.
            // Ahora solo nos encargamos del efecto visual de la muerte.
            StartCoroutine(FadeAndDestroy());
        }

        private IEnumerator FadeAndDestroy()
        {
            // Desactivar el collider para evitar más interacciones mientras muere.
            var collider = GetComponent<Collider>();
            if (collider != null) collider.enabled = false;

            float timer = 0f;
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            Color[] originalColors = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i].material.HasProperty("_Color"))
                    originalColors[i] = renderers[i].material.color;
            }
            while (timer < fadeDuration)
            {
                float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (renderers[i].material.HasProperty("_Color"))
                    {
                        Color c = originalColors[i];
                        c.a = alpha;
                        renderers[i].material.color = c;
                    }
                }
                timer += Time.deltaTime;
                yield return null;
            }
            Destroy(gameObject);
            // El evento estático OnEnemyDestroyed ya no es necesario.
        }
    }
}
