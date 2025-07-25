using System;
using ProyectSecret.Stats;
using UnityEngine;

namespace ProyectSecret.Components
{
    /// <summary>
    /// Componente concreto para la vida, hereda de StatComponent.
    /// </summary>
    [Serializable]
    public class HealthComponent : StatComponent
    {
        // Puedes agregar lógica específica de vida aquí si lo necesitas.
        // El método SetValue(int) se hereda de StatComponent.
    }
}
