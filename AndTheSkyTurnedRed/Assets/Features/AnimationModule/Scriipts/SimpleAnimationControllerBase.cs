using UnityEngine;

namespace Features.AnimationModule.Scriipts
{
    public abstract class SimpleAnimationControllerBase : MonoBehaviour
    {
        public abstract void SetTrigger(string trigger);
        public abstract void ResetTrigger(string trigger);
        public abstract void SetFloat(string trigger, float value);
        public abstract void SetInt(string trigger, int value);
        public abstract void SetBool(string trigger, bool value);
        public abstract void SetVector3(string trigger, Vector3 value);
        public abstract void SetLayerWeight(string layerName, float weight);
        public abstract float GetLayerWeight(string layerName);
        public abstract AnimatorStateInfo GetCurrentAnimatorStateInfo(string layerName);
        public abstract AnimatorStateInfo GetNextAnimatorStateInfo(string layerName);
        public abstract bool IsInTransition(string layerName);
    }
}