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
                var field = typeof(PaperMarioPlayerMovement).GetField("inputActions", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    inputActions = field.GetValue(movement) as InputActionAsset;
                }
            }
        }
        var inputActionAsset = inputActions;
        if (inputActionAsset != null)
            attackAction = inputActionAsset.FindAction(attackActionName);
    }

    private void Update()
    {
        if (attackAction != null && attackAction.WasPressedThisFrame())
        {
            // Opcional: puedes pasar la dirección de la cámara a AttackComponent si lo soporta
            attackComponent.TryAttack();
        }
    }
}
