using System;

namespace Features.DamageableModule.Scripts
{
    public interface IDamageable
    {
        public event Action<DamageData> OnDamage;
        public void Damage(DamageData damageData);
    }
}
