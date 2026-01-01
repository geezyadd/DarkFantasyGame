using System;
using UnityEngine;

namespace Features.AnimationModule.Scriipts
{
    public class SimpleAttackAnimationFunctionReactor : MonoBehaviour
    {
        public event Action OnAttack;
        public event Action OnEndAttack;
        
        public event Action OnEndTurn;

        private void InvokeOnAttack() => OnAttack?.Invoke();

        private void InvokeOnAttackEnded() => OnEndAttack?.Invoke();
        private void InvokeOnTurnEnded() => OnEndTurn?.Invoke();
    }
}