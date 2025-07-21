using UnityEngine;
using ProyectSecret.Components;
using ProyectSecret.Inventory;
using ProyectSecret.Events;

namespace ProyectSecret.Combat.Behaviours
{
    /// <summary>
    /// Componente modular para lógica de ataque. Puede ser añadido a cualquier entidad.
    /// </summary>
    public class AttackComponent : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private int staminaCost = 10;

        [Header("Dependencies")]
        [Tooltip("Componente de estamina del que se descontará el coste del ataque.")]
        [SerializeField] private StaminaComponentBehaviour staminaBehaviour;
        [Tooltip("Controlador de equipamiento que gestiona el arma a usar.")]
        [SerializeField] private PlayerEquipmentController equipmentController;
        
        private float lastAttackTime = -999f;

        private void Awake()
        {
            // Es mejor validar las dependencias que asignarlas silenciosamente.
            // Esto fuerza a una configuración correcta desde el Inspector y previene errores.
            if (staminaBehaviour == null)
            {
                Debug.LogError("AttackComponent: StaminaComponentBehaviour no está asignado en el Inspector.", this);
            }
            if (equipmentController == null)
            {
                Debug.LogError("AttackComponent: PlayerEquipmentController no está asignado en el Inspector.", this);
            }
        }

        /// <summary>
        /// Inicia el ataque: activa el collider del arma equipada y consume stamina.
        /// </summary>
        public void TryAttack()
        {
            if (Time.time - lastAttackTime < attackCooldown)
                return;

            if (equipmentController == null || !equipmentController.CanAttack())
                return;

            if (staminaBehaviour == null || !staminaBehaviour.Stamina.HasEnough(staminaCost))
            {
                #if UNITY_EDITOR
                Debug.Log("No hay suficiente stamina para atacar.");
                #endif
                return;
            }

            if (equipmentController.Attack())
            {
                staminaBehaviour.Stamina.UseStamina(staminaCost);
                lastAttackTime = Time.time;

                // Publicar un evento para notificar que se usó estamina.
                // PlayerHealthController escuchará este evento para reiniciar su contador.
                GameEventBus.Instance.Publish(new PlayerActionUsedStaminaEvent(gameObject, staminaCost));
            }
        }
    }
}
