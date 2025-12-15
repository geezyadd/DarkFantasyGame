using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace SceneLoaderModule {
    public class AddressablesSceneSwitchService : ISceneSwitchService {
        private Dictionary<string, AsyncOperationHandle<SceneInstance>> LoadedScenes { get; set; } = new();
        public event Action<string> OnSceneLoaded;
        public event Action<string> OnSceneUnloaded;

        public async UniTask UpdateLoadedScenes(IReadOnlyList<string> subScenes, string mainScene) {
            await UnloadScenes(GetRedundantScenes(subScenes));
            await LoadScenes(subScenes, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(mainScene));
        }

        public async UniTask LoadScene(string newScene, LoadSceneMode loadSceneMode) {
            if (IsSceneLoaded(newScene))
                return;

            AsyncOperationHandle<SceneInstance> asyncOperationHandler = Addressables.LoadSceneAsync(newScene, loadSceneMode);
            LoadedScenes.Add(newScene, asyncOperationHandler);
            await asyncOperationHandler.Task;
            OnSceneLoaded?.Invoke(newScene);
        }

        public async UniTask UnloadScene(string sceneToUnload, UnloadSceneOptions _) {
            if (IsSceneLoaded(sceneToUnload) is false)
                return;
            
            AsyncOperationHandle<SceneInstance> asyncOperationHandler = Addressables.UnloadSceneAsync(LoadedScenes[sceneToUnload]);
            LoadedScenes.Remove(sceneToUnload);
            await asyncOperationHandler.Task;
            OnSceneUnloaded?.Invoke(sceneToUnload);
        }

        private async UniTask LoadScenes(IReadOnlyList<string> newScenes, LoadSceneMode loadSceneMode) {
            foreach (string newScene in newScenes)
                await LoadScene(newScene, loadSceneMode);
        }

        private async UniTask UnloadScenes(IReadOnlyList<string> newScenes) {
            List<string> scenesToUnload = (from loadedScene in LoadedScenes where !newScenes.Contains(loadedScene.Key) select loadedScene.Key).ToList();

            foreach (string sceneToUnload in scenesToUnload)
                await UnloadScene(sceneToUnload, default);
        }

        private List<string> GetRedundantScenes(IReadOnlyList<string> newScenes) =>
            (from loadedScene in LoadedScenes where !newScenes.Contains(loadedScene.Key) select loadedScene.Key).ToList();

        private bool IsSceneLoaded(string sceneName) =>
            LoadedScenes.ContainsKey(sceneName);
    }
}