using UnityEngine;
using ProyectSecret.Interfaces;

namespace ProyectSecret.MonoBehaviours.Player
{
    /// <summary>
    /// Gestiona la interacción del jugador con el mundo (NPCs, objetos, etc.).
    /// </summary>
    [RequireComponent(typeof(PlayerInputController))]
    public class PlayerInteractionController : MonoBehaviour
    {
        private InteractionDetector interactionDetector;
        private IInteractable currentInteractable;
        private PlayerInputController inputController;

        private void Awake()
        {
            interactionDetector = GetComponentInChildren<InteractionDetector>();
            inputController = GetComponent<PlayerInputController>();

            if (interactionDetector == null)
            {
                Debug.LogError("PlayerInteractionController: No se encontró un InteractionDetector en los hijos. Este componente es necesario para detectar objetos interactuables.", this);
                enabled = false;
            }
        }

        private void OnEnable()
        {
            if (inputController != null)
                inputController.OnInteractPressed += PerformInteraction;
        }

        private void OnDisable()
        {
            if (inputController != null)
                inputController.OnInteractPressed -= PerformInteraction;
        }

        private void Update()
        {
            // Constantemente revisa si hay algo interactuable en frente.
            if (interactionDetector != null)
            {
                // El detector nos da el interactuable más cercano en el trigger.
                // Podrías añadir lógica adicional para ver si está en el campo de visión.
                currentInteractable = interactionDetector.GetClosestInteractable(transform);
                // Aquí podrías mostrar/ocultar un mensaje en la UI si currentInteractable no es nulo.
            }
        }

        // Este método solo se llama cuando se presiona el botón de interactuar.
        private void PerformInteraction()
        {
            // Si tenemos un interactuable a la vista, llamamos a su método Interact.
            if (currentInteractable != null)
            {
                currentInteractable.Interact(gameObject);
            }
        }
    }
}
