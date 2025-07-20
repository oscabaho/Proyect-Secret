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
        private WeaponInstance weaponInstance; // Ahora referencia la instancia, no solo los datos.
        private GameObject owner;
        private bool canDamage = false;

        public void Initialize(WeaponInstance instance, GameObject weaponOwner)
        {
            weaponInstance = instance;
            owner = weaponOwner;
        }

        public void EnableDamage() => canDamage = true;
        public void DisableDamage() => canDamage = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!canDamage || weaponInstance == null || owner == null)
                return;
            
            if (other.gameObject == owner)
                return; // No dañarse a sí mismo
            
            var damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                // Aplicar daño usando los datos del arma
                weaponInstance.WeaponData.ApplyDamage(owner, other.gameObject);
                
                // Incrementar contador de golpes y reducir durabilidad
                weaponInstance.AddHit();
                weaponInstance.DecreaseDurability(1); // O un valor configurable desde WeaponData

                // Desactivar el daño para que no golpee varias veces en un solo swing.
                DisableDamage();
            }
        }
    }
}
