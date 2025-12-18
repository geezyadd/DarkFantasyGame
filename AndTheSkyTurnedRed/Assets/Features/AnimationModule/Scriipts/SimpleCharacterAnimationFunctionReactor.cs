using System;
using UnityEngine;

namespace Features.AnimationModule.Scriipts
{
    public class SimpleCharacterAnimationFunctionReactor : MonoBehaviour
    {
        public event Action OnSimpleAttackEnded;
        public event Action OnTurnEnded;
        private void InvokeSimpleAttackEnded() {
            OnSimpleAttackEnded?.Invoke();
        }
        
        private void InvokeTurnEnded() {
            OnTurnEnded?.Invoke();
        }
    }
}