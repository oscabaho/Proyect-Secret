using UnityEngine;
using ProyectSecret.Inventory;
using ProyectSecret.Interfaces;
using ProyectSecret.Managers;
using ProyectSecret.Inventory.Items;

namespace ProyectSecret.Combat.Behaviours
{
    /// <summary>
    /// Script para el GameObject de un arma (espada, hacha, daga). Detecta colisiones con enemigos y aplica daño usando el WeaponItem equipado.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class WeaponHitbox : MonoBehaviour
    {
        private WeaponInstance weaponInstance;
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
                Vector3 impactPoint = other.ClosestPoint(transform.position);

                // --- Lógica de Efectos Separada ---
                // 1. Llamar al SoundManager para el sonido.
                if (SoundManager.Instancia != null && weaponInstance.WeaponData.ImpactSound != null)
                {
                    SoundManager.Instancia.ReproducirEfectoEnPunto(
                        weaponInstance.WeaponData.ImpactSound, 
                        impactPoint, 
                        weaponInstance.WeaponData.SoundVolume);
                }

                // 2. Llamar al VFXManager para las partículas.
                VFXManager.Instance?.PlayImpactEffect(impactPoint);

                // --- Lógica de Juego ---
                weaponInstance.WeaponData.ApplyDamage(owner, other.gameObject);
                weaponInstance.AddHit();
                weaponInstance.DecreaseDurability(1);

                DisableDamage();
            }
        }
    }
}
