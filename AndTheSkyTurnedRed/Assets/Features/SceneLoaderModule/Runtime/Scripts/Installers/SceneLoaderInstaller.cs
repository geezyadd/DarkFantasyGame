using SceneLoaderModule;
using Zenject;

namespace Features.SceneLoaderModule.Runtime.Scripts.Installers {
    public class SceneLoaderInstaller : Installer<SceneLoaderInstaller> {
        public override void InstallBindings() {
            Container.Bind<ISceneSwitchService>()
                     .To<AddressablesSceneSwitchService>()
                     .AsSingle();
        }
    }
}