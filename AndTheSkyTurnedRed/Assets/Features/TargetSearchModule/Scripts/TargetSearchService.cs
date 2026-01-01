using System.Collections.Generic;
using Features.DamageableModule.Scripts;
using UnityEngine;

namespace Features.TargetSearchModule.Scripts
{
    public class TargetSearchService : ITargetSearchService{
        public HashSet<IDamageable> GetTargetsInRange(Vector3 initialPosition, float range, LayerMask layerMask) {
            Collider[] colliders = Physics.OverlapSphere(initialPosition, range, layerMask);

            HashSet<IDamageable> damageables = new();
            for (int index = 0; index < colliders.Length; index++) {
                IDamageable damageable = colliders[index].GetComponent<IDamageable>();
                if (damageable != null && damageable.IsActive)
                    damageables.Add(damageable);
            }
            
            return damageables;
        }
    }
}




