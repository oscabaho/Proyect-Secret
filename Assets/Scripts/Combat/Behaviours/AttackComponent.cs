using UnityEngine;
using Base;
using Components;

namespace Combat.Behaviours
{
    /// <summary>
    /// Componente modular para lógica de ataque. Puede ser añadido a cualquier entidad.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class AttackComponent : MonoBehaviour
    {
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private int staminaCost = 10;
        [SerializeField] private StaminaComponent staminaComponent;
        private float lastAttackTime = -999f;
        [SerializeField]private Weapons weapon;

        private void Awake()
        {
            if (staminaComponent == null)
                staminaComponent = GetComponent<StaminaComponent>();
        }

        /// <summary>
        /// Intenta atacar en la dirección del transform.forward, consumiendo stamina si es suficiente.
        /// </summary>
        public void TryAttack()
        {
            if (Time.time - lastAttackTime < attackCooldown)
                return;

            if (staminaComponent != null && staminaComponent.CurrentStamina >= staminaCost)
            {
                staminaComponent.UseStamina(staminaCost);
                lastAttackTime = Time.time;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, attackRange))
                {
                    weapon.Attack(hit);
                }
            }
            else
            {
                Debug.Log("No hay suficiente stamina para atacar.");
                // Opcional: feedback visual/sonoro
            }
        }
    }
}
