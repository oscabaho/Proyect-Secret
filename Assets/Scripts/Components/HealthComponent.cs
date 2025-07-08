using System;
using ProyectSecret.Stats;
using UnityEngine;

namespace ProyectSecret.Components
{
    [Serializable]
    public class HealthComponent : StatComponent
    {
        public override void AffectValue(int value)
        {
            base.AffectValue(value);
            Debug.Log($"HealthComponent: Vida actual = {CurrentValue}");
        }
    }
}
