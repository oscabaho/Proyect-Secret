using UnityEngine;
using Characters;
using ProyectSecret.Managers;

using ProyectSecret.VFX;
using UnityEngine.SceneManagement;
using System.Collections;
namespace ProyectSecret.Characters.Enemies
{
    /// <summary>
    /// Controlador de salud y muerte para enemigos únicos. Hereda de HealthControllerBase.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class EnemyHealthController : HealthControllerBase
    {
        [Header("Fade Out Config")]
        [SerializeField] private float fadeDuration = 2f;
        [SerializeField] private bool finalBoss = false;


        protected override void Death()
        {
            // Desactivar el collider para evitar más interacciones mientras muere.
            var collider = GetComponent<Collider>();
            if (collider != null) collider.enabled = false;

            // El evento CharacterDeathEvent ya ha sido publicado por la clase base.
            // Ahora delegamos el efecto visual de la muerte al VFXManager.
            VFXManager.Instance?.PlayFadeAndDestroyEffect(gameObject, fadeDuration);
            if (finalBoss)
            {
                LoadVictory();
            }
        }

        private IEnumerator LoadVictory()
        {
            yield return new WaitForSeconds(fadeDuration);
            SceneManager.LoadScene("Victory");
        }
    }
}
