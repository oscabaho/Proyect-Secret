using UnityEngine;
using UnityEngine.SceneManagement;
using MonoBehaviours.Enemies;
using ProyectSecret.Events;

namespace ProyectSecret.Combat.SceneManagement
{
    /// <summary>
    /// Controla el flujo post-combate: victoria, recompensas y regreso a exploración.
    /// </summary>
    public class CombatSceneController : MonoBehaviour
    {
        [SerializeField] private Enemy enemy;
        [SerializeField] private string explorationSceneName = "Exploracion";
        [SerializeField] private float delayAfterVictory = 2f;
        [SerializeField] private GameObject playerInstance; // Asigna el jugador instanciado en combate
        [SerializeField] private PlayerPersistentData playerPersistentData;

        private void Start()
        {
            if (enemy != null)
                enemy.SubscribeOnDeath(OnEnemyDefeated);
        }

        private void OnEnemyDefeated()
        {
            // Aquí puedes mostrar UI de victoria, animaciones, etc.
            Debug.Log("¡Enemigo derrotado! Regresando a exploración...");
            // Guardar estado actualizado del jugador e inventario
            if (playerPersistentData != null && playerInstance != null)
                playerPersistentData.SaveFromPlayer(playerInstance);
            GameEventBus.Instance.Publish(new CombatVictoryEvent(enemy != null ? enemy.gameObject : null));
            Invoke(nameof(ReturnToExplorationAfterDefeat), delayAfterVictory);
        }

        // Manejo explícito de derrota
        public void OnPlayerDefeated()
        {
            Debug.Log("¡Jugador derrotado! Regresando a exploración, inicio de día, punto fijo.");
            // Guardar estado si es necesario
            if (playerPersistentData != null && playerInstance != null)
            {
                playerPersistentData.SaveFromPlayer(playerInstance);
                playerPersistentData.CameFromDefeat = true;
            }
            // Publicar evento de inicio de día
            GameEventBus.Instance.Publish(new ProyectSecret.Events.DayStartedEvent());
            // Regresar a exploración tras un pequeño delay (puedes ajustar)
            Invoke(nameof(ReturnToExplorationAfterDefeat), 1.5f);
        }

        private void ReturnToExplorationAfterDefeat()
        {
            // Aquí puedes implementar lógica para posicionar al jugador en el punto fijo (estatua)
            // Por ahora solo carga la escena, la lógica de posicionamiento debe ir en el inicializador de la escena de exploración
            SceneManager.LoadScene(explorationSceneName);
        }
    }
}
