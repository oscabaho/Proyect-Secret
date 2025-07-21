using UnityEngine;
using System.Collections;

namespace ProyectSecret.Enemies
{
    /// <summary>
    /// Controla el comportamiento de la sombra/indicador de ataque, incluyendo un efecto de fade-in.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class ShadowController : MonoBehaviour
    {
        [Tooltip("Duración del efecto de aparición gradual (fade-in) en segundos.")]
        [SerializeField] private float fadeInDuration = 0.3f;

        private Renderer shadowRenderer;
        private MaterialPropertyBlock propBlock;
        private int colorID;
        private Color originalColor;
        private Coroutine fadeInCoroutine;

        private void Awake()
        {
            shadowRenderer = GetComponent<Renderer>();
            propBlock = new MaterialPropertyBlock();
            colorID = Shader.PropertyToID("_Color");

            // Guardamos el color original del material para saber a qué nivel de alfa debemos llegar.
            if (shadowRenderer.sharedMaterial.HasProperty(colorID))
            {
                originalColor = shadowRenderer.sharedMaterial.color;
            }
            else
            {
                originalColor = Color.white; // Un valor por defecto si el material no tiene color.
            }
        }

        private void OnEnable()
        {
            // Cada vez que la sombra se activa desde el pool, iniciamos el fade-in.
            if (fadeInCoroutine != null)
                StopCoroutine(fadeInCoroutine);
            fadeInCoroutine = StartCoroutine(FadeInRoutine());
        }

        private IEnumerator FadeInRoutine()
        {
            float timer = 0f;
            float targetAlpha = originalColor.a;

            // Empezamos con el alfa a cero para que sea invisible al inicio.
            Color currentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
            shadowRenderer.GetPropertyBlock(propBlock);
            propBlock.SetColor(colorID, currentColor);
            shadowRenderer.SetPropertyBlock(propBlock);
            
            while (timer < fadeInDuration)
            {
                // Interpolamos el valor del alfa desde 0 hasta el alfa objetivo.
                float currentAlpha = Mathf.Lerp(0f, targetAlpha, timer / fadeInDuration);
                currentColor.a = currentAlpha;
                
                // Aplicamos el nuevo color usando el MaterialPropertyBlock para un rendimiento óptimo.
                shadowRenderer.GetPropertyBlock(propBlock);
                propBlock.SetColor(colorID, currentColor);
                shadowRenderer.SetPropertyBlock(propBlock);

                timer += Time.deltaTime;
                yield return null;
            }

            // Al final, nos aseguramos de que el alfa sea exactamente el valor deseado.
            propBlock.SetColor(colorID, originalColor);
            shadowRenderer.SetPropertyBlock(propBlock);
        }
    }
}
