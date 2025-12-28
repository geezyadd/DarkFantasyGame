using Features.AnimationModule.Scriipts.PlayerData;
using Features.CameraModule.Scripts;
using Features.MovableModule.Scripts.PlayerData;
using Features.PlayerControlModule.Scripts;
using Zenject;

namespace Core.Data.Scripts
{
    public class DataInstaller : Installer<DataInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<CameraModel>().AsSingle();
            Container.Bind<PlayerAnimationControllerModel>().AsSingle();
            Container.Bind<PlayerMovableModel>().AsSingle();
            Container.Bind<PlayerControlDataModel>().AsSingle();
        }
    }
}
