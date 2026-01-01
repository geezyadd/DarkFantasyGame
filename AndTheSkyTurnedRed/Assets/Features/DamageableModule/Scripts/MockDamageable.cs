using System;
using UnityEngine;

namespace Features.DamageableModule.Scripts
{
    public class MockDamageable : MonoBehaviour, IDamageable
    {
        public bool IsActive { get; set; }

        public Transform Transform =>
            transform;
        public event Action<DamageData> OnDamage;

        public void Damage(DamageData damageData)
        {
            OnDamage?.Invoke(damageData);
        }
    }
}