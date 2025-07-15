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
        var inputActionAsset = InputSystem.actions;
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
