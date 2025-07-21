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

    void Start()
    {
        if (playerPrefab == null || statueSpawnPoint == null || playerPersistentData == null || itemDatabase == null)
        {
            #if UNITY_EDITOR
            Debug.LogError("ExplorationSceneInitializer: Faltan referencias esenciales. Asigna PlayerPrefab, StatueSpawnPoint, PlayerPersistentData e ItemDatabase en el Inspector.");
            #endif
            return;
        }

        // Instanciar al jugador en la posición de la estatua, ya que es el punto de inicio en cualquier caso.
        var player = Instantiate(playerPrefab, statueSpawnPoint.position, Quaternion.identity);

        // Publicar el evento de que el jugador ha sido instanciado
        GameEventBus.Instance.Publish(new PlayerSpawnedEvent(player));

        // Si el jugador viene de una derrota, se aplican los datos guardados (que incluyen vida baja, etc.).
        if (playerPersistentData.CameFromDefeat)
        {
            playerPersistentData.ApplyToPlayer(player, itemDatabase);
            playerPersistentData.CameFromDefeat = false; // Resetear el flag
        }
        // Si no viene de una derrota, puede ser la primera vez o una carga normal.
        else
        {
            // Si no hay un arma equipada guardada, asumimos que es la primera vez que se juega.
            if (string.IsNullOrEmpty(playerPersistentData.EquippedWeaponId))
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
    }
}
