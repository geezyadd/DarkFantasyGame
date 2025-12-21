using Features.DamageableModule.Scripts;
using UnityEngine;

namespace Features.WeaponModule.Scripts
{
    public class SwordColliderController : MonoBehaviour
    {
        [SerializeField] private LayerMask _layerMask;
        private void OnTriggerEnter(Collider other)
        {
            if ((_layerMask.value & (1 << other.gameObject.layer)) != 0)
            {
                if (other.TryGetComponent(out IDamageable damageable))
                {
                    damageable.Damage(new DamageData(0, transform.parent));
                }
            }
        }
    }
}