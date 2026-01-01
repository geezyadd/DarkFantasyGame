using System.Collections.Generic;
using Features.DamageableModule.Scripts;
using UnityEngine;

namespace Features.TargetSearchModule.Scripts
{
    public interface ITargetSearchService
    {
        public HashSet<IDamageable> GetTargetsInRange(Vector3 initialPosition, float range, LayerMask layerMask);
    }
}