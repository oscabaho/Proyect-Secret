using UnityEngine;
using ProyectSecret.Characters;
using ProyectSecret.Combat.Behaviours;
using ProyectSecret.Inventory;
using ProyectSecret.MonoBehaviours.Player;

namespace ProyectSecret.Characters.Player
{
    /// <summary>
    /// Componente orquestador principal para el jugador.
    /// Añadir este único script a un GameObject asegurará que todos los
    /// componentes necesarios para el jugador se añadan automáticamente.
    /// </summary>
    [RequireComponent(typeof(PaperMarioPlayerMovement))]
    [RequireComponent(typeof(PlayerHealthController))]
    [RequireComponent(typeof(PlayerEquipmentController))]
    [RequireComponent(typeof(PlayerInventory))]
    [RequireComponent(typeof(PlayerAttackInput))]
    [RequireComponent(typeof(AttackComponent))]
    [RequireComponent(typeof(PlayerPointSwitcher))]
    [RequireComponent(typeof(PlayerCameraController))]
    // Nota: Los componentes como Rigidbody, HealthComponentBehaviour, etc.,
    // ya son requeridos por los scripts de arriba, por lo que se añadirán en cadena.
    public class PlayerController : MonoBehaviour
    {
        // Este script puede permanecer vacío. Su único propósito es
        // forzar la adición de los componentes necesarios a través del
        // atributo [RequireComponent].
    }
}
