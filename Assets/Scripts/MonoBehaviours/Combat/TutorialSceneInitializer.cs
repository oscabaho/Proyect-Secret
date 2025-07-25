using MonoBehaviours.Enemies;
using ProyectSecret.Combat.SceneManagement;
using ProyectSecret.Events;
using ProyectSecret.Inventory;
using ProyectSecret.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialSceneInitializer : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Transform enemySpawnPoint;
    [SerializeField] private PlayerPersistentData playerPersistentData;
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private AudioClip Music;

    private void Start()
    {
        if (playerPrefab != null && enemyPrefab != null)
        {
            var player = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);

            // Publicar el evento de que el jugador ha sido instanciado
            GameEventBus.Instance.Publish(new PlayerSpawnedEvent(player));

            // Solicitar la restauración del estado del jugador a través de un evento.
            GameEventBus.Instance.Publish(new PlayerStateRestoreRequestEvent(player, playerPersistentData, itemDatabase));

            var enemy = Instantiate(enemyPrefab, enemySpawnPoint.position, Quaternion.identity);

            // Configura la kriptonita del enemigo
            var kryptonite = enemy.GetComponent<KryptoniteDebuff>();
            if (kryptonite != null)
            {
                kryptonite.CheckKryptonite(player);
            }
        }
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(Music);
        }
        else
        {
            Debug.LogWarning("CombatSceneInitializer: Instancia de AudioManager no encontrada. La música de fondo no se reproducirá.");
        }
    }

    private void Update()
    {
        if(enemyPrefab == null)
        {
            SceneManager.LoadScene("Exploration");
        }
    }
}
