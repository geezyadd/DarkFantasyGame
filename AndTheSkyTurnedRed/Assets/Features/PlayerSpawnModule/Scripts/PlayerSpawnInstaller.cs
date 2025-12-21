using Zenject;

namespace Features.PlayerSpawnModule.Scripts
{
    public class PlayerSpawnInstaller : Installer<PlayerSpawnInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<IPlayerSpawnService>().To<PlayerSpawnService>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerSpawnSystem>().AsSingle();
        }
    }
}