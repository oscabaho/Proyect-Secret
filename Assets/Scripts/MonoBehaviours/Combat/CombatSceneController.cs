using UnityEngine;
using ProyectSecret.Events;
using ProyectSecret.Managers;
using System.Collections;

namespace ProyectSecret.Combat.SceneManagement
{
    /// <summary>
    /// Controla el flujo post-combate: victoria, recompensas y regreso a exploración.
    /// Ahora encuentra al jugador y al enemigo dinámicamente.
    /// </summary>
    public class CombatSceneController : MonoBehaviour
    {
        [Header("Configuración de Secuencia")]
        [SerializeField] private float delayAfterVictory = 2f;
        [SerializeField] private float delayAfterDefeat = 1.5f;

        [Header("Datos Persistentes")]
        [SerializeField] private PlayerPersistentData playerPersistentData;

        // Las referencias al jugador y al enemigo ahora se obtienen automáticamente.
        private GameObject playerInstance;
        private GameObject enemyInstance;

        private void Awake()
        {
            // Busca al enemigo en la escena por su tag.
            // Asegúrate de que el prefab del enemigo tenga el tag "Enemy".
            enemyInstance = GameObject.FindGameObjectWithTag("Enemy");
            if (enemyInstance == null)
            {
                #if UNITY_EDITOR
                Debug.LogError("CombatSceneController: No se encontró ningún GameObject con el tag 'Enemy' en la escena.", this);
                #endif
            }
        }

        private void OnEnable()
        {
            GameEventBus.Instance.Subscribe<PlayerSpawnedEvent>(OnPlayerSpawned);
            GameEventBus.Instance.Subscribe<PlayerDiedEvent>(HandlePlayerDeath);
            GameEventBus.Instance.Subscribe<CharacterDeathEvent>(HandleCharacterDeath);
        }

        private void OnDisable()
        {
            if (GameEventBus.Instance != null)
            {
                GameEventBus.Instance.Unsubscribe<PlayerSpawnedEvent>(OnPlayerSpawned);
                GameEventBus.Instance.Unsubscribe<PlayerDiedEvent>(HandlePlayerDeath);
                GameEventBus.Instance.Unsubscribe<CharacterDeathEvent>(HandleCharacterDeath);
            }
        }

        // Este método se activa cuando el CombatSceneInitializer crea al jugador.
        private void OnPlayerSpawned(PlayerSpawnedEvent evt)
        {
            playerInstance = evt.PlayerObject;
        }

        private void HandleCharacterDeath(CharacterDeathEvent evt)
        {
            // Comprueba si la entidad que murió es el enemigo que tenemos registrado.
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
                playerPersistentData.SaveFromPlayer(playerInstance, savePosition: false); // No guardamos la posición del combate
            
            GameEventBus.Instance.Publish(new CombatVictoryEvent(enemyInstance));

            // Esperar el delay usando la corutina
            yield return new WaitForSeconds(delayAfterVictory);

            // Cargar la escena de exploración
            SceneTransitionManager.Instance?.LoadExplorationScene(playerInstance);
        }

        private void HandlePlayerDeath(PlayerDiedEvent evt)
        {
            // Comprueba si el jugador que murió es el que tenemos registrado.
            if (playerInstance == null || evt.PlayerObject != playerInstance) return;

            #if UNITY_EDITOR
            Debug.Log("¡Jugador derrotado! Regresando a exploración, inicio de día, punto fijo.");
            #endif
            
            StartCoroutine(DefeatSequence());
        }

        private IEnumerator DefeatSequence()
        {
            // Guardar estado actualizado de vida y stamina del jugador antes de regresar a exploración
            if (playerPersistentData != null && playerInstance != null)
            {
                playerPersistentData.SaveFromPlayer(playerInstance, savePosition: false); // No guardamos la posición del combate
                playerPersistentData.CameFromDefeat = true;
            }
            
            GameEventBus.Instance.Publish(new DayStartedEvent());

            // Esperar el delay
            yield return new WaitForSeconds(delayAfterDefeat);

            // Cargar la escena de exploración
            SceneTransitionManager.Instance?.LoadExplorationScene(playerInstance);
        }
    }
}
