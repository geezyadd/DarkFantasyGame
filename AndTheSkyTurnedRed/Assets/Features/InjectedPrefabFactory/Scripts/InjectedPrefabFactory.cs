using Features.SceneLoaderModule.Runtime.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Features.InjectedPrefabFactory.Scripts
{
    public class InjectedPrefabFactory : IInjectedPrefabFactory
    {
        private readonly DiContainer _diContainer;

        public InjectedPrefabFactory(DiContainer diContainer) => _diContainer = diContainer;

        public GameObject CreatePrefab(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null) {
            return _diContainer.InstantiatePrefab(prefab, position, rotation, parent);
        }
    }
}
