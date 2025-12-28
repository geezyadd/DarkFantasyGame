using Features.AnimationModule.Scriipts;
using Features.CameraModule.Scripts;
using Features.InjectedPrefabFactory.Scripts;
using Features.PlayerSpawnModule.Scripts;
using Zenject;

namespace Core.Contexts.GameSceneContext.Scripts
{
    public class GameSceneContextInstaller : MonoInstaller<GameSceneContextInstaller>
    {
        public override void InstallBindings()
        {
            InjectedPrefabFactoryInstaller.Install(Container);
            AnimationModuleInstaller.Install(Container);
            CameraInstaller.Install(Container);
            PlayerSpawnInstaller.Install(Container);
        }
    }
}
