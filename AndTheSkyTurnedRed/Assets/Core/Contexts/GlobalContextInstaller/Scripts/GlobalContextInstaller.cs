using Core.Data.Scripts;
using Features.AssetLoaderModule.Scripts.Installer;
using Zenject;

namespace Core.Contexts.GlobalContextInstaller.Scripts
{
    public class GlobalContextInstaller : MonoInstaller<GlobalContextInstaller>
    {
        public override void InstallBindings() {
            AssetLoaderInstaller.Install(Container);
            ConfigurationInstaller.Install(Container);
            DataInstaller.Install(Container);
        }
    }
}
