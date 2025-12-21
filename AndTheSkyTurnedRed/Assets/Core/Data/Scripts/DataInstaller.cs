using Features.CameraModule.Scripts;
using Zenject;

namespace Core.Data.Scripts
{
    public class DataInstaller : Installer<DataInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<CameraModel>().AsSingle();
        }
    }
}
