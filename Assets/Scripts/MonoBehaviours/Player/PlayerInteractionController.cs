using UnityEngine;
using UnityEngine.InputSystem;
using ProyectSecret.Interfaces;

namespace ProyectSecret.MonoBehaviours.Player
{
    /// <summary>
    /// Gestiona la interacción del jugador con el mundo (NPCs, objetos, etc.).
    /// </summary>
    public class PlayerInteractionController : MonoBehaviour
    {
        [Header("Input")]
        [Tooltip("El asset de Input Actions que contiene la acción de interactuar.")]
        [SerializeField] private InputActionAsset inputActions;
        [Tooltip("El nombre de la acción de interactuar definida en el Input Action Asset.")]
        [SerializeField] private string interactionActionName = "Interact";
        
        private Camera mainCamera;
        private InteractionDetector interactionDetector;
        private InputAction interactAction;
        private IInteractable currentInteractable;

        private void Awake()
        {
            mainCamera = Camera.main;
            interactionDetector = GetComponentInChildren<InteractionDetector>();

            if (inputActions != null)
            {
                // Asumimos que la acción está en el mapa "PlayerDay"
                var actionMap = inputActions.FindActionMap("PlayerDay");
                if (actionMap != null)
                {
                    interactAction = actionMap.FindAction(interactionActionName);
                }
            }

            if (interactionDetector == null)
            {
                Debug.LogError("PlayerInteractionController: No se encontró un InteractionDetector en los hijos. Este componente es necesario para detectar objetos interactuables.", this);
            }
        }

        private void OnEnable()
        {
            if (interactAction != null)
            {
                interactAction.Enable();
                interactAction.performed += PerformInteraction;
            }
        }

        private void OnDisable()
        {
            if (interactAction != null)
            {
                interactAction.performed -= PerformInteraction;
                interactAction.Disable();
            }
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
        private void PerformInteraction(InputAction.CallbackContext context)
        {
            // Si tenemos un interactuable a la vista, llamamos a su método Interact.
            if (currentInteractable != null)
            {
                currentInteractable.Interact(gameObject);
            }
        }
    }
}
