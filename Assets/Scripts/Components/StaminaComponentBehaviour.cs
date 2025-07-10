using UnityEngine;

namespace ProyectSecret.Components
{
    /// <summary>
    /// Componente MonoBehaviour para exponer y gestionar StaminaComponent en un GameObject.
    /// </summary>
    [DisallowMultipleComponent]
    public class StaminaComponentBehaviour : MonoBehaviour, ProyectSecret.Interfaces.IStatController
    {
        [SerializeField]
        private StaminaComponent stamina = new StaminaComponent();
        public StaminaComponent Stamina => stamina;

        public int CurrentValue => stamina.CurrentValue;
        public int MaxValue => stamina.MaxValue;
        public void AffectValue(int value) => stamina.AffectValue(value);

        private void Awake()
        {
            // Inicialización si es necesario
        }
    }
}
