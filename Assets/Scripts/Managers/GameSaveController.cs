using UnityEngine;
using ProyectSecret.Events;
using ProyectSecret.Combat.SceneManagement; // Necesario para PlayerPersistentData

namespace ProyectSecret.Managers
{
    /// <summary>
    /// Gestiona los eventos de guardado automático del juego.
    /// Escucha eventos del juego (como el inicio del día) y dispara el guardado.
    /// </summary>
    public class GameSaveController : MonoBehaviour
    {
        [Header("Datos Persistentes")]
        [Tooltip("Arrastra aquí el ScriptableObject 'PlayerPersistentData' que contiene el estado del jugador.")]
        [SerializeField] private PlayerPersistentData playerPersistentData;

        private void Start()
        {
            // Nos suscribimos al bus de eventos para escuchar cuando empieza el día.
            GameEventBus.Instance.Subscribe<DayStartedEvent>(OnDayStarted);
        }

        private void OnDestroy()
        {
            // Es una buena práctica desuscribirse para evitar errores.
            if (GameEventBus.Instance != null)
            {
                GameEventBus.Instance.Unsubscribe<DayStartedEvent>(OnDayStarted);
            }
        }

        /// <summary>
        /// Método que se ejecuta cuando se recibe el evento de que ha comenzado el día.
        /// </summary>
        private void OnDayStarted(DayStartedEvent evt)
        {
            Debug.Log("Comienza el día. Iniciando autoguardado...");
            SaveCurrentGameState();
        }

        /// <summary>
        /// Recopila los datos actuales del jugador y los guarda usando el SaveLoadManager.
        /// </summary>
        public void SaveCurrentGameState()
        {
            if (playerPersistentData == null)
            {
                Debug.LogError("GameSaveController: No se ha asignado el 'PlayerPersistentData' en el Inspector. No se puede guardar.");
                return;
            }

            var playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                playerPersistentData.SaveFromPlayer(playerObject);
            }
            
            SaveLoadManager.SaveGame(playerPersistentData);
        }
    }
}