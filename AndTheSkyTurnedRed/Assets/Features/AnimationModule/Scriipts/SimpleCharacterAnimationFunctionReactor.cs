using System;
using UnityEngine;

namespace Features.AnimationModule.Scriipts
{
    public class SimpleCharacterAnimationFunctionReactor : MonoBehaviour
    {
        public event Action OnSimpleAttackEnded;
        private void InvokeSimpleAttackEnded() {
            OnSimpleAttackEnded?.Invoke();
        }
    }
}