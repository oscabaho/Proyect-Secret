using UnityEngine;
using UnityEngine.SceneManagement;
using Combat.SceneManagement;

namespace Combat.SceneManagement
{
    /// <summary>
    /// Carga la escena de combate y transfiere los datos necesarios.
    /// </summary>
    public class CombatSceneLoader : MonoBehaviour
    {
        [SerializeField] private CombatTransferData transferData;
        [SerializeField] private string combatSceneName = "CombatScene";

        public void LoadCombatScene(GameObject playerPrefab, GameObject enemyPrefab, string kryptoniteItemId)
        {
            transferData.playerPrefab = playerPrefab;
            transferData.enemyPrefab = enemyPrefab;
            transferData.kryptoniteItemId = kryptoniteItemId;
            SceneManager.LoadScene(combatSceneName);
        }
    }
}
