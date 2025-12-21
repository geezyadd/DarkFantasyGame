using Zenject;

namespace Features.CameraModule.Scripts
{
    public class CameraInstaller : Installer<CameraInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CameraSpawnService>().AsSingle();
        }
    }
}