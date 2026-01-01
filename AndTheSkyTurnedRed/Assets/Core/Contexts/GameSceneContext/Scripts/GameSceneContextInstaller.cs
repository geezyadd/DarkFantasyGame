using Features.AnimationModule.Scriipts;
using Features.CameraModule.Scripts;
using Features.EntityStatsModule.Scripts.Realization.Installers;
using Features.InjectedPrefabFactory.Scripts;
using Features.PlayerSpawnModule.Scripts;
using Features.PlayerStatsModule.Scripts;
using Features.TargetSearchModule.Scripts;
using Zenject;

namespace Core.Contexts.GameSceneContext.Scripts
{
    public class GameSceneContextInstaller : MonoInstaller<GameSceneContextInstaller>
    {
        public override void InstallBindings()
        {
            InjectedPrefabFactoryInstaller.Install(Container);
            StatsInstaller.Install(Container);
            AnimationModuleInstaller.Install(Container);
            TargetSearchModuleInstaller.Install(Container);
            CameraInstaller.Install(Container);
            PlayerStatsInstaller.Install(Container);
            PlayerSpawnInstaller.Install(Container);
        }
    }
}
