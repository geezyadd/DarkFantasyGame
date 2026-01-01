using System;
using UnityEngine;

namespace Features.DamageableModule.Scripts
{
    public interface IDamageable
    {
        public bool IsActive { get; set; }
        public Transform Transform { get; }
        public event Action<DamageData> OnDamage;
        public void Damage(DamageData damageData);
    }
}
