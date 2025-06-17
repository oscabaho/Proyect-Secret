using UnityEngine;
using Components;
using Interfaces;

namespace Combat.Behaviours
{
    /// <summary>
    /// Componente modular para lógica de ataque. Puede ser añadido a cualquier entidad.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class AttackComponent : MonoBehaviour
    {
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private int damage = 10;
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private int staminaCost = 10;
        [SerializeField] private StaminaComponent staminaComponent;
        private float lastAttackTime = -999f;

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
                    var damageable = hit.transform.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(damage);
                        Debug.Log($"{gameObject.name} atacó a {hit.transform.name} por {damage} de daño.");
                    }
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
