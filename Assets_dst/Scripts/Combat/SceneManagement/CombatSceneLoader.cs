using UnityEngine;
using UnityEngine.SceneManagement;
using ProyectSecret.Combat.SceneManagement;

namespace ProyectSecret.Combat.SceneManagement
{
    /// <summary>
    /// Carga la escena de combate y transfiere los datos necesarios.
    /// </summary>
    public class CombatSceneLoader : MonoBehaviour
    {
        [SerializeField] private CombatTransferData transferData;
        [SerializeField] private string combatSceneName = "CombatScene";
        [SerializeField] private PlayerPersistentData playerPersistentData;

        public void LoadCombatScene(GameObject playerPrefab, GameObject enemyPrefab, string kryptoniteItemId, GameObject playerInstance)
        {
            // Guardar estado del jugador antes de cambiar de escena
            if (playerPersistentData != null && playerInstance != null)
                playerPersistentData.SaveFromPlayer(playerInstance);
            transferData.playerPrefab = playerPrefab;
            transferData.enemyPrefab = enemyPrefab;
            transferData.kryptoniteItemId = kryptoniteItemId;
            SceneManager.LoadScene(combatSceneName);
        }
    }
}
