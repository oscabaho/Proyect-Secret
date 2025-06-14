using System;
using Base;
using UnityEngine;

namespace Components
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
