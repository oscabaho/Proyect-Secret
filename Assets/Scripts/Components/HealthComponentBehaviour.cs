using UnityEngine;

namespace Components
{
    /// <summary>
    /// Componente MonoBehaviour para exponer y gestionar HealthComponent en un GameObject.
    /// </summary>
    public class HealthComponentBehaviour : MonoBehaviour
    {
        [SerializeField]
        private HealthComponent health = new HealthComponent();
        public HealthComponent Health => health;

        private void Awake()
        {
            // Inicialización si es necesario
        }
    }
}
