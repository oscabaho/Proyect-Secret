using UnityEngine;
using Base;

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
        private float lastAttackTime = -999f;

        /// <summary>
        /// Intenta atacar en la dirección del transform.forward.
        /// </summary>
        public void TryAttack()
        {
            if (Time.time - lastAttackTime < attackCooldown)
                return;

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
    }
}
