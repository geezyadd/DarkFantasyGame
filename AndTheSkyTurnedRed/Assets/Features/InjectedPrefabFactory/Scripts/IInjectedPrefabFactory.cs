using Features.SceneLoaderModule.Runtime.Scripts;
using UnityEngine;

namespace Features.InjectedPrefabFactory.Scripts
{
    public interface IInjectedPrefabFactory
    {
        public GameObject CreatePrefab(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null);
    }
}