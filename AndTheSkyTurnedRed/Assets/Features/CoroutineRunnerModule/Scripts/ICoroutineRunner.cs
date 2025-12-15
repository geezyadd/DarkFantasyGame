using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace Features.CoroutineRunnerModule.Scripts {
    [PublicAPI]
    public interface ICoroutineRunner {
        Coroutine StartCoroutine(IEnumerator coroutine);
        void StopCoroutine(Coroutine inverseCoroutine);
    }
}