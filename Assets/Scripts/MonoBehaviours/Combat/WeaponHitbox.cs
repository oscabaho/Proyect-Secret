using UnityEngine;
using ProyectSecret.Inventory;
using ProyectSecret.Interfaces;
using ProyectSecret.VFX;
using ProyectSecret.Events;

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

                // Publicar un evento de impacto para que otros sistemas (audio, vfx) reaccionen.
                GameEventBus.Instance?.Publish(new HitboxImpactEvent(weaponInstance.WeaponData, impactPoint, other.gameObject));

                // --- Lógica de Juego ---
                weaponInstance.WeaponData.ApplyDamage(owner, other.gameObject);
                weaponInstance.AddHit();

                // La pérdida de durabilidad puede depender del objeto golpeado.
                int durabilityLoss = 1; // Pérdida por defecto.
                var weaponDamager = other.GetComponent<IWeaponDamager>();
                if (weaponDamager != null)
                {
                    durabilityLoss = weaponDamager.GetDurabilityDamage();
                }
                weaponInstance.DecreaseDurability(durabilityLoss);
                
                DisableDamage();
            }
        }
    }
}
