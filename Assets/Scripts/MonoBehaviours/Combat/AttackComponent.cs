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
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private int staminaCost = 10;
        [SerializeField] private StaminaComponentBehaviour staminaBehaviour;
        [SerializeField] private PlayerEquipmentController equipmentController;
        private float lastAttackTime = -999f;

        private void Awake()
        {
            if (staminaBehaviour == null)
                staminaBehaviour = GetComponent<StaminaComponentBehaviour>();
            if (equipmentController == null)
                equipmentController = GetComponent<Inventory.PlayerEquipmentController>();
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
