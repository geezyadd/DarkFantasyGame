using UnityEngine;

namespace Features.WeaponModule.Scripts
{
    public abstract class WeaponBase : MonoBehaviour
    {
        public abstract void Attack();
        public abstract void StopAttack();
    }
}
