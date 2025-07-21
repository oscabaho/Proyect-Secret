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

            // Usamos un MaterialPropertyBlock para cambiar el color sin crear instancias de material.
            var propBlock = new MaterialPropertyBlock();
            int colorID = Shader.PropertyToID("_Color"); // Cacheamos el ID de la propiedad del shader.

            float timer = 0f;
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            
            // Guardamos los colores originales de cada renderer para no perderlos.
            var originalColors = new System.Collections.Generic.List<Color>();
            foreach (var rend in renderers)
            {
                if (rend.material.HasProperty(colorID))
                    originalColors.Add(rend.material.color);
                else
                    originalColors.Add(Color.white); // Añadir un color por defecto si no tiene la propiedad
            }

            while (timer < fadeDuration)
            {
                float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].GetPropertyBlock(propBlock); // Obtenemos el bloque actual
                    Color newColor = originalColors[i];
                    newColor.a = alpha;
                    propBlock.SetColor(colorID, newColor);
                    renderers[i].SetPropertyBlock(propBlock); // Aplicamos el bloque modificado
                }
                timer += Time.deltaTime;
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}
