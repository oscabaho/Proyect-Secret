using UnityEngine;
using ProyectSecret.Combat.SceneManagement;
using ProyectSecret.Inventory;
using Enemies.Behaviours;

namespace ProyectSecret.Combat.SceneManagement
{
    /// <summary>
    /// Inicializa la escena de combate instanciando jugador y enemigo, y aplica la lógica de kriptonita.
    /// </summary>
    public class CombatSceneInitializer : MonoBehaviour
    {
        [SerializeField] private CombatTransferData transferData;
        [SerializeField] private Transform playerSpawnPoint;
        [SerializeField] private Transform enemySpawnPoint;
        [SerializeField] private PlayerPersistentData playerPersistentData;
        [SerializeField] private ItemDatabase itemDatabase;

        private void Start()
        {
            if (transferData.playerPrefab != null && transferData.enemyPrefab != null)
            {
                var player = Instantiate(transferData.playerPrefab, playerSpawnPoint.position, Quaternion.identity);
                // Restaurar estado del jugador
                if (playerPersistentData != null && itemDatabase != null)
                    playerPersistentData.ApplyToPlayer(player, itemDatabase);
                var enemy = Instantiate(transferData.enemyPrefab, enemySpawnPoint.position, Quaternion.identity);

                // Configura la kriptonita del enemigo
                var kryptonite = enemy.GetComponent<KryptoniteDebuff>();
                if (kryptonite != null)
                {
                    kryptonite.CheckKryptonite(player);
                }
            }
            transferData.Clear();
        }
    }
}
