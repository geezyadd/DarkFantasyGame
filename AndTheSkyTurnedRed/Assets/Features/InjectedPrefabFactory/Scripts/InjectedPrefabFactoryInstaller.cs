using Zenject;

namespace Features.InjectedPrefabFactory.Scripts
{
    public class InjectedPrefabFactoryInstaller : Installer<InjectedPrefabFactoryInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<InjectedPrefabFactory>().AsSingle();
        }
    }
}