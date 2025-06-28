using UnityEngine;
using UnityEngine.SceneManagement;
using Enemies;

namespace Combat.SceneManagement
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
            Invoke(nameof(ReturnToExploration), delayAfterVictory);
        }

        private void ReturnToExploration()
        {
            SceneManager.LoadScene(explorationSceneName);
        }
    }
}
