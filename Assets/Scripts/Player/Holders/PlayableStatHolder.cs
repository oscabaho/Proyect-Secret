using UnityEngine;
using Components;

namespace Holders
{
    /// <summary>
    /// Portador jugable: puede tener vida y/o stamina por asociación.
    /// </summary>
    public class PlayableStatHolder : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private HealthComponent health;
        [SerializeReference] private StaminaComponent stamina;
        [SerializeField] private GameObject explosionVFXPrefab;
        [SerializeField] private GameObject projectilePrefab;

        public HealthComponent Health => health;
        public StaminaComponent Stamina => stamina;

        public void Initialize(HealthComponent health = null, StaminaComponent stamina = null)
        {
            if (health != null) this.health = health;
            if (stamina != null) this.stamina = stamina;
        }

        public void TakeDamage(int amount)
        {
            health?.AffectValue(-amount);
        }
    }
}
