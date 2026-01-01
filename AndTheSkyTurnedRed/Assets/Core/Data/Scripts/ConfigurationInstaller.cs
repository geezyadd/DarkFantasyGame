using Features.AddressablesConstantsGenerator.Generated;
using Features.CameraModule.Scripts;
using Features.PlayerSpawnModule.Scripts;
using Features.PlayerStatsModule.Scripts;
using Plugins.Zenject.Addons.AddressablesConfigurationsLoader;
using Zenject;
public class ConfigurationInstaller : Installer<ConfigurationInstaller>
{
    public override void InstallBindings()
    {
        Container.BindConfigurationFromAddressables<CameraConfiguration>(Address.Configurations.CameraConfiguration_Default).AsSingle();
        Container.BindConfigurationFromAddressables<PlayerSpawnConfiguration>(Address.Configurations.PlayerSpawnConfiguration_Default).AsSingle();
        Container.BindConfigurationFromAddressables<DefaultPlayerStatsValuesConfiguration>(Address.Configurations.DefaultPlayerStatsValuesConfiguration_Default).AsSingle();
    }
}
