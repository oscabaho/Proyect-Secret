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
        [Header("Configuración de Interacción")]
        [SerializeField] private float interactionDistance = 5f;
        [SerializeField] private LayerMask interactionLayer;

        [Header("Input")]
        [Tooltip("El asset de Input Actions que contiene la acción de interactuar.")]
        [SerializeField] private InputActionAsset inputActions;
        [Tooltip("El nombre de la acción de interactuar definida en el Input Action Asset.")]
        [SerializeField] private string interactionActionName = "Interact";
        
        private Camera mainCamera;
        private InputAction interactAction;
        private IInteractable currentInteractable;

        private void Awake()
        {
            mainCamera = Camera.main;
            if (inputActions != null)
            {
                // Asumimos que la acción está en el mapa "PlayerDay"
                var actionMap = inputActions.FindActionMap("PlayerDay");
                if (actionMap != null)
                {
                    interactAction = actionMap.FindAction(interactionActionName);
                }
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
            CheckForInteractable();
        }

        private void CheckForInteractable()
        {
            if (mainCamera == null) return;

            RaycastHit hit;
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, interactionDistance, interactionLayer))
            {
                // Si encontramos algo, lo guardamos como el interactuable actual.
                currentInteractable = hit.collider.GetComponent<IInteractable>();
                // Aquí podrías mostrar un mensaje en la UI como "[E] Hablar"
            }
            else
            {
                // Si no hay nada, limpiamos la referencia.
                currentInteractable = null;
                // Aquí podrías ocultar el mensaje de la UI.
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
