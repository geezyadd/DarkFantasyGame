using UnityEngine;

namespace Features.WeaponModule.Scripts
{
    public class SwordWeapon : WeaponBase
    {
        [SerializeField] private Collider _collider;
        public override void Attack()
        {
            _collider.enabled = true;
        }

        public override void StopAttack()
        {
            _collider.enabled = false;
        }
    }
}