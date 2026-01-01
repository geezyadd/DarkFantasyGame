using Zenject;

namespace Features.PlayerStatsModule.Scripts
{
    public class PlayerStatsInstaller : Installer<PlayerStatsInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PlayerStatsDefaultValuesInitializationSystem>().AsSingle();
        }
    }
}