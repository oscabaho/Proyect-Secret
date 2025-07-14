using UnityEngine;

namespace ProyectSecret.Components
{
    /// <summary>
    /// Componente MonoBehaviour para exponer y gestionar HealthComponent en un GameObject.
    /// </summary>
    [DisallowMultipleComponent]
    public class HealthComponentBehaviour : MonoBehaviour, ProyectSecret.Interfaces.IStatController
    {
        [SerializeField]
        private HealthComponent health = new HealthComponent();
        public HealthComponent Health { get { return health; } }

        public int CurrentValue => health.CurrentValue;
        public int MaxValue => health.MaxValue;
        public void AffectValue(int value) => health.AffectValue(value);

        private void Awake()
        {
            // Inicialización si es necesario
        }
    }
}
