using System;
using UnityEngine;

namespace Features.AnimationModule.Scriipts
{
    public class SimpleAnimationController : SimpleAnimationControllerBase
    {
        [SerializeField] private Animator _animator;
        public override void SetTrigger(string trigger) => _animator.SetTrigger(trigger);
        public override void ResetTrigger(string trigger) => _animator.ResetTrigger(trigger);
        public override void SetFloat(string trigger, float value) => _animator.SetFloat(trigger, value);
        public override void SetInt(string trigger, int value) => _animator.SetInteger(trigger, value);
        public override void SetBool(string trigger, bool value) => _animator.SetBool(trigger, value);
        public override void SetVector3(string trigger, Vector3 value) => _animator.SetTrigger(trigger);

        public override void SetLayerWeight(string layerName, float weight) {
            int layerIndex = _animator.GetLayerIndex(layerName);
            _animator.SetLayerWeight(layerIndex, weight); 
        }

        public override float GetLayerWeight(string layerName) {
            int layerIndex = _animator.GetLayerIndex(layerName);
            return _animator.GetLayerWeight(layerIndex);
        }

        public override AnimatorStateInfo GetCurrentAnimatorStateInfo(string layerName)
        {
            int layerIndex = _animator.GetLayerIndex(layerName);
            return _animator.GetCurrentAnimatorStateInfo(layerIndex);
        }

        public override AnimatorStateInfo GetNextAnimatorStateInfo(string layerName)
        {
            int layerIndex = _animator.GetLayerIndex(layerName);
            return _animator.GetNextAnimatorStateInfo(layerIndex);
        }

        public override bool IsInTransition(string layerName)
        {
            int layerIndex = _animator.GetLayerIndex(layerName);
            return _animator.IsInTransition(layerIndex);
        }
    }
}
