using ProyectSecret.Components;

namespace ProyectSecret.Components.NullObjects
{
    /// <summary>
    /// Implementación Null Object para StaminaComponent, evita comprobaciones de null.
    /// </summary>
    public class NullStaminaComponent : StaminaComponent
    {
        public static readonly StaminaComponent Instance = new NullStaminaComponent();
        public override float CurrentStamina => 0f;
        // Si tienes métodos virtuales como UseStamina, descomenta y ajusta:
        // public override void UseStamina(float amount) { }
    }
}
