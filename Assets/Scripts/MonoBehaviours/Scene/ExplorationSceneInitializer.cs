using UnityEngine;
using ProyectSecret.Combat.SceneManagement;

/// <summary>
/// Inicializa la escena de exploración y posiciona al jugador en un punto fijo (estatua) si viene de una derrota en combate.
/// </summary>
public class ExplorationSceneInitializer : MonoBehaviour
{
    [Header("Referencia al jugador")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform statueSpawnPoint;
    [SerializeField] private PlayerPersistentData playerPersistentData;

    void Start()
    {
        // Determina si el jugador viene de una derrota
        if (playerPersistentData != null && playerPersistentData.CameFromDefeat)
        {
            // Instancia el jugador en el punto de la estatua
            var player = Instantiate(playerPrefab, statueSpawnPoint.position, Quaternion.identity);
            playerPersistentData.ApplyToPlayer(player, null); // Aplica datos persistentes si es necesario
            playerPersistentData.CameFromDefeat = false; // Resetea el flag
        }
        else
        {
            // Instancia el jugador en el punto normal (puedes adaptar esto)
            var player = Instantiate(playerPrefab, statueSpawnPoint.position, Quaternion.identity);
            playerPersistentData.ApplyToPlayer(player, null);
        }
    }
}
