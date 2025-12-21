using Zenject;

namespace Features.AssetLoaderModule.Scripts.Installer
{
    public class AssetLoaderInstaller : Installer<AssetLoaderInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AddressableAssetLoaderService>().AsSingle();
            Container.BindInterfacesAndSelfTo<AssetLoaderService>().AsSingle();
        }
    }
}
