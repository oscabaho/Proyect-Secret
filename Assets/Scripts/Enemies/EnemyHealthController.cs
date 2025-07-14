using UnityEngine;
using System.Collections;
using ProyectSecret.Components;

namespace ProyectSecret.Enemies
{
    public class EnemyHealthController : MonoBehaviour
    {
        [Header("Vida del enemigo")]
        [SerializeField] private HealthComponentBehaviour healthBehaviour;
        public HealthComponent Health { get { return healthBehaviour != null ? healthBehaviour.Health : null; } }

        private void Awake()
        {
            if (healthBehaviour == null)
                healthBehaviour = GetComponent<HealthComponentBehaviour>();
        }

        public void TakeDamage(int amount)
        {
            if (Health != null)
            {
                Health.AffectValue(-amount);
                if (Health.CurrentValue <= 0)
                {
                    Die();
                }
            }
        }

        private void Die()
        {
            // Inicia desvanecimiento gradual antes de destruir el objeto
            StartCoroutine(FadeAndDestroy());
        }

        [Header("Fade Out Config")]
        [SerializeField] private float fadeDuration = 2f; // Estándar para fade out
        public static System.Action OnEnemyDestroyed;

        private IEnumerator FadeAndDestroy()
        {
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
            OnEnemyDestroyed?.Invoke(); // Notifica que el enemigo fue destruido para iniciar el cambio de escena
        }
    }
}
