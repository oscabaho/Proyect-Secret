using UnityEngine;
using ProyectSecret.Combat.SceneManagement; // Añadir esta línea

namespace ProyectSecret.Triggers
{
    /// <summary>
    /// Guarda el estado del jugador (incluida la posición) cuando una entidad con un tag específico entra en este trigger.
    /// Ideal para colocar en los triggers que inician el combate.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class SavePlayerStateOnTrigger : MonoBehaviour
    {
        [Header("Configuración")]
        [Tooltip("El ScriptableObject que contiene los datos persistentes del jugador.")]
        [SerializeField] private PlayerPersistentData playerPersistentData;
        
        [Tooltip("La etiqueta del GameObject que activará el guardado (generalmente 'Player').")]
        [SerializeField] private string targetTag = "Player";

        private void Awake()
        {
            // Asegurarse de que el collider es un trigger para que OnTriggerEnter funcione.
            var col = GetComponent<Collider>();
            if (col != null && !col.isTrigger)
            {
                col.isTrigger = true;
                #if UNITY_EDITOR
                Debug.LogWarning($"El collider en {gameObject.name} no era un trigger. Se ha activado automáticamente.", this);
                #endif
            }

            // Validar en Awake para detectar errores de configuración temprano.
            if (playerPersistentData == null)
            {
                #if UNITY_EDITOR
                Debug.LogError($"PlayerPersistentData no está asignado en el Inspector de '{gameObject.name}'. El estado del jugador no se guardará.", this);
                #endif
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Si el objeto que entra no tiene el tag correcto, no hacemos nada.
            if (!other.CompareTag(targetTag))
                return;

            if (playerPersistentData != null)
            {
                // Guardamos todos los datos del jugador, incluida su posición actual.
                playerPersistentData.SaveFromPlayer(other.gameObject, savePosition: true);
                #if UNITY_EDITOR
                Debug.Log($"Estado y posición del jugador guardados en ({playerPersistentData.LastPosition}) por el trigger '{gameObject.name}'.", this);
                #endif
            }
            // El error si playerPersistentData es null ya se muestra en Awake, por lo que no es necesario repetirlo aquí.
        }
    }
}
