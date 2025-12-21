using System;
using UnityEngine;

namespace Features.DamageableModule.Scripts
{
    public class MockDamageable : MonoBehaviour, IDamageable
    {
        public event Action<DamageData> OnDamage;

        public void Damage(DamageData damageData)
        {
            OnDamage?.Invoke(damageData);
        }
    }

    public class DamageData
    {
        public Transform DamageDealer;

        public float Damage;
        public DamageData(float damage, Transform damageDealer)
        {
            Damage = damage;
            DamageDealer = damageDealer;
        }
    }
}