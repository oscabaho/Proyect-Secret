using UnityEngine;

namespace Components
{
    /// <summary>
    /// Componente MonoBehaviour para exponer y gestionar StaminaComponent en un GameObject.
    /// </summary>
    public class StaminaComponentBehaviour : MonoBehaviour
    {
        [SerializeField]
        private StaminaComponent stamina = new StaminaComponent();
        public StaminaComponent Stamina => stamina;

        private void Awake()
        {
            // Inicialización si es necesario
        }
    }
}
