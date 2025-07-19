using Characters;
using ProyectSecret.Interfaces;
using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(fileName = "HealingItem", menuName = "Inventory/HealingItem")]
    public class HealingItem : ScriptableObject, IUsableItem
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private int healAmount;

        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => icon;
        public int HealAmount => healAmount;

        public void Use(GameObject user)
        {
            var health = user.GetComponent<ProyectSecret.Characters.PlayerHealthController>();
            if (health != null && health.Health != null)
            {
                health.Health.AffectValue(healAmount);
                #if UNITY_EDITOR
                Debug.Log($"Curado {healAmount} puntos de vida.");
                #endif
            }
            else
            {
                #if UNITY_EDITOR
                Debug.LogWarning("No se pudo curar: PlayerHealthController o HealthComponent no encontrado.");
                #endif
            }
        }

        public string GetId() => Id;
    }
}
