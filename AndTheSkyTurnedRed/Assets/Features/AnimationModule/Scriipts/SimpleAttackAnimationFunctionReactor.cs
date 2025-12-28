using System;
using UnityEngine;

namespace Features.AnimationModule.Scriipts
{
    public class SimpleAttackAnimationFunctionReactor : MonoBehaviour
    {
        public event Action OnAttack;
        public event Action OnEndAttack;

        private void InvokeOnAttack() => OnAttack?.Invoke();

        private void InvokeOnAttackEnded() => OnEndAttack?.Invoke();
    }
}