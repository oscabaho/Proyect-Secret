using UnityEngine;
using MonoBehaviours.Enemies;
using ProyectSecret.Events;
using ProyectSecret.Managers;
using System.Collections;

namespace ProyectSecret.Combat.SceneManagement
{
    /// <summary>
    /// Controla el flujo post-combate: victoria, recompensas y regreso a exploración.
    /// </summary>
    public class CombatSceneController : MonoBehaviour
    {
        [SerializeField] private GameObject enemyInstance; // Cambiamos la referencia a GameObject
        [SerializeField] private float delayAfterVictory = 2f;
        [SerializeField] private float delayAfterDefeat = 1.5f;
        [SerializeField] private GameObject playerInstance; // Asigna el jugador instanciado en combate
        [SerializeField] private PlayerPersistentData playerPersistentData;

        private void OnEnable()
        {
            GameEventBus.Instance.Subscribe<PlayerDiedEvent>(HandlePlayerDeath);
            GameEventBus.Instance.Subscribe<CharacterDeathEvent>(HandleCharacterDeath);
        }

        private void OnDisable()
        {
            if(GameEventBus.Instance != null)
            {
                GameEventBus.Instance.Unsubscribe<PlayerDiedEvent>(HandlePlayerDeath);
                GameEventBus.Instance.Unsubscribe<CharacterDeathEvent>(HandleCharacterDeath);
            }
        }

        private void HandleCharacterDeath(CharacterDeathEvent evt)
        {
            if (enemyInstance != null && evt.Entity == enemyInstance)
            {
                StartCoroutine(VictorySequence());
            }
        }

        private IEnumerator VictorySequence()
        {
            #if UNITY_EDITOR
            Debug.Log("¡Enemigo derrotado! Regresando a exploración...");
            #endif

            // Guardar estado actualizado del jugador e inventario
            if (playerPersistentData != null && playerInstance != null)
                playerPersistentData.SaveFromPlayer(playerInstance);
            
            GameEventBus.Instance.Publish(new CombatVictoryEvent(enemyInstance));

            // Esperar el delay usando la corutina
            yield return new WaitForSeconds(delayAfterVictory);

            // Cargar la escena de exploración
            SceneTransitionManager.Instance?.LoadExplorationScene(playerInstance);
        }

        private void HandlePlayerDeath(PlayerDiedEvent evt)
        {
            if (evt.PlayerObject != playerInstance) return;

            #if UNITY_EDITOR
            Debug.Log("¡Jugador derrotado! Regresando a exploración, inicio de día, punto fijo.");
            #endif
            
            StartCoroutine(DefeatSequence());
        }

        private IEnumerator DefeatSequence()
        {
            // Guardar estado actualizado de vida y stamina del jugador antes de regresar a exploración
            if (playerPersistentData != null)
            {
                playerPersistentData.SaveFromPlayer(playerInstance);
                playerPersistentData.CameFromDefeat = true;
            }
            
            GameEventBus.Instance.Publish(new ProyectSecret.Events.DayStartedEvent());

            // Esperar el delay
            yield return new WaitForSeconds(delayAfterDefeat);

            // Cargar la escena de exploración
            SceneTransitionManager.Instance?.LoadExplorationScene(playerInstance);
        }
    }
}
