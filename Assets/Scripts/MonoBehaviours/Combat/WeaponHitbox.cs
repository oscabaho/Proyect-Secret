
using UnityEngine;
using ProyectSecret.Inventory;
using ProyectSecret.Interfaces;
using ProyectSecret.Inventory.Items;

namespace ProyectSecret.Combat.Behaviours
{
    /// <summary>
    /// Script para el GameObject de un arma (espada, hacha, daga). Detecta colisiones con enemigos y aplica daño usando el WeaponItem equipado.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class WeaponHitbox : MonoBehaviour
    {
        [SerializeField] private WeaponItem weaponData; // Asignar desde el prefab o dinámicamente
        [SerializeField] private GameObject owner; // El jugador o entidad que porta el arma
        private bool canDamage = false;

        public void SetWeapon(WeaponItem weapon, GameObject weaponOwner)
        {
            weaponData = weapon;
            owner = weaponOwner;
        }

        public void EnableDamage() => canDamage = true;
        public void DisableDamage() => canDamage = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!canDamage || weaponData == null || owner == null)
                return;
            if (other.gameObject == owner)
                return; // No dañarse a sí mismo
            var damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                weaponData.ApplyDamage(owner, other.gameObject);
            }
        }
    }
}
