using UnityEngine;
using ProyectSecret.Combat.SceneManagement;
using ProyectSecret.Inventory; // Necesario para ItemDatabase
using ProyectSecret.Events;   // Necesario para el GameEventBus

/// <summary>
/// Inicializa la escena de exploración y posiciona al jugador en un punto fijo (estatua) si viene de una derrota en combate.
/// </summary>
public class ExplorationSceneInitializer : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform statueSpawnPoint;
    [SerializeField] private PlayerPersistentData playerPersistentData;
    [SerializeField] private ItemDatabase itemDatabase; // Añadir referencia a la base de datos de ítems
    [Header("Cancion de fondo")]
    [SerializeField] private AudioClip backgroundMusic; // Canción de fondo para la escena

    void Start()
    {
        if (playerPrefab == null || statueSpawnPoint == null || playerPersistentData == null || itemDatabase == null)
        {
            #if UNITY_EDITOR
            Debug.LogError("ExplorationSceneInitializer: Faltan referencias esenciales. Asigna PlayerPrefab, StatueSpawnPoint, PlayerPersistentData e ItemDatabase en el Inspector.");
            #endif
            return;
        }

        Vector3 spawnPosition = statueSpawnPoint.position;

        // Decidir la posición de spawn
        if (playerPersistentData.CameFromDefeat)
        {
            // Si viene de una derrota, siempre en la estatua.
            spawnPosition = statueSpawnPoint.position;
        }
        else if (playerPersistentData.HasSavedPosition)
        {
            // Si hay una posición guardada (y no viene de derrota), la usamos.
            spawnPosition = playerPersistentData.LastPosition;
        }

        var player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

        // Publicar el evento de que el jugador ha sido instanciado
        GameEventBus.Instance.Publish(new PlayerSpawnedEvent(player));

        if (playerPersistentData.CameFromDefeat)
        {
            playerPersistentData.ApplyToPlayer(player, itemDatabase);
            playerPersistentData.CameFromDefeat = false; // Resetear el flag para la próxima vez
            playerPersistentData.HasSavedPosition = false; // Borramos la posición guardada para que no reaparezca ahí si sale y entra del juego
        }
        // Si no viene de una derrota, puede ser la primera vez o una carga normal.
        else
        {
            // Si no hay un arma equipada guardada, asumimos que es la primera vez que se juega.
            if (string.IsNullOrEmpty(playerPersistentData.equippedWeaponId))
            {
                // Inicializa vida y stamina al máximo SOLO si es la primera vez.
                var health = player.GetComponent<ProyectSecret.Components.HealthComponentBehaviour>();
                if (health != null) health.SetToMax();
                
                var stamina = player.GetComponent<ProyectSecret.Components.StaminaComponentBehaviour>();
                if (stamina != null) stamina.SetToMax();
            }
            else
            {
                // Si hay datos guardados (no es la primera vez), simplemente los aplicamos.
                playerPersistentData.ApplyToPlayer(player, itemDatabase);
            }

            // Una vez usada la posición, la invalidamos para el siguiente inicio de juego.
            playerPersistentData.HasSavedPosition = false;
        }

        // Configuración de la cámara del jugador después de la inicialización.
        var playerMovement = player.GetComponent<PaperMarioPlayerMovement>();
        if (playerMovement != null)
        {
            Camera cam = player.GetComponentInChildren<Camera>();
            if (cam != null)
            {
                playerMovement.SetActiveCamera(cam);
            }
            else
            {
                #if UNITY_EDITOR
                Debug.LogError("ExplorationSceneInitializer: No se encontró una cámara en los hijos del jugador instanciado.");
                #endif
            }
        }

        if (SoundManager.Instancia != null)
        {
            if (backgroundMusic != null)
                SoundManager.Instancia.IniciarMusica(backgroundMusic);
        }
        else
        {
            #if UNITY_EDITOR
            Debug.LogWarning("ExplorationSceneInitializer: Instancia de SoundManager no encontrada. La música de fondo no se reproducirá.");
            #endif
        }
    }
}
