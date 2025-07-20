using System.Collections;
using UnityEngine;
using Characters;
using ProyectSecret.Events;

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
        [SerializeField] private float staminaRecoveryInterval = 0.5f;
        [SerializeField] private float staminaRecoveryDelay = 2f;
        private Coroutine staminaRecoveryCoroutine;
        [Header("Sonido de muerte del jugador")]
        [SerializeField] private AudioClip PlayerDeathSound;

        protected override void Awake()
        {
            base.Awake();
            if (healthBehaviour == null)
                Debug.LogWarning("PlayerHealthController: No se encontró HealthComponentBehaviour.");
            staminaBehaviour = GetComponent<ProyectSecret.Components.StaminaComponentBehaviour>();
            if (staminaBehaviour == null)
                Debug.LogWarning("PlayerHealthController: No se encontró StaminaComponentBehaviour.");
        }

        private void OnEnable()
        {
            // Suscribirse a un evento genérico de acción del jugador que consume stamina
            GameEventBus.Instance.Subscribe<PlayerActionUsedStaminaEvent>(HandlePlayerAction);
        }

        private void OnDisable()
        {
            if (GameEventBus.Instance != null)
                GameEventBus.Instance.Unsubscribe<PlayerActionUsedStaminaEvent>(HandlePlayerAction);
        }

        // Este método se llama cuando cualquier acción (ataque, esquivar, etc.) consume stamina
        private void HandlePlayerAction(PlayerActionUsedStaminaEvent evt)
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
            while (Stamina != null && Stamina.CurrentValue < Stamina.MaxValue)
            {
                Stamina.AffectValue(staminaRecoveryAmount);
                yield return new WaitForSeconds(staminaRecoveryInterval);
            }
            staminaRecoveryCoroutine = null;
        }

        protected override void Death()
        {
            // Notificar a otros sistemas que el jugador ha muerto, en lugar de destruirlo aquí.
            // El CombatSceneController se encargará de la lógica de derrota y de la destrucción del objeto.
            SoundManager.Smanager.ReproduceEffect(PlayerDeathSound);
            GameEventBus.Instance.Publish(new PlayerDiedEvent(gameObject));
            // Ya no se destruye aquí para permitir que otros sistemas reaccionen al evento.
        }
    }
}
