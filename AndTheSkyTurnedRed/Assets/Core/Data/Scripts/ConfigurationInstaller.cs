using Features.CameraModule.Scripts;
using Features.PlayerSpawnModule.Scripts;
using Plugins.Zenject.Addons.AddressablesConfigurationsLoader;
using RSG.SharedData.Generated;
using Zenject;
public class ConfigurationInstaller : Installer<ConfigurationInstaller>
{
    public override void InstallBindings()
    {
        Container.BindConfigurationFromAddressables<CameraConfiguration>(Address.Configurations.CameraConfiguration_Default).AsSingle();
        Container.BindConfigurationFromAddressables<PlayerSpawnConfiguration>(Address.Configurations.PlayerSpawnConfiguration_Default).AsSingle();
    }
}
