using UnityEngine;
using ProyectSecret.Combat.SceneManagement;
using ProyectSecret.Inventory;
using MonoBehaviours.Enemies;
using ProyectSecret.Events; // Necesario para el GameEventBus

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
        [SerializeField] private AudioClip Music;

        private void Start()
        {
            if (transferData.playerPrefab != null && transferData.enemyPrefab != null)
            {
                var player = Instantiate(transferData.playerPrefab, playerSpawnPoint.position, Quaternion.identity);
                
                // Publicar el evento de que el jugador ha sido instanciado
                GameEventBus.Instance.Publish(new PlayerSpawnedEvent(player));

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
            SoundManager.Instancia.IniciarMusica(Music);
            transferData.Clear();
        }
    }
}
