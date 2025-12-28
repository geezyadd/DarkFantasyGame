using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace Features.CoroutineRunnerModule.Scripts {
    [PublicAPI]
    public interface ICoroutineRunner {
        public Coroutine StartCoroutine(IEnumerator coroutine);
        public void StopCoroutine(Coroutine inverseCoroutine);
    }
}