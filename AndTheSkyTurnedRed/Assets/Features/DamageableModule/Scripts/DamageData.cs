using UnityEngine;

namespace Features.DamageableModule.Scripts
{
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