using UnityEngine;

namespace Features.CoroutineRunnerModule.Scripts {
    public class CoroutineRunner : MonoBehaviour, ICoroutineRunner {
        public void OnDestroy() =>
            StopAllCoroutines();
    }
}
