using System.Collections;
using UnityEngine;
using Characters;

namespace ProyectSecret.Characters
{
    /// <summary>
    /// Controlador de salud y muerte para el jugador. Hereda de HealthControllerBase.
    /// </summary>
    [RequireComponent(typeof(ProyectSecret.Components.HealthComponentBehaviour))]
    [RequireComponent(typeof(ProyectSecret.Components.StaminaComponentBehaviour))]
    public class PlayerHealthController : HealthControllerBase
    {
        private ProyectSecret.Components.StaminaComponentBehaviour staminaBehaviour;
        public ProyectSecret.Components.StaminaComponent Stamina { get { return staminaBehaviour != null ? staminaBehaviour.Stamina : null; } }
        // Usa la propiedad Health de la base (no se redefine para evitar warning)

        [Header("Stamina Recovery")]
        [SerializeField] private int staminaRecoveryAmount = 5;
        [SerializeField] private float staminaRecoveryInterval = 1f;
        private Coroutine staminaRecoveryCoroutine;
        private float staminaRecoveryDelay = 2f;

        protected override void Awake()
        {
            base.Awake();
            if (healthBehaviour == null)
                Debug.LogWarning("PlayerHealthController: No se encontró HealthComponentBehaviour.");
            staminaBehaviour = GetComponent<ProyectSecret.Components.StaminaComponentBehaviour>();
            if (staminaBehaviour == null)
                Debug.LogWarning("PlayerHealthController: No se encontró StaminaComponentBehaviour.");
        }

        public void OnPlayerAttack()
        {
            if (staminaRecoveryCoroutine != null)
            {
                StopCoroutine(staminaRecoveryCoroutine);
            }
            staminaRecoveryCoroutine = StartCoroutine(StaminaRecoveryRoutine());
        }

        private IEnumerator StaminaRecoveryRoutine()
        {
            yield return new WaitForSeconds(staminaRecoveryDelay);
            while (Stamina != null && Stamina.CurrentStamina < Stamina.MaxValue)
            {
                Stamina.AffectValue(staminaRecoveryAmount);
                yield return new WaitForSeconds(staminaRecoveryInterval);
            }
            staminaRecoveryCoroutine = null;
        }

        protected override void Death()
        {
            // Aquí puedes agregar lógica personalizada para la muerte del jugador
            Destroy(gameObject);
        }
    }
}
