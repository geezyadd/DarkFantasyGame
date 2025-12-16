using Core.InputModule.Scripts;
using Core.InputModule.Scripts.Generated;
using Zenject;

namespace Features.Input.Scripts.Installers {
    public class InputRealizationInstaller : Installer<InputRealizationInstaller> {
        public override void InstallBindings() {
            Container.Bind<InputActions>()
                .AsSingle();
            
            Container.BindInterfacesTo<InputService>()
                .AsSingle();

            Container.BindInterfacesTo<InputInitializeSystem>()
                .AsSingle();

            Container.Bind<InputModel>()
                .AsSingle();
        }
    }
}