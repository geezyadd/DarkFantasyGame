using System;
using UnityEngine;

namespace Features.AnimationModule.Scriipts
{
    public interface IAnimationLayersService
    {
        public Coroutine SmoothLayerToZero(SimpleAnimationControllerBase animationController, string layerName, float duration, Action onEnd = null);
        public Coroutine SmoothLayerTo(SimpleAnimationControllerBase animationController, string layerName, float startValue, float endValue, float duration, Action onEnd = null);
        public void StopSmooth(Coroutine coroutine);
    }
}