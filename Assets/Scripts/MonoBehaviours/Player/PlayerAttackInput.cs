using UnityEngine;
using UnityEngine.InputSystem;
using ProyectSecret.Combat.Behaviours;

/// <summary>
/// Controlador de input de ataque para el jugador. Traduce el input y la dirección de la cámara en ataques usando AttackComponent.
/// </summary>
[RequireComponent(typeof(AttackComponent))]
public class PlayerAttackInput : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private string attackActionName = "Attack";
    [SerializeField] private InputActionAsset inputActions; // Asigna el mismo asset que el movimiento
    private InputAction attackAction;
    private AttackComponent attackComponent;

    private void Awake()
    {
        attackComponent = GetComponent<AttackComponent>();
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera == null)
        {
            #if UNITY_EDITOR
            Debug.LogWarning("PlayerAttackInput: No se encontró cámara en el jugador.");
            #endif
        }

        // Si no se asignó manualmente, intenta obtener el InputActionAsset del movimiento
        if (inputActions == null)
        {
            var movement = GetComponent<PaperMarioPlayerMovement>();
            if (movement != null)
            {
                inputActions = movement.InputActions;
            }
        }
        
        if (inputActions != null)
        {
            attackAction = inputActions.FindAction(attackActionName);
        }
        else
        {
            #if UNITY_EDITOR
            Debug.LogError("PlayerAttackInput: No se pudo encontrar el InputActionAsset. Asegúrate de que esté asignado en el Inspector o en el componente PaperMarioPlayerMovement.");
            #endif
        }
    }

    private void OnEnable()
    {
        if (attackAction != null)
        {
            attackAction.Enable();
            // Nos suscribimos al evento 'performed' en lugar de comprobar en Update.
            attackAction.performed += OnAttack;
        }
    }

    private void OnDisable()
    {
        if (attackAction != null)
        {
            // Nos desuscribimos para evitar memory leaks.
            attackAction.performed -= OnAttack;
            attackAction.Disable();
        }
    }

    // Este método se llama automáticamente solo cuando se presiona el botón de ataque.
    private void OnAttack(InputAction.CallbackContext context)
    {
        if (attackComponent != null)
        {
            attackComponent.TryAttack();
        }
    }
}
