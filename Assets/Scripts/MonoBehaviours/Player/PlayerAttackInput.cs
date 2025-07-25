using UnityEngine;
using ProyectSecret.Combat.Behaviours;
using ProyectSecret.Characters.Player;
using ProyectSecret.MonoBehaviours.Player;

/// <summary>
/// Controlador de input de ataque para el jugador. Escucha los eventos de input y los traduce en ataques usando AttackComponent.
/// </summary>
[RequireComponent(typeof(PlayerEquipmentController), typeof(PlayerInputController))]
public class PlayerAttackInput : MonoBehaviour
{
    private PlayerEquipmentController equipmentController;
    private PlayerInputController inputController;

    private void Awake()
    {
        equipmentController = GetComponent<PlayerEquipmentController>();
        inputController = GetComponent<PlayerInputController>();
    }

    private void OnEnable()
    {
        if (inputController != null)
            inputController.OnAttackPressed += OnAttack;
    }

    private void OnDisable()
    {
        if (inputController != null)
            inputController.OnAttackPressed -= OnAttack;
    }

    // Este método se llama automáticamente solo cuando se presiona el botón de ataque.
    private void OnAttack()
    {
        if (equipmentController != null && equipmentController.CanAttack())
        {
            equipmentController.PerformAttack();
        }
    }
}