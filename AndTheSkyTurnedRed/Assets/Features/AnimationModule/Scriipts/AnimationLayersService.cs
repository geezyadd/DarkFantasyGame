using System;
using System.Collections;
using Features.CoroutineRunnerModule.Scripts;
using UnityEngine;

namespace Features.AnimationModule.Scriipts
{
    public class AnimationLayersService : IAnimationLayersService
    {
        private readonly ICoroutineRunner _coroutineRunner;

        private AnimationLayersService(ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;
        }

        public Coroutine SmoothLayerToZero(SimpleAnimationControllerBase animationController, string layerName, float duration, Action onEnd = null) {
            float startWeight = animationController.GetLayerWeight(layerName);
            return _coroutineRunner.StartCoroutine(SmoothLayerToZeroCoroutine(animationController, layerName, startWeight, 0, duration, onEnd));
        }
        
        public Coroutine SmoothLayerTo(SimpleAnimationControllerBase animationController, string layerName, float startValue, float endValue, float duration, Action onEnd = null) {
            return _coroutineRunner.StartCoroutine(SmoothLayerToZeroCoroutine(animationController, layerName, startValue, endValue, duration, onEnd));
        }

        public void StopSmooth(Coroutine coroutine) => _coroutineRunner.StopCoroutine(coroutine);

        private IEnumerator SmoothLayerToZeroCoroutine(SimpleAnimationControllerBase animationController, string layerName,float startValue, float endValue, float duration, Action onEnd = null)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float weight = Mathf.Lerp(startValue, endValue, t);
                animationController.SetLayerWeight(layerName, weight);
                yield return null;
            }

            animationController.SetLayerWeight(layerName, endValue);
            if(onEnd == null)
                yield break;
            
            onEnd();
        }
    }
}