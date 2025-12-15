using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace SceneLoaderModule {
    public interface ISceneSwitchService {
        public event Action<string> OnSceneLoaded;
        public event Action<string> OnSceneUnloaded;
        public UniTask UpdateLoadedScenes(IReadOnlyList<string> subScenes, string mainScene);
        public UniTask LoadScene(string newScene, LoadSceneMode loadSceneMode);
        public UniTask UnloadScene(string sceneToUnload, UnloadSceneOptions unloadSceneOptions);
    }
}