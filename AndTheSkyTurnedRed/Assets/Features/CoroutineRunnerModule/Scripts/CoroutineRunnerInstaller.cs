using UnityEngine;
using Zenject;

namespace Features.CoroutineRunnerModule.Scripts
{
    public class CoroutineRunnerInstaller : Installer<CoroutineRunnerInstaller>
    {
        public override void InstallBindings()
        {
            GameObject coroutineRunnerGameObject = new GameObject("CoroutineRunner");
            CoroutineRunner runner = coroutineRunnerGameObject.AddComponent<CoroutineRunner>();
            Container.Bind<ICoroutineRunner>().FromInstance(runner).AsSingle();
        }
    }
}