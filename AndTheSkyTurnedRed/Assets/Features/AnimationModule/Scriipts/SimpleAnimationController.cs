using System;
using UnityEngine;

namespace Features.AnimationModule.Scriipts
{
    public class SimpleAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        public void SetTrigger(string trigger) => _animator.SetTrigger(trigger);
        public void ResetTrigger(string trigger) => _animator.ResetTrigger(trigger);
        public void SetFloat(string trigger, float value) => _animator.SetFloat(trigger, value);
        public void SetInt(string trigger, int value) => _animator.SetInteger(trigger, value);
        public void SetBool(string trigger, bool value) => _animator.SetBool(trigger, value);
        public void SetVector3(string trigger, Vector3 value) => _animator.SetTrigger(trigger);

        public void SetLayerWeight(string layerName, float weight) {
            int layerIndex = _animator.GetLayerIndex(layerName);
            _animator.SetLayerWeight(layerIndex, weight); 
        }

        public float GetLayerWeight(string layerName) {
            int layerIndex = _animator.GetLayerIndex(layerName);
            return _animator.GetLayerWeight(layerIndex);
        }

        public AnimatorStateInfo GetCurrentAnimatorStateInfo(string layerName)
        {
            int layerIndex = _animator.GetLayerIndex(layerName);
            return _animator.GetCurrentAnimatorStateInfo(layerIndex);
        }

        public AnimatorStateInfo GetNextAnimatorStateInfo(string layerName)
        {
            int layerIndex = _animator.GetLayerIndex(layerName);
            return _animator.GetNextAnimatorStateInfo(layerIndex);
        }

        public bool IsInTransition(string layerName)
        {
            int layerIndex = _animator.GetLayerIndex(layerName);
            return _animator.IsInTransition(layerIndex);
        }
    }
}
